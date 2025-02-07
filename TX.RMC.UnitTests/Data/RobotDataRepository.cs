namespace TX.RMC.UnitTests.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;

internal class RobotDataRepository : DataRepository<Robot>, IRobotDataRepository
{
    public ValueTask<Robot> GetByIdAsync(Guid id)
    {
        return ValueTask.FromResult(GetById(id));
    }

    public ValueTask<Robot?> GetByNameIdentityAsync(string nameIdentity)
    {
        Robot robot = new();

        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<string>("NameIdentity") == nameIdentity
                       select rows)
                .SingleOrDefault();

        PopulateUserModel(robot, dataRow);

        return ValueTask.FromResult<Robot?>(robot);
    }
}
