namespace TX.RMC.DataService.MongoDB.Models;

using global::MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Enumerators;

internal class State
{
    [BsonElement("position_x")]
    public int PositionX { get; set; }

    [BsonElement("position_y")]
    public int PositionY { get; set; }

    public EDirections Direction { get; set; }
}
