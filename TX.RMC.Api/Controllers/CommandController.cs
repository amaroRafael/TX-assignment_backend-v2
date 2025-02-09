namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TX.RMC.Api.Extensions;
using TX.RMC.Api.Models;
using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Models;

/// <summary>
/// Command controller.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CommandController(BusinessLogic.CommandService commandService, BusinessLogic.RobotService robotService, BusinessLogic.UserService userService) : ApiBaseController
{
    private readonly CommandService commandService = commandService;
    private readonly RobotService robotService = robotService;
    private readonly UserService userService = userService;
    private static readonly string error = "Operation could not be executed at this moment.";

    /// <summary>
    /// Accepts a command to control the robot (e.g., MoveForward, RotateRight).
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Responds with the status of the robot.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Command
    ///     Authorization Bearer [token]
    ///     Body:
    ///     {
    ///         "command": "MoveForward",
    ///         "robot": "TX-010"
    ///     }
    /// 
    /// </remarks>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ApiResponse), Description = "Returns the status of the robot.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse), Description = "If command couldn't be sent to robot.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Post([FromBody] CommandRequest request)
    {
        var result = await commandService.SendAsync(request.Command, request.Robot, HttpContext.User.GetId());

        try
        {
            if (result is not null)
            {
                var status = await this.robotService.GetStatusAsync(request.Robot);
                return CreatedAtAction(nameof(Post), new { request.Robot, status });
            }

            return BadRequest(CreateFailResponse(new { Command = $"Failed to send command to robot: {request.Robot}" }));
        }
        catch (Exception)
        {
            return BadRequest(CreateErrorResponse(error));
        }
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
    ///     Authorization Bearer [token]
    ///     Body:
    ///     {
    ///         "command": "MoveBackward",
    ///         "robot": "TX-010"
    ///     }
    /// 
    /// </remarks>
    [HttpPut]
    [SwaggerResponse(StatusCodes.Status202Accepted, Type = typeof(ApiResponse), Description = "Returns the status of the robot.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse), Description = "If command couldn't be updated.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Put([FromBody] CommandRequest request)
    {
        try
        {
            Command? command = await this.commandService.UpdateAsync(request.Command, request.Robot, HttpContext.User.GetId());
            if (command is not null)
            {
                var status = await this.robotService.GetStatusAsync(request.Robot);
                return AcceptedAtAction(nameof(Put), new { request.Robot, status });
            }

            return BadRequest(CreateFailResponse(new { Command = $"Failed to update command to robot: {request.Robot}" }));
        }
        catch (Exception)
        {
            return BadRequest(CreateErrorResponse(error));
        }
    }

    /// <summary>
    /// Gets the details of a command.
    /// </summary>
    /// <param name="id">The command identity.</param>
    /// <returns>The command details.</returns>
    /// <remarks>
    /// 
    ///     GET /command/[id]
    ///     Authorization Bearer [token]
    /// 
    /// </remarks>
    [HttpGet("{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse), Description = "Returns the command details.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ApiResponse), Description = "If the command is not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If command couldn't be retrieved.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            Command? command = await this.commandService.GetAsync(id);
            if (command != null)
            {
                Robot? robot = await this.robotService.GetAsync(command.RobotId);
                DataAccess.Core.Models.User? user = await this.userService.GetAsync(command.UserId);

                return Ok(new
                {
                    Command = new
                    {
                        command.Id,
                        command.Action,
                        command.CreatedAt,
                        Robot = new
                        {
                            robot?.Id,
                            robot?.NameIdentity,
                            command.PositionX,
                            command.PositionY,
                            command.Direction
                        },
                        User = new
                        {
                            user?.Id,
                            user?.Name,
                            user?.Username
                        }
                    }
                });
            }

            return NotFound(CreateFailResponse(null));
        }
        catch (Exception)
        {
            return BadRequest(CreateErrorResponse(error));
        }
    }
}
