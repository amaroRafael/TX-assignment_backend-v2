namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TX.RMC.Api.Extensions;
using TX.RMC.Api.Models;
using TX.RMC.BusinessLogic;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;

/// <summary>
/// Command controller.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CommandController(BusinessLogic.CommandService commandService, BusinessLogic.RobotService robotService, BusinessLogic.UserService userService, ILogger<CommandController> logger) : ApiBaseController
{
    private readonly CommandService commandService = commandService;
    private readonly RobotService robotService = robotService;
    private readonly UserService userService = userService;
    private readonly ILogger<CommandController> logger = logger;
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
    [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ApiResponse<StatusController.StatusResponse>), Description = "Returns the status of the robot.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<CommandFailedResponse>), Description = "If command couldn't be sent to robot.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Post([FromBody] CommandRequest request)
    {
        var result = await commandService.SendAsync(request.Command, request.Robot, HttpContext.User.GetId(), HttpContext.RequestAborted);

        try
        {
            if (result is not null)
            {
                var status = await this.robotService.GetStatusAsync(request.Robot, HttpContext.RequestAborted);
                return CreatedAtAction(nameof(Post), new StatusController.StatusResponse { Robot = request.Robot, Status = status });
            }

            return BadRequest(CreateFailResponse(new CommandFailedResponse { Command = $"Failed to send command to robot: {request.Robot}" }));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Post] method to send command.");
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
    [SwaggerResponse(StatusCodes.Status202Accepted, Type = typeof(ApiResponse<StatusController.StatusResponse>), Description = "Returns the status of the robot.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<CommandFailedResponse>), Description = "If command couldn't be updated.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Put([FromBody] CommandRequest request)
    {
        try
        {
            Command? command = await this.commandService.UpdateAsync(request.Command, request.Robot, HttpContext.User.GetId(), HttpContext.RequestAborted);
            if (command is not null)
            {
                var status = await this.robotService.GetStatusAsync(request.Robot, HttpContext.RequestAborted);
                return AcceptedAtAction(nameof(Put), new StatusController.StatusResponse { Robot = request.Robot, Status = status });
            }

            return BadRequest(CreateFailResponse(new CommandFailedResponse { Command = $"Failed to update command to robot: {request.Robot}" }));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Put] method to modify command.");
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
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<CommandDetailsResponse>), Description = "Returns the command details.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>), Description = "If the command is not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse), Description = "If command couldn't be retrieved.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "If the user is not authenticated.")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            Command? command = await this.commandService.GetAsync(id, HttpContext.RequestAborted);
            if (command != null)
            {
                Robot? robot = await this.robotService.GetAsync(command.RobotId, HttpContext.RequestAborted);
                DataAccess.Core.Models.User? user = await this.userService.GetAsync(command.UserId, HttpContext.RequestAborted);

                return Ok(new
                {
                    Command = new CommandDetailsResponse
                    {
                        Id = command.Id,
                        Action = command.Action,
                        ExecutedAt = command.CreatedAt,
                        Robot = new CommandDetailsResponse.RobotResponse
                        {
                            Id = robot?.Id,
                            NameIdentity = robot?.NameIdentity,
                            PositionX = command.PositionX,
                            PositionY = command.PositionY,
                            Direction = command.Direction
                        },
                        User = new CommandDetailsResponse.UserResponse
                        {
                            Id = user?.Id,
                            Name = user?.Name,
                            Username = user?.Username
                        }
                    }
                });
            }

            return NotFound(CreateFailResponse<object>(null));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing [Get] method to retrieve command.");
            return BadRequest(CreateErrorResponse(error));
        }
    }

    class CommandFailedResponse
    {
        public string Command { get; internal set; } = null!;
    }

    class CommandDetailsResponse
    {
        public object Id { get; internal set; } = null!;
        public ECommands Action { get; internal set; }
        public DateTime ExecutedAt { get; internal set; }
        public RobotResponse Robot { get; internal set; } = null!;
        public UserResponse User { get; internal set; } = null!;

        public class RobotResponse
        {
            public object? Id { get; internal set; }
            public string? NameIdentity { get; internal set; }
            public int PositionX { get; internal set; }
            public int PositionY { get; internal set; }
            public EDirections Direction { get; internal set; }
        }

        public class UserResponse
        {
            public object? Id { get; internal set; }
            public string? Name { get; internal set; }
            public string? Username { get; internal set; }
        }
    }
}
