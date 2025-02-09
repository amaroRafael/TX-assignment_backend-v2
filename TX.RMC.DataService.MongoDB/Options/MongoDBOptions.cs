namespace TX.RMC.DataService.MongoDB.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class MongoDBOptions
{
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
}
