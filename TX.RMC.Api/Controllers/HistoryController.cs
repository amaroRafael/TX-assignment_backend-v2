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
public class HistoryController(RobotService robotService, ILogger<HistoryController> logger) : ApiBaseController
{
    private readonly RobotService robotService = robotService;
    private readonly ILogger<HistoryController> logger = logger;
    private static readonly string error = "Operation could not be executed at this moment.";

    /// <summary>
    /// Gets the robot command history executed.
    /// </summary>
    /// <param name="robot">The robot name identity</param>
    /// <param name="count">Maximum number of commands</param>
    /// <returns>Returns the most recent commands executed.</returns>
    /// <remarks>
    /// 
    ///     GET /history/[robot]
    ///     Authorization Bearer [token]
    /// 
    /// </remarks>
    [HttpGet("{robot}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<HistoryItem>>), Description = "Returns the robot commands executed most recently.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If there is an error during the process.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> GetHistoryAsync(string robot, [FromQuery(Name = "count")] int count = 10)
    {
        try
        {
            var commandHistory = await this.robotService.GetCommandHistoryAsync(robot, count, HttpContext.RequestAborted);
            return Ok(commandHistory.Select(c => new HistoryItem { Id = c.Id, Command = c.Command, ExecutedAt = c.ExecutedAt }));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [GetHistoryAsync] method.");
            return BadRequest(CreateErrorResponse(error));
        }
    }

    public class  HistoryItem
    {
        public object Id { get; init; } = null!;
        public string Command { get; init; } = null!;
        public DateTime ExecutedAt { get; init; }
    }
}
