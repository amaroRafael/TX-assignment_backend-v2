namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics;
using TX.RMC.Api.Services;
using TX.RMC.BusinessLogic;

[Route("login")]
[ApiController]
[AllowAnonymous]
[Produces("application/json")]
public class LoginController(IdentityService identityService, UserService userService) : ControllerBase
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
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Models.AuthenticationResponse), Description = "Returns the authentication token (JWT).")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If the username or password is invalid.")]
    public async Task<IActionResult> Post([FromForm(Name = "username")] string username, [FromForm(Name = "password")] string password)
    {
        try
        {
            var result = await this.identityService.LoginAsync(username, password, HttpContext.RequestAborted);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(ApiBaseController.CreateFailResponse(new { Login = errorInvalid }));
        }
        catch (ArgumentException argEx)
        {
            return BadRequest(ApiBaseController.CreateFailResponse(new { Parameter = argEx.ParamName, argEx.Message }));
        }
        catch (Exception)
        {
            return BadRequest(ApiBaseController.CreateErrorResponse(errorFailed));
        }
    }
}
