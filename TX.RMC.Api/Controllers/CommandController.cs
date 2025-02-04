namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TX.RMC.Api.Extensions;
using TX.RMC.Api.Models;
using TX.RMC.BusinessLogic;

/// <summary>
/// Command controller.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
public class CommandController(BusinessLogic.CommandService commandService, BusinessLogic.StatusService statusService) : ControllerBase
{
    private readonly CommandService commandService = commandService;
    private readonly StatusService statusService = statusService;

    /// <summary>
    /// Accepts a command to control the robot (e.g., MoveForward, RotateRight).
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Responds with the status of the robot.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Command
    ///     Authorization Bearer token
    ///     Body:
    ///     {
    ///         "command": "MoveForward",
    ///         "robot": "TX-010"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns the status of the robot.</response>
    /// <response code="400">If command couldn't be sent to robot.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CommandRequest request)
    {
        bool result = await commandService.SendAsync(request.Command, request.Robot, HttpContext.User.GetId());

        if (result)
        {
            var status = await this.statusService.GetStatusAsync(request.Robot);
            return CreatedAtAction(nameof(Post), new { request.Robot, status });
        }
        return BadRequest(new { Error = $"Failed to send command to robot: {request.Robot}" });
    }

    /// <summary>
    /// Updates an existing command.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Responds with the status of the robot.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /Command
    ///     Authorization Bearer token
    ///     Body:
    ///     {
    ///         "command": "MoveBackward",
    ///         "robot": "TX-010"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns the status of the robot.</response>
    /// <response code="400">If command couldn't be updated.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] CommandRequest request)
    {
        bool result = await commandService.UpdateAsync(request.Command, request.Robot, HttpContext.User.GetId());
        if (result)
        {
            var status = await this.statusService.GetStatusAsync(request.Robot);
            return AcceptedAtAction(nameof(Put), new { request.Robot, status });
        }
        return BadRequest(new { Error = $"Failed to update command to robot: {request.Robot}" });
    }

    /// <summary>
    /// Gets the details of a command.
    /// </summary>
    /// <param name="id">The command identity.</param>
    /// <returns>The command details.</returns>
    /// <reponse code="200">Returns the command details.</reponse>
    /// <reponse code="404">If the command is not found.</reponse>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var command = await this.commandService.GetAsync(id);
        if (command != null)
        {
            return Ok(command);
        }

        return NotFound();
    }
}
