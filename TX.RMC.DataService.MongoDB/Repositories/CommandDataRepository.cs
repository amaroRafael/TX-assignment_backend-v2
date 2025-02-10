namespace TX.RMC.DataService.MongoDB.Repositories;

using global::MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.DataService.MongoDB.Options;

internal class CommandDataRepository(MongoDbContext mongoDbContext) : ICommandDataRepository
{
    private readonly MongoDbContext mongoDbContext = mongoDbContext;

    public async ValueTask<Command> AddAsync(Command model, CancellationToken cancellationToken = default)
    {
        Models.Robot robot = await this.mongoDbContext.Robots
            .Where(r => r.Id == model.RobotId.ToString())
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Robot with id {model.RobotId} not found.");

        Models.Command command = await AddAsync(model, robot, cancellationToken);

        model.Id = command.Id;

        return model;
    }

    public async ValueTask<IEnumerable<Command>> GetAllByRobotAsync(object robotId, int count, CancellationToken cancellationToken = default)
    {
        Models.Robot robot = await this.mongoDbContext.Robots
            .Where(r => r.Id == robotId.ToString())
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Robot with id {robotId} not found.");

        var commands = robot.Commands
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToList();

        return commands.Select(c => TransformToCommand(c, robot.Id));
    }

    public async ValueTask<Command?> GetByIdAsync(object robotId, object id, CancellationToken cancellationToken = default)
    {
        IEnumerable<Models.Command> commands = await this.mongoDbContext.Robots
            .Where(r => r.Id == robotId.ToString())
            .SelectMany(r => r.Commands)
            .ToListAsync(cancellationToken);

        Guid commandId = Guid.Parse(id.ToString()!);
        Models.Command? command = commands.FirstOrDefault(c => c.Id == commandId);

        return command == null ? null : TransformToCommand(command, robotId.ToString()!);
    }

    public async ValueTask<Command?> GetLastCommandExecutedAsync(object robotId, CancellationToken cancellationToken = default)
    {
        Models.Command? command = await this.mongoDbContext.Robots
            .Where(r => r.Id == robotId.ToString())
            .SelectMany(r => r.Commands)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var result = command == null ? null : TransformToCommand(command, robotId.ToString()!);

        return await ValueTask.FromResult(result);
    }

    public async Task SetReplacedByCommandAsync(Command command, Command replacedByCommand, CancellationToken cancellationToken = default)
    {
        Models.Robot robot = await this.mongoDbContext.Robots
            .Where(r => r.Id == replacedByCommand.RobotId.ToString())
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Robot with id {replacedByCommand.RobotId} not found.");

        Models.Command replacedByCommandDB = await AddAsync(replacedByCommand, robot, cancellationToken);

        Guid guid = Guid.Parse(command.Id.ToString()!);
        Models.Command commandDB = robot.Commands.Where(c => c.Id == guid).SingleOrDefault() ?? throw new InvalidOperationException($"Command with id {command.Id} not found.");

        commandDB.ReplacedByCommand = replacedByCommandDB;

        this.mongoDbContext.Robots.Update(robot);
        await this.mongoDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Models.Command> AddAsync(Command model, Models.Robot robot, CancellationToken cancellationToken)
    {
        Models.Command command = new()
        {
            Id = Guid.NewGuid(),
            Action = model.Action,
            CreatedAt = model.CreatedAt,
            UserId = model.UserId.ToString() ?? string.Empty,
            LogState = new()
            {
                BeforeExecution = new()
                {
                    PositionX = robot.CurrentState?.PositionX ?? 0,
                    PositionY = robot.CurrentState?.PositionY ?? 0,
                    Direction = robot.CurrentState?.Direction ?? DataAccess.Core.Enumerators.EDirections.North,
                },
                AfterExecution = new()
                {
                    PositionX = model.PositionX,
                    PositionY = model.PositionY,
                    Direction = model.Direction,
                }
            }
        };

        robot.CurrentState = new()
        {
            PositionX = model.PositionX,
            PositionY = model.PositionY,
            Direction = model.Direction,
        };

        this.mongoDbContext.Robots.Update(robot);
        await this.mongoDbContext.SaveChangesAsync(cancellationToken);
        return command;
    }

    private static Command TransformToCommand(Models.Command command, string robotId)
    {
        return new Command
        {
            Id = command.Id,
            RobotId = robotId,
            UserId = command.UserId,
            Action = command.Action,
            CreatedAt = command.CreatedAt,
            Direction = command.LogState.AfterExecution.Direction,
            PositionX = command.LogState.AfterExecution.PositionX,
            PositionY = command.LogState.AfterExecution.PositionY,
            ReplacedByCommandId = command.ReplacedByCommand?.Id,
        };
    }
}
