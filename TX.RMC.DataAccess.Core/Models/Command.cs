namespace TX.RMC.DataAccess.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TX.RMC.DataAccess.Core.Contracts;
    using TX.RMC.DataAccess.Core.Enumerators;

    public class Command : IModel
    {
        public string Id { get; set; } = null!;

        public ECommands Action { get; set; }

        public string RobotId { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public EDirections Direction { get; set; }

        public string? ReplacedByCommandId { get; set; }
    }
}
