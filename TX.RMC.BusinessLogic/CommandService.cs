namespace TX.RMC.BusinessLogic;

using System;
using System.Threading.Tasks;
using TX.RMC.BusinessLogic.Enumerators;

public class CommandService
{
    public async Task<object> GetAsync(Guid id)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }

    public async Task<bool> SendAsync(EComands command, string robot, Guid? v)
    {
        // TODO: Implement this method
        return true;
    }

    public async Task<bool> UpdateAsync(EComands command, string robot, Guid? v)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }
}
