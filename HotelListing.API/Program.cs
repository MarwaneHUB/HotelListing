using HotelListing.API.Data;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace HotelListing.API;

public class Program {
    public static void Main( string[] args ) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString") ?? "";
        builder.Services.AddDbContext<HotelListingDbContext>( options => {
            options.UseSqlServer( connectionString );
        } );
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors( options => {
            options.AddPolicy( "AllowAll",b => { b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); } );
        } );

        builder.Host.UseSerilog( ( context,loggerConfig ) => 
            loggerConfig.WriteTo.Console().ReadFrom.Configuration( context.Configuration ) );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors( "AllowAll" );
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
