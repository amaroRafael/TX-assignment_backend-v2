namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TX.RMC.BusinessLogic;

[Route("[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class StatusController(RobotService robotService, ILogger<StatusController> logger) : ApiBaseController
{
    private readonly RobotService robotService = robotService;
    private readonly ILogger<StatusController> logger = logger;
    private static readonly string error = "Operation could not be executed at this moment.";


    /// <summary>
    /// Gets the status of the robot
    /// </summary>
    /// <param name="robot">The robot name identity.</param>
    /// <returns>THe current status.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /status/[robot]
    ///     Authorization Bearer [token]
    /// 
    /// </remarks>
    [HttpGet("{robot}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<StatusResponse>), Description = "Returns the robot status.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If there is an error during the process.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Get(string robot)
    {
        try
        {
            string status = await this.robotService.GetStatusAsync(robot, HttpContext.RequestAborted);
            return Ok(new StatusResponse { Robot = robot, Status = status });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Get] method to retrieve robot status.");
            return BadRequest(CreateErrorResponse(error));
        }
    }

    internal class StatusResponse
    {
        public string Robot { get; init; } = null!;
        public string Status { get; init; } = null!;
    }
}
