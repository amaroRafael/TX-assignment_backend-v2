namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface IUserDataRepository : IDataRepository<User>
    {
        ValueTask<User?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        ValueTask<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    }
}
