using System.Text;

using HotelListing.API.Core.Configurations;
using HotelListing.API.Core.Contract;
using HotelListing.API.Data;
using HotelListing.API.Core.Middlewares;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Serilog;
using HotelListing.API.Core.Repository;

namespace HotelListing.API;

public class Program {
    public static void Main( string[] args ) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString") ?? "";
        builder.Services.AddDbContext<HotelListingDbContext>( options => {
            options.UseSqlServer( connectionString );
        } );
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors( options => {
            options.AddPolicy( "AllowAll",b => { b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); } );
        } );

        builder.Services.AddApiVersioning( options => {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion( 1,0 );
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver")
                );
        } );

        builder.Services.AddVersionedApiExplorer( options => {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl= true;
        } );

        builder.Host.UseSerilog( ( context,loggerConfig ) =>
            loggerConfig.WriteTo.Console().ReadFrom.Configuration( context.Configuration ) );
        builder.Services.AddAutoMapper( typeof( MapperConfig ) );

        builder.Services.AddScoped( typeof( IGenericRepository<> ),typeof( GenericRepository<> ) );
        builder.Services.AddScoped<ICountriesRepository,CountriesRepository>();
        builder.Services.AddScoped<IAuthManager,AuthManager>();

        builder.Services.AddIdentityCore<ApiUser>()
            .AddRoles<IdentityRole>()
            .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingApi")
            .AddEntityFrameworkStores<HotelListingDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddResponseCaching( options => {
            options.UseCaseSensitivePaths = true;
            options.MaximumBodySize = 1024;
        } );

        builder.Services.AddAuthentication( options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        } ).AddJwtBearer( options => {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( builder.Configuration["JwtSettings:Key"] ) )
            };
        } );

        builder.Services.AddControllers().AddOData( options => {
            options.Select().Filter().OrderBy();
        } );
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseCors( "AllowAll" );

        app.Use( async ( context,next ) => {
            context.Response.GetTypedHeaders().CacheControl =
                new Microsoft.Net.Http.Headers.CacheControlHeaderValue {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds( 10 ),
                };

            context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = 
                new string[] { "Accept-Encoding" };

            await next();
        } );

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
