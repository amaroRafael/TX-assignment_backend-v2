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
    public ValueTask<Robot> AddAsync(Robot robot, CancellationToken cancellationToken = default)
    {
        var newRobot = this.Add(robot);

        return ValueTask.FromResult(newRobot);
    }


    public ValueTask<Robot?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(GetById(id));
    }

    public ValueTask<Robot?> GetByNameIdentityAsync(string nameIdentity, CancellationToken cancellationToken = default)
    {
        Robot robot = new();

        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<string>("NameIdentity") == nameIdentity
                       select rows)
                .SingleOrDefault();

        PopulateModel(robot, dataRow);

        return ValueTask.FromResult<Robot?>(robot);
    }
}
