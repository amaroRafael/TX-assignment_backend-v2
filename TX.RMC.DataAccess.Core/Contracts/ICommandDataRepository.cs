namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface ICommandDataRepository : IDataRepository<Command>
    {
        Command? AddAsync(Command commandModel);
        ValueTask<IEnumerable<Command>> GetAllByRobotAsync(Guid id, int count);
        ValueTask<Command?> GetLastCommandExecutedAsync(string robot);
    }
}
