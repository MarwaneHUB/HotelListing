using AutoMapper;

using HotelListing.API.Core.Contract;
using HotelListing.API.Data;

using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Core.Repository;

public class CountriesRepository : GenericRepository<Country>, ICountriesRepository {
    private readonly HotelListingDbContext _dbContext;
    public CountriesRepository( HotelListingDbContext dbContext,IMapper mapper ) : base( dbContext,mapper ) {
        _dbContext = dbContext;
    }

    public async Task<Country> GetDetailedAsync( int? id ) {
        return await _dbContext.Countries.Include( q => q.Hotels ).FirstOrDefaultAsync( q => q.Id == id );
    }
}