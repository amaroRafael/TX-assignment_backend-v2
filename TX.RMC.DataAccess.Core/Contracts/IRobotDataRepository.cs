namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Models;

    public interface IRobotDataRepository : IDataRepository<Robot>
    {
        Robot? GetByNameIdentityAsync(string robot);
    }
}
