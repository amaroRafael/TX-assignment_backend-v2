namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;

public class CommandService(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;

    /// <summary>
    /// Retrieves command from database.
    /// </summary>
    /// <param name="id">Command identity.</param>
    /// <returns>Returns the command.</returns>
    public async Task<Command> GetAsync(Guid id)
    {
        using var scope = this.scopeFactory.CreateAsyncScope();
        ICommandDataRepository dataRepository = scope.ServiceProvider.GetRequiredService<ICommandDataRepository>();
        
        Command command = await dataRepository.GetByIdAsync(id);
        return command;
    }

    /// <summary>
    /// Sends the command to the robot and add it to the database.
    /// The new positions and/or direction will be updated based on the last command executed.
    /// </summary>
    /// <param name="command">Enum ECommands. Accepted values include: Stop, MoveForward, MoveBackward, RotateLeft, RotateRight,</param>
    /// <param name="robot">The robot name identity.</param>
    /// <param name="userId">User identity.</param>
    /// <returns>Return the command data executed.</returns>
    /// <exception cref="ArgumentNullException">Robot and/or User is required.</exception>
    public async Task<Command?> SendAsync(ECommands command, string robot, Guid? userId)
    {
        if (!userId.HasValue) throw new ArgumentNullException(nameof(userId), "User is required.");
        if (string.IsNullOrWhiteSpace(robot)) throw new ArgumentNullException(nameof(robot), "Robot is required.");

        using var scope = this.scopeFactory.CreateAsyncScope();
        IRobotDataRepository robotDataRepository = scope.ServiceProvider.GetRequiredService<IRobotDataRepository>();

        Robot? robotModel = robotDataRepository.GetByNameIdentityAsync(robot);

        if (robotModel is Robot robotFound)
        {
            ICommandDataRepository dataRepository = scope.ServiceProvider.GetRequiredService<ICommandDataRepository>();

            Command? lastCommand = await dataRepository.GetLastCommandExecutedAsync(robot);

            var posX = lastCommand?.PositionX ?? 0;
            var posY = lastCommand?.PositionY ?? 0;
            var direction = lastCommand?.Direction ?? EDirections.North;

            switch (command)
            {
                case ECommands.MoveForward:
                    UpdatePositions(direction, 1, ref posX, ref posY);
                    break;
                case ECommands.MoveBackward:
                    UpdatePositions(direction, -1, ref posX, ref posY);
                    break;
                case ECommands.RotateLeft:
                    UpdateDirection(false, ref direction);
                    break;
                case ECommands.RotateRight:
                    UpdateDirection(true, ref direction);
                    break;
                case ECommands.Stop:
                default:
                    break;
            }

            Command commandModel = new()
            {
                Action = command,
                CreatedAt = DateTime.UtcNow,
                RobotId = robotFound.Id,
                UserId = userId!.Value,
                Direction = direction,
                PositionX = posX,
                PositionY = posY,
            };

            return dataRepository.AddAsync(commandModel);
        }

        return null;

        static void UpdatePositions(EDirections direction, int step, ref int posX, ref int posY)
        {
            switch (direction)
            {
                case EDirections.North:
                    posY += step;
                    break;
                case EDirections.Northeast:
                    posX += step;
                    posY += step;
                    break;
                case EDirections.East:
                    posX += step;
                    break;
                case EDirections.Southeast:
                    posX += step;
                    posY -= step;
                    break;
                case EDirections.South:
                    posY -= step;
                    break;
                case EDirections.Southwest:
                    posX -= step;
                    posY -= step;
                    break;
                case EDirections.West:
                    posX -= step;
                    break;
                case EDirections.Northwest:
                    posX -= step;
                    posY += step;
                    break;
            }
        }

        static void UpdateDirection(bool clockwise, ref EDirections direction)
        {
            var values = Enum.GetValues<EDirections>();
            int min = values.Select(s => (int)s).Min();
            int max = values.Select(s => (int)s).Max();

            // the first item has value 0 and the max value 360 is not inclued sinc eit comes back to 0.
            // That is the reason it was not included to calculate the space between items.
            int degree = max / (values.Length - 1); 

            if (!clockwise) degree += -1;

            var newDirection = (int)direction + degree;
            if (newDirection < min) newDirection = max;
            if (newDirection > max) newDirection = min;

            direction = (EDirections)newDirection;
        }
    }

    public async Task<bool> UpdateAsync(ECommands command, string robot)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }
}
