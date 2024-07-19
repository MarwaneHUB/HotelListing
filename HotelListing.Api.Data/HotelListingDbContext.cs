using HotelListing.API.Data.Configurations;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public class HotelListingDbContext : IdentityDbContext<ApiUser> {
    public HotelListingDbContext( DbContextOptions options ) : base( options ) {

    }

    protected override void OnModelCreating( ModelBuilder modelBuilder ) {
        base.OnModelCreating( modelBuilder );
        modelBuilder.ApplyConfiguration( new RoleConfiguration() );
        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1,Name = "Jamaica",ShortName = "JM" },
			new Country { Id = 2,Name = "Bahamas",ShortName = "BS" },
			new Country { Id = 3,Name = "Cayman Island",ShortName = "CI" }
		);

        modelBuilder.Entity<Hotel>().HasData(
           new Hotel { Id = 1,Name = "Sandals Resort & Spa",Address = "Negril",Rating = 4.3, CountryId = 1 },
           new Hotel { Id = 2,Name = "Comfort Suites",Address = "George Town",Rating = 4.3, CountryId = 3 },
           new Hotel { Id = 3,Name = "Grand Paldium",Address = "Nassua",Rating = 4.3, CountryId = 2 }
          
       );
    }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Hotel> Hotels { get; set; }
}
