using AutoMapper;

using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Core.Models.Users;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;

namespace HotelListing.API.Core.Configurations;

public class MapperConfig : Profile {
    public MapperConfig() {
        CreateMap<Country,CreateCountryDto>().ReverseMap();
        CreateMap<Country,GetCountryDto>().ReverseMap();
        CreateMap<Country,CountryDto>().ReverseMap();
        CreateMap<Hotel,HotelDto>().ReverseMap();
        CreateMap<Hotel,GetHotelDto>().ReverseMap();

        CreateMap<ApiUser,ApiUserDto>().ReverseMap();

    }
}
