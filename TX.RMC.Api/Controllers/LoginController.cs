namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TX.RMC.Api.Services;

[Route("login")]
[ApiController]
[AllowAnonymous]
public class LoginController(IdentityService identityService) : ControllerBase
{
    private readonly IdentityService identityService = identityService;
    private static readonly string[] errorInvalid = ["Invalid username or password"];
    private static readonly string[] errorFailed = ["Failed to authenticate user."];

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="username">The user name identity.</param>
    /// <param name="password">The user secret.</param>
    /// <returns>Authentication token (JWT).</returns>
    /// <remarks>
    /// 
    ///     POST /login
    ///     Content-Type: multipart/form-data
    ///     Parameters:
    ///         username: string
    ///         password: string
    /// 
    /// </remarks>
    /// <response code="200">Returns the authentication token (JWT).</response>
    /// <response code="400">If the username or password is invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromForm(Name = "username")] string username, [FromForm(Name = "password")] string password)
    {
        try
        {
            var result = await this.identityService.LoginAsync(username, password, HttpContext.RequestAborted);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new
            {
                Success = false,
                Errors = errorInvalid
            });
        }
        catch (ArgumentException argEx)
        {
            return BadRequest(new
            {
                Success = false,
                Errors = new[] { argEx.Message }
            });
        }
        catch (Exception)
        {
            return BadRequest(new
            {
                Success = false,
                Errors = errorFailed
            });
        }
    }
}
