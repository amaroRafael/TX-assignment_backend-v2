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
    public async Task<IActionResult> Get(string robot)
    {
        string status = await this.robotService.GetStatusAsync(robot);
        return new OkObjectResult(new { robot, status });
    }
}
