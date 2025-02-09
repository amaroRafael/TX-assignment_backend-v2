namespace TX.RMC.DataAccess.Core.Enumerators
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Enumerates the possible directions that the robot can face.
    /// </summary>
    public enum EDirections
    {
        /// <summary>
        /// The robot is facing north.
        /// </summary>
        North = 0,
        /// <summary>
        /// The robot is facing northeast.
        /// </summary>
        Northeast = 45,
        /// <summary>
        /// The robot is facing east.
        /// </summary>
        East = 90,
        /// <summary>
        /// The robot is facing southeast.
        /// </summary>
        Southeast = 135,
        /// <summary>
        /// The robot is facing south.
        /// </summary>
        South = 180,
        /// <summary>
        /// The robot is facing southwest.
        /// </summary>
        Southwest = 225,
        /// <summary>
        /// The robot is facing west.
        /// </summary>
        West = 270,
        /// <summary>
        /// The robot is facing northwest.
        /// </summary>
        Northwest = 315
    }
}
