namespace TX.RMC.UnitTests.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Models;

internal class CommandDataRepository : DataRepository<Command>, ICommandDataRepository
{
    public ValueTask<Command> AddAsync(Command model)
    {
        var newCommand = Add(model);

        return ValueTask.FromResult(newCommand);
    }

    public ValueTask<IEnumerable<Command>> GetAllByRobotAsync(object id, int count)
    {
        var commands = _dataTable.AsEnumerable()
            .Where(row => row.Field<object>("RobotId") == id)
            .Select(row => row)
            .OrderByDescending(row => row.Field<DateTime>("CreatedAt"))
            .Take(count);

        IList<Command> commandList = [];

        if (commands?.Count() > 0)
        {
            foreach (var command in commands)
            {
                Command newCommand = new();

                PopulateModel(newCommand, command);

                commandList.Add(newCommand);
            }
        }

        return ValueTask.FromResult<IEnumerable<Command>>(commandList);
    }

    public ValueTask<Command?> GetByIdAsync(object id)
    {
        return ValueTask.FromResult(GetById(id));
    }

    public ValueTask<Command?> GetLastCommandExecutedAsync(object robotId)
    {
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<object>("RobotId") == robotId
                       select rows)
                       .OrderByDescending(row => row.Field<DateTime>("CreatedAt"))
                       .FirstOrDefault();

        if (dataRow != null)
        {
            Command command = new();
            PopulateModel(command, dataRow);
            return ValueTask.FromResult<Command?>(command);
        }

        return ValueTask.FromResult<Command?>(null);
    }

    public Task SetReplacedByCommandIdAsync(object id, object replacedByCommandId)
    {
        // Gets the DataRow from the DataTable
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<object>("Id") == id
                       select rows)
                       .SingleOrDefault();

        // If DataRow was found, populate the DataRow with the data from the model
        if (dataRow != null)
        {
            // Set the value in the DataRow
            dataRow["ReplacedByCommandId"] = replacedByCommandId;

            // Accept the changes
            this._dataTable.AcceptChanges();
        }

        return Task.CompletedTask;
    }
}
