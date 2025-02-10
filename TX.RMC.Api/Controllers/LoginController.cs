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
public class LoginController(IdentityService identityService, UserService userService, ILogger<LoginController> logger) : ControllerBase
{
    private readonly IdentityService identityService = identityService;
    private readonly UserService userService = userService;
    private readonly ILogger<LoginController> logger = logger;
    private static readonly string errorInvalid = "Invalid username and/or password";
    private static readonly string errorFailed = "Failed to authenticate user.";

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="username">The user name identity.</param>
    /// <param name="password">The user secret.</param>
    /// <returns>Authentication token (JWT).</returns>
    /// <remarks>
    /// Sample request: 
    ///
    ///     POST /login
    ///     Content-Type: multipart/form-data
    ///     Parameters:
    ///         username: string
    ///         password: string
    /// 
    /// </remarks>
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
            this.logger.LogError(argEx,
                $"Error executing [Post] method of type ArgumentException occurred when it was authenticating the user. Parameter {argEx.ParamName} with message \"{argEx.Message}\".");
            return BadRequest(ApiBaseController.CreateFailResponse(new { Parameter = argEx.ParamName, argEx.Message }));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Post] method to authenticate the user.");
            return BadRequest(ApiBaseController.CreateErrorResponse(errorFailed));
        }
    }

    /// <summary>
    /// Sign up a new user
    /// </summary>
    /// <returns>Returns the user identity.</returns>
    /// <remarks>
    /// Sample request: 
    ///
    ///     POST /login/sign-up
    ///     Content-Type: multipart/form-data
    ///     Parameters:
    ///         name: string
    ///         username: string
    ///         password: string
    /// 
    /// </remarks>
    [HttpPost("sign-up")]
    [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ApiResponse<string>), Description = "Returns the user identity.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If the name, username or password is missing.")]
    public async Task<IActionResult> SignUp([FromForm(Name = "name")] string name, [FromForm(Name = "username")] string username, [FromForm(Name = "password")] string password)
    {
        try
        {
            var user = await this.userService.AddAsync(name, username, password, HttpContext.RequestAborted);

            if (user is null)
            {
                this.logger.LogWarning("Unable to save user.");
                return BadRequest(ApiBaseController.CreateErrorResponse("User could not be signed up at this moment. Try again later."));
            }

            return CreatedAtAction(nameof(SignUp), user.Id);
        }
        catch (ArgumentException argEx)
        {
            this.logger.LogError(argEx,
                $"Error executing method [SignUp] of type ArgumentException occurred when it was signing up the user. Parameter {argEx.ParamName} with message \"{argEx.Message}\".");
            return BadRequest(ApiBaseController.CreateFailResponse(new { Parameter = argEx.ParamName, argEx.Message }));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Post] method to sign up the user.");
            return BadRequest(ApiBaseController.CreateErrorResponse(errorFailed));
        }
    }
}
