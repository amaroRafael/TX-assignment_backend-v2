namespace TX.RMC.DataService.MongoDB.Repositories;

using global::MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.DataService.MongoDB.Options;

internal class RobotDataRepository(MongoDBOptions mongoDBOptions) : IRobotDataRepository
{
    private const string collectionName = "robots";
    private readonly MongoDBOptions mongoDBOptions = mongoDBOptions;

    public async ValueTask<Robot> AddAsync(Robot model, CancellationToken cancellationToken = default)
    {
        Models.Robot robot = TransformToRobotDb(model);

        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Robot> collection = database.GetCollection<Models.Robot>(collectionName);

        await collection.InsertOneAsync(robot, null, cancellationToken);

        return TransformToRobot(robot);
    }

    public async ValueTask<Robot?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Robot> collection = database.GetCollection<Models.Robot>(collectionName);

        var robotDb = await collection.Find(r => r.Id == id).SingleOrDefaultAsync(cancellationToken);

        return robotDb is null ? null : TransformToRobot(robotDb);
    }

    public async ValueTask<Robot?> GetByNameIdentityAsync(string nameIdentity, CancellationToken cancellationToken = default)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.Robot> collection = database.GetCollection<Models.Robot>(collectionName);

        Models.Robot? robotDb = await collection.Find(r => r.Name == nameIdentity).SingleOrDefaultAsync(cancellationToken);

        return robotDb is null ? null : TransformToRobot(robotDb);
    }

    private static Robot TransformToRobot(Models.Robot robot)
    {
        return new Robot
        {
            Id = robot.Id,
            NameIdentity = robot.Name,
        };
    }

    private static Models.Robot TransformToRobotDb(Robot model)
    {
        return new Models.Robot
        {
            Id = model.Id?.ToString() ?? null!,
            Name = model.NameIdentity,
        };
    }
}
