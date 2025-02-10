namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;

public class RobotService(IRobotDataRepository robotDataRepository, ICommandDataRepository commandDataRepository)
{
    private readonly IRobotDataRepository robotDataRepository = robotDataRepository;
    private readonly ICommandDataRepository commandDataRepository = commandDataRepository;

    /// <summary>
    /// Gets the robot current status.
    /// </summary>
    /// <param name="robot">The Robot name identity.</param>
    /// <returns>Current status of the robot.</returns>
    public async ValueTask<string> GetStatusAsync(string robot, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        /// Get robot from database.
        Robot? robotModel = await this.robotDataRepository.GetByNameIdentityAsync(robot, cancellationToken);

        /// If robot not found return message.
        if (robotModel is null) return "Robot not found.";

        /// Get last command executed by robot.
        Command? command = await this.commandDataRepository.GetLastCommandExecutedAsync(robotModel!.Id, cancellationToken);

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
    public async ValueTask<IEnumerable<(object Id, string Command, DateTime ExecutedAt)>> GetCommandHistoryAsync(string robot, int count = 10, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        /// Get robot from database.
        Robot? robotModel = await this.robotDataRepository.GetByNameIdentityAsync(robot, cancellationToken);

        /// If robot not found return empty list.
        if (robotModel is Robot robotFound)
        {
            /// Get last commands executed by robot.
            IEnumerable<Command> commands = await this.commandDataRepository.GetAllByRobotAsync(robotFound.Id, count, cancellationToken);

            IList<(object Id, string Command, DateTime ExecutedAt)> commandList = [];
            IList<object> replaceCommandIds = [];

            /// Iterate over commands and get the command text.
            foreach (Command command in commands.OrderByDescending(o => o.CreatedAt))
            {
                if (replaceCommandIds.Contains(command.Id)) continue;

                string commandText = command.Action.ToString();

                if (command.ReplacedByCommandId is not null)
                {
                    Command? replacedCommand = commands.FirstOrDefault(f => f.Id == command.ReplacedByCommandId);

                    replacedCommand ??= await this.commandDataRepository.GetByIdAsync(command.ReplacedByCommandId, cancellationToken);

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
    public async Task<Robot?> GetAsync(object id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        /// Get robot from database.
        Robot? robot = await this.robotDataRepository.GetByIdAsync(id, cancellationToken);
        return robot;
    }

    public async ValueTask<Robot> AddAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        return await this.robotDataRepository.AddAsync(new Robot { NameIdentity = name }, cancellationToken);
    }
}
