
using HotelListing.API.Core.Contract;
using HotelListing.API.Core.Models.Users;
using HotelListing.API.Core.Repository;

using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Contract;
[Route( "api/[controller]" )]
[ApiController]
public class AccountController : ControllerBase {

	private readonly IAuthManager _authManager;
	public AccountController(AuthManager authManager) {
		_authManager = authManager;
	}

	[HttpPost]
	[Route("register")]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Register( [FromBody] ApiUserDto apiUserDto ) {

		var errors = await _authManager.Register( apiUserDto );
		if (errors.Any()) {
			foreach (var error in errors) {
				ModelState.AddModelError( error.Code,error.Description );
			}
			return BadRequest(ModelState);
		}
		return Ok("Created");
	}

    [HttpPost]
    [Route( "login" )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized)]
    [ProducesResponseType( StatusCodes.Status200OK )]
    public async Task<IActionResult> Login( [FromBody] LoginDto apiUserDto ) {
		var authResponse = await _authManager.LoginAsync( apiUserDto );
        if (authResponse != null) return Ok( authResponse );
        return Unauthorized();
    }

    [HttpPost]
    [Route( "refreshtoken" )]
    [ProducesResponseType( StatusCodes.Status400BadRequest )]
    [ProducesResponseType( StatusCodes.Status401Unauthorized )]
    [ProducesResponseType( StatusCodes.Status200OK )]
    public async Task<IActionResult> RefreshToken( [FromBody] AuthResponseDto authResponseDto ) {
        var authResponse = await _authManager.VerifyRefreshToken( authResponseDto );
        if (authResponse != null) return Ok( authResponse );
        return Unauthorized();
    }
}
