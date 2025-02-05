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
        IRobotDataRepository robotRepository = scope.ServiceProvider.GetRequiredService<IRobotDataRepository>();

        Robot? robotModel = robotRepository.GetByNameIdentityAsync(robot);

        if (robotModel is null) return "Robot not found.";

        ICommandDataRepository commandRepository = scope.ServiceProvider.GetRequiredService<ICommandDataRepository>();
        Command? command = await commandRepository.GetLastCommandExecutedAsync(robot);
        return (command?.Action ?? ECommands.None) switch
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
        IRobotDataRepository robotRepository = scope.ServiceProvider.GetRequiredService<IRobotDataRepository>();

        Robot? robotModel = robotRepository.GetByNameIdentityAsync(robot);

        if (robotModel is Robot robotFound)
        {
            ICommandDataRepository commandRepository = scope.ServiceProvider.GetRequiredService<ICommandDataRepository>();
            IEnumerable<Command> commands = await commandRepository.GetAllByRobotAsync(robotFound.Id, count);

            IList<(Guid Id, string Command, DateTime ExecutedAt)> commandList = [];

            string commandText = string.Empty;
            foreach (Command command in commands.Reverse())
            {
                if (string.IsNullOrEmpty(commandText))
                {
                    commandText = command.Action.ToString();

                    if (command.ReplacedByCommandId.HasValue)
                    {
                        continue;
                    }

                    commandList.Add((command.Id, commandText, command.CreatedAt));
                }
                else
                {
                    commandText += $" replaced by {command.Action}";

                    commandList.Add((command.Id, commandText, command.CreatedAt));

                    if (command.ReplacedByCommandId.HasValue)
                    {
                        commandText = command.Action.ToString();
                        continue;
                    }
                }

                commandText = string.Empty;
            }

            return commandList;
        }

        return [];
    }
}
