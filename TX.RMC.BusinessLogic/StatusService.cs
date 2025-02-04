namespace TX.RMC.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StatusService
{
    public Task<string> GetStatusAsync(string robot)
    {
        // TODO: Implement this method
        return Task.FromResult("Moving forward");
    }
}
