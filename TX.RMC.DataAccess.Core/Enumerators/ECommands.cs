namespace TX.RMC.DataAccess.Core.Enumerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Enumerates the possible commands that the robot can receive.
    /// </summary>
    public enum ECommands
    {
        /// <summary>
        /// Stop the robot.
        /// </summary>
        Stop,
        /// <summary>
        /// Move the robot forward.
        /// </summary>
        MoveForward,
        /// <summary>
        /// Move the robot backward.
        /// </summary>
        MoveBackward,
        /// <summary>
        /// Rotate the robot to the left.
        /// </summary>
        RotateLeft,
        /// <summary>
        /// Rotate the robot to the right.
        /// </summary>
        RotateRight,
    }
}