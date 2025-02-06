namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TX.RMC.BusinessLogic;

[Route("[controller]")]
[ApiController]
[Authorize]
public class StatusController(RobotService robotService) : ControllerBase
{
    private readonly RobotService robotService = robotService;
    private static readonly string[] error = ["Operation could not be executed at this moment."];


    /// <summary>
    /// Gets the status of the robot
    /// </summary>
    /// <param name="robot">The robot name identity.</param>
    /// <returns>THe current status.</returns>
    /// <remarks>
    /// 
    ///     GET /status/[robot]
    ///     Authorization Bearer [token]
    /// 
    /// </remarks>
    /// <response code="200">Returns the robot status.</response>
    [HttpGet("{robot}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(string robot)
    {
        try
        {
            string status = await this.robotService.GetStatusAsync(robot);
            return Ok(new { robot, status });
        }
        catch (Exception)
        {
            return BadRequest(new
            {
                Error = error
            });
        }
    }
}
