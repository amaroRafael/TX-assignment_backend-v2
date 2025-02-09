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

internal class UserDataRepository(MongoDBOptions mongoDBOptions) : IUserDataRepository
{
    private const string collectionName = "users";
    private readonly MongoDBOptions mongoDBOptions = mongoDBOptions;

    public async ValueTask<User> AddAsync(User model)
    {
        Models.User userDb = TransformToUserDb(model);

        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.User> collection = database.GetCollection<Models.User>(collectionName);

        await collection.InsertOneAsync(userDb);
        
        // Update the model with the new Id
        model.Id = userDb.Id;

        return model;
    }

    public async ValueTask<User?> GetByIdAsync(object id)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.User> collection = database.GetCollection<Models.User>(collectionName);

        var userDb = await collection.Find(u => u.Id == id.ToString()).SingleOrDefaultAsync();

        return userDb is null ? null : TransformToUser(userDb);
    }

    public async ValueTask<User?> GetByUsernameAsync(string username)
    {
        using MongoClient client = new MongoClient(this.mongoDBOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(this.mongoDBOptions.DatabaseName);
        IMongoCollection<Models.User> collection = database.GetCollection<Models.User>(collectionName);

        var userDb = await collection.Find(u => u.Username == username).SingleOrDefaultAsync();

        return userDb is null ? null : TransformToUser(userDb);
    }

    private static User TransformToUser(Models.User userDb)
    {
        return new User
        {
            Id = userDb.Id,
            Name = userDb.Name,
            Username = userDb.Username,
            Secret = userDb.Secret,
            Salt = userDb.Salt,
        };
    }

    private static Models.User TransformToUserDb(User model)
    {
        return new Models.User
        {
            Id = model.Id?.ToString() ?? null!,
            Name = model.Name,
            Username = model.Username,
            Secret = model.Secret,
            Salt = model.Salt,
        };
    }
}
