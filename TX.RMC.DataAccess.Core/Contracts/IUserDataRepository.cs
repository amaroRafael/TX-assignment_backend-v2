namespace TX.RMC.DataAccess.Core.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TX.RMC.DataAccess.Core.Models;

    public interface IUserDataRepository : IDataRepository<User>
    {
        ValueTask<User> AddAsync(User user);
        ValueTask<User?> GetByUsernameAsync(string username);
    }
}
