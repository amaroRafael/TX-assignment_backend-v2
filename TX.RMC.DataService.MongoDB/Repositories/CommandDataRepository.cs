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
    private const string collectionName = "commands";
    private readonly MongoDBOptions mongoDBOptions = mongoDBOptions;

    public async ValueTask<Command> AddAsync(Command model, CancellationToken cancellationToken = default)
    {
        Models.Command command = TransformToCommandDb(model);

        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Command> collection = database.GetCollection<Models.Command>(collectionName);

        await collection.InsertOneAsync(command,null, cancellationToken);

        return TransformToCommand(command);
    }

    public async ValueTask<IEnumerable<Command>> GetAllByRobotAsync(object robotId, int count, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Command> collection = database.GetCollection<Models.Command>(collectionName);

        var commands = await collection.AsQueryable()
            .Where(c => c.RobotId == robotId.ToString())
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

        return commands.Select(TransformToCommand);
    }

    public async ValueTask<Command?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Command> collection = database.GetCollection<Models.Command>(collectionName);

        Models.Command? command = await collection.Find(c => c.Id == id.ToString()).SingleOrDefaultAsync(cancellationToken);

        return command == null ? null : TransformToCommand(command);
    }

    public async ValueTask<Command?> GetLastCommandExecutedAsync(object robotId, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Command> collection = database.GetCollection<Models.Command>(collectionName);

        Models.Command? command = await collection.AsQueryable()
            .Where(c => c.RobotId == robotId.ToString())
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return command == null ? null : TransformToCommand(command);
    }

    public async Task SetReplacedByCommandIdAsync(object id, object replacedByCommandId, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Command> collection = database.GetCollection<Models.Command>(collectionName);

        await collection.UpdateOneAsync(c => c.Id == id.ToString(), Builders<Models.Command>.Update.Set(f => f.ReplacedByCommandId, replacedByCommandId), null, cancellationToken);
    }

    private static Command TransformToCommand(Models.Command command)
    {
        return new Command
        {
            Id = command.Id,
            RobotId = command.RobotId,
            UserId = command.UserId,
            Action = command.Action,
            CreatedAt = command.CreatedAt,
            Direction = command.Direction,
            PositionX = command.PositionX,
            PositionY = command.PositionY,
            ReplacedByCommandId = command.ReplacedByCommandId,
        };
    }

    private static Models.Command TransformToCommandDb(Command model)
    {
        return new Models.Command
        {
            Id = model.Id?.ToString() ?? null!,
            RobotId = model.RobotId.ToString()!,
            UserId = model.UserId.ToString()!,
            Action = model.Action,
            CreatedAt = model.CreatedAt,
            Direction = model.Direction,
            PositionX = model.PositionX,
            PositionY = model.PositionY,
            ReplacedByCommandId = model.ReplacedByCommandId?.ToString(),
        };
    }
}
