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
    public ValueTask<Command> AddAsync(Command model, CancellationToken cancellationToken = default)
    {
        var newCommand = Add(model);

        return ValueTask.FromResult(newCommand);
    }

    public ValueTask<IEnumerable<Command>> GetAllByRobotAsync(string id, int count, CancellationToken cancellationToken = default)
    {
        var commands = _dataTable.AsEnumerable()
            .Where(row => row.Field<string>("RobotId") == id)
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

    public ValueTask<Command?> GetByIdAsync(string robotId, string id, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(GetById(id));
    }

    public ValueTask<Command?> GetLastCommandExecutedAsync(string robotId, CancellationToken cancellationToken = default)
    {
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<string>("RobotId") == robotId
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

    public Task SetReplacedByCommandAsync(Command command, Command replacedByCommand, CancellationToken cancellationToken = default)
    {
        // Gets the DataRow from the DataTable
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<string>("Id") == command.Id
                       select rows)
                       .SingleOrDefault();

        // If DataRow was found, populate the DataRow with the data from the model
        if (dataRow != null)
        {
            // Set the value in the DataRow
            dataRow["ReplacedByCommandId"] = replacedByCommand.Id;

            // Accept the changes
            this._dataTable.AcceptChanges();
        }

        return Task.CompletedTask;
    }
}
