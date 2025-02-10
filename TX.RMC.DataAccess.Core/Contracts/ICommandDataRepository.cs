namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface ICommandDataRepository : IDataRepository<Command>
    {
        ValueTask<IEnumerable<Command>> GetAllByRobotAsync(object robotId, int count, CancellationToken cancellationToken = default);
        ValueTask<Command?> GetLastCommandExecutedAsync(object robotId, CancellationToken cancellationToken = default);
        Task SetReplacedByCommandIdAsync(object id, object replacedByCommandId, CancellationToken cancellationToken = default);
    }
}
