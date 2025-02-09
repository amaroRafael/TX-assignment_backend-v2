namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface ICommandDataRepository : IDataRepository<Command>
    {
        ValueTask<IEnumerable<Command>> GetAllByRobotAsync(object robotId, int count);
        ValueTask<Command?> GetLastCommandExecutedAsync(object robotId);
        Task SetReplacedByCommandIdAsync(object id, object replacedByCommandId);
    }
}
