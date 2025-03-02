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
        ValueTask<Command?> GetByIdAsync(string robotId, string id, CancellationToken cancellationToken = default);
        ValueTask<IEnumerable<Command>> GetAllByRobotAsync(string robotId, int count, CancellationToken cancellationToken = default);
        ValueTask<Command?> GetLastCommandExecutedAsync(string robotId, CancellationToken cancellationToken = default);
        ValueTask<Command> SetReplacedByCommandAsync(Command command, Command replacedByCommand, CancellationToken cancellationToken = default);
    }
}
