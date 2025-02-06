﻿namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;

public class RobotService(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;

    /// <summary>
    /// Gets the robot current status.
    /// </summary>
    /// <param name="robot">The Robot name identity.</param>
    /// <returns>Current status of the robot.</returns>
    public async ValueTask<string> GetStatusAsync(string robot)
    {
        using var scope = scopeFactory.CreateAsyncScope();
        IRobotDataRepository robotDataRepository = scope.ServiceProvider.GetRequiredService<IRobotDataRepository>();

        /// Get robot from database.
        Robot? robotModel = await robotDataRepository.GetByNameIdentityAsync(robot);

        /// If robot not found return message.
        if (robotModel is null) return "Robot not found.";

        ICommandDataRepository commandDataRepository = scope.ServiceProvider.GetRequiredService<ICommandDataRepository>();

        /// Get last command executed by robot.
        Command? command = await commandDataRepository.GetLastCommandExecutedAsync(robot);

        /// If no command executed return stopped.
        /// Otherwise return the command executed
        return (command?.Action ?? ECommands.Stop) switch
        {
            ECommands.MoveForward => "Moved forward",
            ECommands.MoveBackward => "Moved backward",
            ECommands.RotateLeft => "Rotated left",
            ECommands.RotateRight => "Rotated right",
            _ => "Stopped"
        };
    }

    /// <summary>
    /// Gets robot command history executed.
    /// </summary>
    /// <param name="robot">The robot name identity.</param>
    /// <param name="count">Number of history items to be retrieved. Deefault 10</param>
    /// <returns>Command history</returns>
    public async ValueTask<IEnumerable<(Guid Id, string Command, DateTime ExecutedAt)>> GetCommandHistoryAsync(string robot, int count = 10)
    {
        using var scope = scopeFactory.CreateAsyncScope();
        IRobotDataRepository robotDataRepository = scope.ServiceProvider.GetRequiredService<IRobotDataRepository>();

        /// Get robot from database.
        Robot? robotModel = await robotDataRepository.GetByNameIdentityAsync(robot);

        /// If robot not found return empty list.
        if (robotModel is Robot robotFound)
        {
            ICommandDataRepository commandDataRepository = scope.ServiceProvider.GetRequiredService<ICommandDataRepository>();
            /// Get last commands executed by robot.
            IEnumerable<Command> commands = await commandDataRepository.GetAllByRobotAsync(robotFound.Id, count);

            IList<(Guid Id, string Command, DateTime ExecutedAt)> commandList = [];
            IList<Guid> replaceCommandIds = [];

            /// Iterate over commands and get the command text.
            foreach (Command command in commands.OrderByDescending(o => o.CreatedAt))
            {
                if (replaceCommandIds.Contains(command.Id)) continue;

                string commandText = command.Action.ToString();

                if (command.ReplacedByCommandId.HasValue)
                {
                    Command? replacedCommand = commands.FirstOrDefault(f => f.Id == command.ReplacedByCommandId.Value);

                    replacedCommand ??= await commandDataRepository.GetByIdAsync(command.ReplacedByCommandId.Value);

                    if (replacedCommand is not null)
                    {
                        commandText += $" replaced by {command.Action}";
                        replaceCommandIds.Add(replacedCommand.Id);
                    }
                }

                commandList.Add((command.Id, commandText, command.CreatedAt));
            }

            return commandList;
        }

        return [];
    }

    /// <summary>
    /// Retrieves robot from database.
    /// </summary>
    /// <param name="id">Robot identity.</param>
    /// <returns>Returns the robot.</returns>
    public async Task<Robot> GetAsync(Guid id)
    {
        using var scope = this.scopeFactory.CreateAsyncScope();
        IRobotDataRepository robotDataRepository = scope.ServiceProvider.GetRequiredService<IRobotDataRepository>();

        /// Get robot from database.
        Robot robot = await robotDataRepository.GetByIdAsync(id);
        return robot;
    }
}
