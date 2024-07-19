using HotelListing.API.Data;

namespace HotelListing.API.Core.Contract;

public interface ICountriesRepository : IGenericRepository<Country> {
    Task<Country> GetDetailedAsync(int? id);
}
