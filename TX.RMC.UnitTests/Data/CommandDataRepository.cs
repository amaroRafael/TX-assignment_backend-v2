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

    public ValueTask<IEnumerable<Command>> GetAllByRobotAsync(Guid id, int count)
    {
        var commands = _dataTable.AsEnumerable()
            .Where(row => row.Field<Guid>("RobotId") == id)
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

    public ValueTask<Command?> GetByIdAsync(Guid id)
    {
        return ValueTask.FromResult(GetById(id));
    }

    public ValueTask<Command?> GetLastCommandExecutedAsync(Guid robotId)
    {
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<Guid>("RobotId") == robotId
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

    public ValueTask<Command> UpdateAsync(Command command)
    {
        // Gets the DataRow from the DataTable
        var dataRow = (from rows in this._dataTable.AsEnumerable()
                       where rows.Field<Guid>("Id") == command.Id
                       select rows)
                       .SingleOrDefault();

        // If DataRow was found, populate the DataRow with the data from the model
        if (dataRow != null)
        {
            // Loop through the properties of the model and set the value in the DataRow
            foreach (var propertyInfo in typeof(Command).GetProperties())
            {
                // Set the value in the DataRow
                dataRow[propertyInfo.Name] = propertyInfo.GetValue(command);
            }

            // Accept the changes
            this._dataTable.AcceptChanges();
        }

        // Return the command
        return ValueTask.FromResult(command);
    }
}
