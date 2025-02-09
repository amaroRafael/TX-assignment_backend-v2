namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;
    using TX.RMC.DataAccess.Core.Enumerators;

    public class Command : IModel
    {
        public object Id { get; set; }

        public ECommands Action { get; set; }

        public object RobotId { get; set; }

        public object UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public EDirections Direction { get; set; }

        public object? ReplacedByCommandId { get; set; }
    }
}
