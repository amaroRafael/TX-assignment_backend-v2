namespace TX.RMC.UnitTests.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;

internal class UserDataRepository : DataRepository<User>, IUserDataRepository
{
    public ValueTask<User> AddAsync(User model)
    {
        var newUser = this.Add(model);

        return ValueTask.FromResult(newUser);
    }

    public ValueTask<User?> GetByIdAsync(Guid id)
    {
        return ValueTask.FromResult(GetById(id));
    }

    public ValueTask<User?> GetByUsernameAsync(string username)
    {
        DataRow? dataRow = (from rows in this._dataTable.AsEnumerable()
                            where rows.Field<string>("Username") == username
                            select rows)
                            .FirstOrDefault();

        if (dataRow != null)
        {
            User user = new();
            PopulateModel(user, dataRow);

            return ValueTask.FromResult<User?>(user);
        }

        return ValueTask.FromResult<User?>(null);
    }
}
