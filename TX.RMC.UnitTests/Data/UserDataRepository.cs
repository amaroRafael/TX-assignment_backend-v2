namespace TX.RMC.UnitTests.Data;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.DataService.MongoDB;

internal class UserDataRepository : DataService.MongoDB.Repositories.UserDataRepository
{
    public static UserDataRepository Create()
    {
        var options = new DbContextOptionsBuilder<MongoDBContext>()
            .UseInMemoryDatabase(databaseName: "MockDatabase")
            .Options;

        return new UserDataRepository(new MongoDBContext(options));
    }

    private UserDataRepository(MongoDBContext dbContext)
        : base(dbContext)
    {
        
    }
}
