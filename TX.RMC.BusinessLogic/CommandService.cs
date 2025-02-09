namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataAccess.Core.Enumerators;
using TX.RMC.DataAccess.Core.Models;

public class CommandService(ICommandDataRepository commandDataRepository, IRobotDataRepository robotDataRepository)
{
    private readonly ICommandDataRepository commandDataRepository = commandDataRepository;
    private readonly IRobotDataRepository robotDataRepository = robotDataRepository;
    private readonly static int stepsToMove = 1;

    /// <summary>
    /// Retrieves command from database.
    /// </summary>
    /// <param name="id">Command identity.</param>
    /// <returns>Returns the command.</returns>
    public async Task<Command?> GetAsync(object id)
    {
        /// The command will be retrieved.
        Command? command = await this.commandDataRepository.GetByIdAsync(id);
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
    public async Task<Command?> SendAsync(ECommands command, string robot, object userId)
    {
        /// The robot and user are required.
        if (userId is null) throw new ArgumentNullException(nameof(userId), "User is required.");
        if (string.IsNullOrWhiteSpace(robot)) throw new ArgumentNullException(nameof(robot), "Robot is required.");

        /// The robot will be retrieved.
        Robot? robotModel = await this.robotDataRepository.GetByNameIdentityAsync(robot);

        /// The robot must be found.
        if (robotModel is Robot robotFound)
        {
            /// The last command executed will be retrieved.
            Command? lastCommand = await this.commandDataRepository.GetLastCommandExecutedAsync(robotFound.Id);

            /// The last command executed will be used to update the new positions and direction.
            var posX = lastCommand?.PositionX ?? 0;
            var posY = lastCommand?.PositionY ?? 0;
            var direction = lastCommand?.Direction ?? EDirections.North;

            UpdatePositionAndDirection(command, ref direction, ref posX, ref posY);

            /// The new command will be executed.
            Command commandModel = new()
            {
                Action = command,
                CreatedAt = DateTime.UtcNow,
                RobotId = robotFound.Id,
                UserId = userId,
                Direction = direction,
                PositionX = posX,
                PositionY = posY,
            };

            /// The new command executed will be added to the database.
            return await this.commandDataRepository.AddAsync(commandModel);
        }

        return null;
    }


    /// <summary>
    /// Updates the last command executed and add the new command to the database.
    /// </summary>
    /// <param name="command">Enum ECommands. Accepted values include: Stop, MoveForward, MoveBackward, RotateLeft, RotateRight,</param>
    /// <param name="robot">The robot name identity.</param>
    /// <param name="userId">User identity.</param>
    /// <returns>Return the command data executed.</returns>
    /// <exception cref="ArgumentNullException">Robot and/or User is required.</exception>
    public async Task<Command?> UpdateAsync(ECommands command, string robot, object userId)
    {
        /// The user and robot are required.
        if (userId is null) throw new ArgumentNullException(nameof(userId), "User is required.");
        if (string.IsNullOrWhiteSpace(robot)) throw new ArgumentNullException(nameof(robot), "Robot is required.");

        /// The robot will be retrieved.
        Robot? robotModel = await this.robotDataRepository.GetByNameIdentityAsync(robot);

        /// The robot must be found.
        if (robotModel is Robot robotFound)
        {
            /// The last command executed will be retrieved.
            Command? lastCommand = await this.commandDataRepository.GetLastCommandExecutedAsync(robotFound.Id);

            if (lastCommand is Command commandFound)
            {
                /// The last command executed will be used to update the new positions and direction.
                var posX = commandFound.PositionX;
                var posY = commandFound.PositionY;
                var direction = commandFound.Direction;

                /// The last command executed will be reversed.
                switch (lastCommand.Action)
                {
                    case ECommands.MoveForward:
                        UpdatePositionAndDirection(ECommands.MoveBackward, ref direction, ref posX, ref posY);
                        break;
                    case ECommands.MoveBackward:
                        UpdatePositionAndDirection(ECommands.MoveForward, ref direction, ref posX, ref posY);
                        break;
                    case ECommands.RotateLeft:
                        UpdatePositionAndDirection(ECommands.RotateRight, ref direction, ref posX, ref posY);
                        break;
                    case ECommands.RotateRight:
                        UpdatePositionAndDirection(ECommands.RotateLeft, ref direction, ref posX, ref posY);
                        break;
                }

                /// The new command will be executed.
                UpdatePositionAndDirection(command, ref direction, ref posX, ref posY);

                Command commandModel = new()
                {
                    Action = command,
                    CreatedAt = DateTime.UtcNow,
                    RobotId = robotFound.Id,
                    UserId = userId,
                    Direction = direction,
                    PositionX = posX,
                    PositionY = posY,
                };

                /// The new command executed will be added to the database.
                Command newCommand = await this.commandDataRepository.AddAsync(commandModel);

                /// The last command executed will be updated with the new command executed.
                lastCommand.ReplacedByCommandId = newCommand.Id;
                await this.commandDataRepository.SetReplacedByCommandIdAsync(lastCommand.Id, newCommand.Id);

                return newCommand;
            }
        }

        return null;
    }


    /// <summary>
    /// Updates the position and direction
    /// </summary>
    /// <param name="command">Enum ECommands. Accepted values include: Stop, MoveForward, MoveBackward, RotateLeft, RotateRight,</param>
    /// <param name="direction">
    /// The direction of the robot.
    /// Enum EDirections.
    /// Accepetd values include:
    ///     North = 0, Northeast = 45,
    ///     East = 90, Southeast = 135,
    ///     South = 180, Southwest = 225,
    ///     West = 270, Northwest = 315</param>
    /// <param name="posX">The position of the robot at the X axis</param>
    /// <param name="posY">The position of the robot at the Y axis</param>
    private static void UpdatePositionAndDirection(ECommands command, ref EDirections direction, ref int posX, ref int posY)
    {
        switch (command)
        {
            case ECommands.MoveForward:
                /// The new position will be updated based on the old position.
                UpdatePositions(direction, stepsToMove, ref posX, ref posY);
                break;
            case ECommands.MoveBackward:
                /// The new position will be updated based on the old position.
                UpdatePositions(direction, -stepsToMove, ref posX, ref posY);
                break;
            case ECommands.RotateLeft:
                /// The new direction will be updated based on the old direction.
                UpdateDirection(false, ref direction);
                break;
            case ECommands.RotateRight:
                /// The new direction will be updated based on the old direction.
                UpdateDirection(true, ref direction);
                break;
            case ECommands.Stop:
            default:
                break;
        }

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
            /// Gets all the values of the enum EDirections.
            var values = Enum.GetValues<EDirections>();
            /// Gets the minimum and maximum value of the enum EDirections.
            int min = values.Select(s => (int)s).Min();
            int max = values.Select(s => (int)s).Max();

            // the first item has value 0 and the max value 360 is not inclued sinc eit comes back to 0.
            // That is the reason it was not included to calculate the space between items.
            int degree = max / (values.Length - 1);

            /// If clockwise is true, the degree will be added (rotate right), otherwise it will be subtracted (rotate left).
            if (!clockwise) degree *= -1;

            /// The new direction will be updated.
            var newDirection = (int)direction + degree;
            /// If the new direction is less than the minimum value, the new direction will be the maximum value.
            if (newDirection < min) newDirection = max;
            /// If the new direction is greater than the maximum value, the new direction will be the minimum value.
            if (newDirection > max) newDirection = min;

            /// Update the direction.
            direction = (EDirections)newDirection;
        }
    }
}
