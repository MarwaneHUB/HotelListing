using System.Text;

using AutoMapper;
using HotelListing.API.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using HotelListing.API.Core.Models.Users;
using HotelListing.API.Core.Contract;

namespace HotelListing.API.Core.Repository;

public class AuthManager : IAuthManager {
    const string TOKEN_PROVIDER = "HotelListingApi";
    private const string TOKEN_PURPOSE = "RefreshToken";
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser>  _userManager;
    readonly IConfiguration _configuration;
    private ApiUser _user;

    public AuthManager( IMapper mapper,UserManager<ApiUser> userManager,IConfiguration configuration ) {
        _configuration = configuration;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IdentityError>> Register( ApiUserDto apiUserDto ) {
        _user = _mapper.Map<ApiUser>( apiUserDto );
        _user.UserName = apiUserDto.Email;

        var result = await _userManager.CreateAsync( _user , apiUserDto.Password );
        if (result.Succeeded) await _userManager.AddToRoleAsync( _user,"User" );
        return result.Errors;
    }

    public async Task<AuthResponseDto> LoginAsync( LoginDto loginDto ) {

        _user = await _userManager.FindByEmailAsync( loginDto.Email );
        var isValidUser = await _userManager.CheckPasswordAsync( _user,loginDto.Password );
        if (isValidUser && _user != null)
            return new AuthResponseDto {
                Token = await GenerateTokenAsync(),
                UserId = _user.Id
            };

        return null;
    }

    public async Task<string> GenerateTokenAsync() {
        var seucurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
        var credentials = new SigningCredentials(seucurityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var userClaims = await _userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>{
            new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email),
            new Claim("uid", _user.Id),
        }.Union(userClaims).Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
            ) ;

        return new JwtSecurityTokenHandler().WriteToken( token );
    }

    public async Task<AuthResponseDto> VerifyRefreshToken( AuthResponseDto request ) {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken( request.Token );
        var userName =
            tokenContent?.Claims.FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
        _user = await _userManager.FindByEmailAsync( userName );

        if (_user == null) return null;
        var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, TOKEN_PROVIDER, TOKEN_PURPOSE, request.Token);

        if (isValidRefreshToken) {
            await _userManager.UpdateSecurityStampAsync( _user );
            return new AuthResponseDto {
                Token = await GenerateTokenAsync(),
                UserId = _user.Id
            };
        }

        return null;

    }

    public async Task<string> CreateRefreshToken() {
        await _userManager.RemoveAuthenticationTokenAsync( _user,TOKEN_PROVIDER,TOKEN_PURPOSE );
        var refreshToken =
            await _userManager.GenerateUserTokenAsync( _user,TOKEN_PROVIDER,"RefreshToken" );

        await _userManager.SetAuthenticationTokenAsync( _user,TOKEN_PROVIDER,"RefreshToken",refreshToken );
        return refreshToken;
    }
}
