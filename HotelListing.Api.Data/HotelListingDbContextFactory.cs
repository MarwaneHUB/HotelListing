
using HotelListing.API.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelListing.Api.Data;
public class HotelListingDbContextFactory : IDesignTimeDbContextFactory<HotelListingDbContext> {

    /// <summary>
    /// Problem of Scafoldin when DbContext is not in the same project
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public HotelListingDbContext CreateDbContext( string[] args ) {

        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<HotelListingDbContext>();
        var connectionString = config?.GetConnectionString("HotelListingDbConnectionString") ?? "";
        optionsBuilder.UseSqlServer(connectionString);

        return new HotelListingDbContext(optionsBuilder.Options);

    }
}
