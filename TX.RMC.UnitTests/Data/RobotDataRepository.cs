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

internal class RobotDataRepository : DataService.MongoDB.Repositories.RobotDataRepository
{
    public static RobotDataRepository Create()
    {
        var options = new DbContextOptionsBuilder<MongoDBContext>()
            .UseInMemoryDatabase(databaseName: "MockDatabase")
            .Options;

        return new RobotDataRepository(new MongoDBContext(options));
    }

    private RobotDataRepository(MongoDBContext dbContext)
        : base(dbContext)
    {

    }
}
