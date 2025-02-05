namespace TX.RMC.BusinessLogic;

using System;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Enumerators;

public class CommandService
{
    public async Task<object> GetAsync(Guid id)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }

    public async Task<bool> SendAsync(ECommands command, string robot, Guid? v)
    {
        // TODO: Implement this method
        return true;
    }

    public async Task<bool> UpdateAsync(ECommands command, string robot, Guid? v)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }
}
