namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Models;

[Route("[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class RobotController(RobotService robotService, ILogger<RobotController> logger) : ApiBaseController
{
    private readonly RobotService robotService = robotService;
    private readonly ILogger<RobotController> logger = logger;
    private static readonly string error = "Operation could not be executed at this moment.";

    /// <summary>
    /// Adds a new robot to the system.
    /// </summary>
    /// <param name="name">Robot name identity.</param>
    /// <returns>Returns the robot details.</returns>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<Robot>), Description = "Returns the robot details.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If there is an error during the process.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Post([FromForm(Name = "name")] string name)
    {
        try
        {
            Robot robot = await this.robotService.AddAsync(name, HttpContext.RequestAborted);
            return Ok(robot);
        }
        catch (ArgumentException argEx)
        {
            this.logger.LogError(argEx,
                $"Error executing [Post] method of type ArgumentException occurred when it was adding a new robot. Parameter {argEx.ParamName} with message \"{argEx.Message}\".");
            return BadRequest(CreateFailResponse(new { Parameter = argEx.ParamName, argEx.Message }));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Post] method to add a new robot.");
            return BadRequest(CreateErrorResponse(error));
        }
    }
}
