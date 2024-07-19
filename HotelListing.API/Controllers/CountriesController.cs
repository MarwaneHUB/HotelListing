using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Contract;
using HotelListing.API.Models;
using HotelListing.API.Core.Contract;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models;

namespace HotelListing.API.Controllers {
    [Route( "api/[controller]" )]
    [ApiController]
    public class CountriesController : ControllerBase {
        private readonly ICountriesRepository _countriesRepository;
        private readonly IMapper _mapper;

        public CountriesController( IMapper mapper,ICountriesRepository countriesRepository ) {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
        }

        // GET: api/Countries
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries() {
            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<IList<GetCountryDto>>(countries);
            return Ok( records );
        }

        // GET: api/Countries/?StartIndex=2&pagesize=25&PageNumber=2
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries( [FromQuery] QueryParameters queryParameters) {
            return Ok( await _countriesRepository.GetAllAsync<GetCountryDto>( queryParameters ) );
        }

        // GET: api/Countries/5
        [HttpGet( "{id}" )]
        public async Task<ActionResult<CountryDto>> GetCountry( int id ) {
            if (await _countriesRepository.GetAsync( id ) == null) {
                return NotFound();
            }
            var country =await _countriesRepository.GetDetailedAsync(id);

            if (country == null) {
                return NotFound();
            }
            return _mapper.Map<CountryDto>( country );
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut( "{id}" )]
        public async Task<IActionResult> PutCountry( int id,UpdateCountryDto updateCountryDto ) {

            if (id != updateCountryDto.Id)
                return BadRequest();

            var country = await _countriesRepository.GetAsync(id);
            var entity =  _mapper.Map(updateCountryDto, country);

            try {
                await _countriesRepository.UpdateAsync( entity );
            } catch (DbUpdateConcurrencyException) {
                if (!await _countriesRepository.Exists( id ))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CountryDto>> PostCountry( CreateCountryDto createCountryDto ) {

            if (createCountryDto == null) return Problem( "Entity is null." );

            var entity = _mapper.Map<Country>(createCountryDto);
            await _countriesRepository.AddAsync( entity );

            return CreatedAtAction( "GetCountry",new { id = entity.Id },createCountryDto );
        }

        // DELETE: api/Countries/5
        [HttpDelete( "{id}" )]
        public async Task<IActionResult> DeleteCountry( int id ) {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null) return NotFound();

            await _countriesRepository.DeleteAsync( id );

            return NoContent();
        }

    }
}
