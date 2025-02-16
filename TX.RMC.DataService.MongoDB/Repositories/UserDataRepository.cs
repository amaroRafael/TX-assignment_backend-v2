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

public class UserDataRepository(MongoDBContext dbContext) : IUserDataRepository
{
    private readonly MongoDBContext dbContext = dbContext;

    public async ValueTask<User> AddAsync(User model, CancellationToken cancellationToken = default)
    {
        Models.User userDb = TransformToUserDb(model);

        await dbContext.Users.AddAsync(userDb, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Update the model with the new Id
        model.Id = userDb.Id;

        return model;
    }

    public async ValueTask<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var userDb = await dbContext.Users.Where(u => u.Id == id).SingleOrDefaultAsync(cancellationToken);

        return userDb is null ? null : TransformToUser(userDb);
    }

    public async ValueTask<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var userDb = await dbContext.Users
            .Where(u => u.Username == username)
            .SingleOrDefaultAsync(cancellationToken);

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
            Id = model.Id,
            Name = model.Name,
            Username = model.Username,
            Secret = model.Secret,
            Salt = model.Salt,
        };
    }
}
