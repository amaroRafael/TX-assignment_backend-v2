namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TX.RMC.Api.Services;
using TX.RMC.BusinessLogic;

[Route("login")]
[ApiController]
[AllowAnonymous]
public class LoginController(IdentityService identityService, UserService userService) : ApiBaseController
{
    private readonly IdentityService identityService = identityService;
    private readonly UserService userService = userService;
    private static readonly string errorInvalid = "Invalid username and/or password";
    private static readonly string errorFailed = "Failed to authenticate user.";

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

            return BadRequest(CreateFailResponse(new { Login = errorInvalid }));
        }
        catch (ArgumentException argEx)
        {
            return BadRequest(CreateFailResponse(new { Parameter = argEx.ParamName, argEx.Message }));
        }
        catch (Exception)
        {
            return BadRequest(CreateErrorResponse(errorFailed));
        }
    }
}
