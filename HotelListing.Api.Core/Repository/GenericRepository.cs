using AutoMapper;
using AutoMapper.QueryableExtensions;

using HotelListing.API.Core.Contract;
using HotelListing.API.Core.Models;
using HotelListing.API.Data;
using HotelListing.API.Models;

using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Core.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class {

    private readonly HotelListingDbContext _context;
    readonly IMapper _mapper;

    public GenericRepository( HotelListingDbContext dbContext, IMapper mapper) {
        _mapper = mapper;
        _context = dbContext;
    }
    public async Task<T> AddAsync( T entity ) {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync( int id ) {
        var entity = await GetAsync(id);
        _context.Remove( entity );
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists( int id ) {
        return await GetAsync( id ) != null;
    }

    public async Task<List<T>> GetAllAsync() {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<PagedResult<TResult>> GetAllAsync<TResult>( QueryParameters queryParameters ) {
        var totalSize = _context.Set<T>().Count();

        var items = _context.Set<T>()
            .Skip(queryParameters.StartIndex)
            .Take(queryParameters.PageSize)
            .ProjectTo<TResult>(_mapper.ConfigurationProvider) // Exact columns to Query
            .ToList();

        return new PagedResult<TResult> {
            Records = items,
            RecordNumber = queryParameters.PageSize,
            PageNumber = queryParameters.PageNumber,
            TotalCount = totalSize
        };
    }

    public async Task<T> GetAsync( int? id ) {
        if (id is null) return null;
        return await _context.Set<T>().FindAsync( id );
    }

    public async Task UpdateAsync( T entity ) {
         _context.Update( entity);
         await _context.SaveChangesAsync();
    }
}
