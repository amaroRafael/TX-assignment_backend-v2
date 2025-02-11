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

internal class CommandDataRepository(MongoDBOptions mongoDBOptions) : ICommandDataRepository
{
    private readonly MongoDBOptions mongoDBOptions = mongoDBOptions;

    public async ValueTask<Command> AddAsync(Command model, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        using MongoDbContext dbContext = MongoDbContext.Create(database);

        Models.Robot robot = await dbContext.Robots
            .Where(r => r.Id == model.RobotId.ToString())
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Robot with id {model.RobotId} not found.");

        Models.Command command = AddCommandToRobot(model, ref robot);

        model.Id = command.Id.ToString();

        return model;
    }

    public async ValueTask<IEnumerable<Command>> GetAllByRobotAsync(string robotId, int count, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        using MongoDbContext dbContext = MongoDbContext.Create(database);

        Models.Robot robot = await dbContext.Robots
            .Where(r => r.Id == robotId)
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Robot with id {robotId} not found.");

        var commands = robot.Commands
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToList();

        return commands.Select(c => TransformToCommand(c, robot.Id));
    }

    public async ValueTask<Command?> GetByIdAsync(string robotId, string id, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        using MongoDbContext dbContext = MongoDbContext.Create(database);

        IEnumerable<Models.Command> commands = await dbContext.Robots
            .Where(r => r.Id == robotId)
            .SelectMany(r => r.Commands)
            .ToListAsync(cancellationToken);

        Guid commandId = Guid.Parse(id);
        Models.Command? command = commands.FirstOrDefault(c => c.Id == commandId);

        return command == null ? null : TransformToCommand(command, robotId!);
    }

    public async ValueTask<Command?> GetLastCommandExecutedAsync(string robotId, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        using MongoDbContext dbContext = MongoDbContext.Create(database);

        Models.Robot? robot = await dbContext.Robots
            .Where(r => r.Id == robotId)
            .FirstOrDefaultAsync(cancellationToken);

        Models.Command? command = robot?.Commands
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();

        var result = command == null ? null : TransformToCommand(command, robotId);

        return result;
    }

    public async Task SetReplacedByCommandAsync(Command command, Command replacedByCommand, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        using MongoDbContext dbContext = MongoDbContext.Create(database);

        Models.Robot robot = await dbContext.Robots
            .Where(r => r.Id == replacedByCommand.RobotId)
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException($"Robot with id {replacedByCommand.RobotId} not found.");

        Models.Command replacedByCommandDB = AddCommandToRobot(replacedByCommand, ref robot);

        Guid guid = Guid.Parse(command.Id);
        Models.Command commandDB = robot.Commands.Where(c => c.Id == guid).SingleOrDefault() ?? throw new InvalidOperationException($"Command with id {command.Id} not found.");

        commandDB.ReplacedByCommandId = replacedByCommandDB.Id;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private Models.Command AddCommandToRobot(Command model, ref Models.Robot robot)
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

        if (robot.Commands == null)
        {
            robot.Commands = new List<Models.Command>();
        }

        robot.Commands.Add(command);

        return command;
    }

    private static Command TransformToCommand(Models.Command command, string robotId)
    {
        return new Command
        {
            Id = command.Id.ToString(),
            RobotId = robotId,
            UserId = command.UserId,
            Action = command.Action,
            CreatedAt = command.CreatedAt,
            Direction = command.LogState.AfterExecution.Direction,
            PositionX = command.LogState.AfterExecution.PositionX,
            PositionY = command.LogState.AfterExecution.PositionY,
            ReplacedByCommandId = command.ReplacedByCommandId?.ToString(),
        };
    }
}
