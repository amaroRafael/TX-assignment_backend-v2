namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface IRobotDataRepository : IDataRepository<Robot>
    {
        ValueTask<Robot?> GetByNameIdentityAsync(string robot);
    }
}
