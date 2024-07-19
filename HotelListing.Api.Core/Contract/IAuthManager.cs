using HotelListing.API.Core.Models.Users;

using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Core.Contract;

public interface IAuthManager {
    Task<IEnumerable<IdentityError>> Register( ApiUserDto apiUserDto );
    Task<AuthResponseDto> LoginAsync( LoginDto loginDto );
    Task<AuthResponseDto> VerifyRefreshToken( AuthResponseDto request );
    Task<string> CreateRefreshToken();
}
