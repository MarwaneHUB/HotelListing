using System.ComponentModel.DataAnnotations.Schema;

using HotelListing.API.Models.Country;

namespace HotelListing.API.Core.Models.Hotel;

public class HotelDto : BaseHotelDto {
    public int Id { get; set; }
 
}
