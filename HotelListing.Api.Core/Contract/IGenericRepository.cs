using HotelListing.API.Core.Models;

namespace HotelListing.API.Core.Contract;

public interface IGenericRepository<T> where T : class {
    Task<T> GetAsync(int? id);
    Task<bool> Exists(int id);
    Task<T> AddAsync(T entity);
    Task<List<T>> GetAllAsync();
    Task<PagedResult<TResult>>  GetAllAsync<TResult>(QueryParameters queryParameters);
    Task DeleteAsync(int id);
    Task UpdateAsync( T entity);
}
