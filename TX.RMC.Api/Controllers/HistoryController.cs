namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TX.RMC.BusinessLogic;

[Route("[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class HistoryController(RobotService robotService) : ApiBaseController
{
    private readonly RobotService robotService = robotService;
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
    /// <response code="200">Returns the robot commands executed most recently.</response>
    [HttpGet("{robot}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistoryAsync(string robot, [FromQuery(Name = "count")] int count = 10)
    {
        try
        {
            var commandHistory = await this.robotService.GetCommandHistoryAsync(robot, count);
            return Ok(commandHistory);
        }
        catch (Exception)
        {
            return BadRequest(CreateErrorResponse(error));
        }
    }
}
