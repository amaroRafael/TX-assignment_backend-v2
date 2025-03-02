namespace TX.RMC.UnitTests.Data;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;
using TX.RMC.DataService.MongoDB;

internal class CommandDataRepository : DataService.MongoDB.Repositories.CommandDataRepository
{
    public static CommandDataRepository Create()
    {
        var options = new DbContextOptionsBuilder<MongoDBContext>()
            .UseInMemoryDatabase(databaseName: "MockDatabase")
            .Options;

        return new CommandDataRepository(new MongoDBContext(options));
    }

    private CommandDataRepository(MongoDBContext dbContext)
        : base(dbContext)
    {

    }
}
