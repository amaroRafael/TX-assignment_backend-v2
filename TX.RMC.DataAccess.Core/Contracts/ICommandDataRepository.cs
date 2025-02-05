namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface ICommandDataRepository : IDataRepository<Command>
    {
        ValueTask<IEnumerable<Command>> GetAllByRobotAsync(Guid id, int count);
        ValueTask<Command?> GetLastCommandExecutedAsync(string robot);
    }
}
