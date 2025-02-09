namespace TX.RMC.DataService.MongoDB.Models;

using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Enumerators;

public class Command
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("action")]
    public ECommands Action { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("position_x")]
    public int PositionX { get; set; }

    [BsonElement("position_y")]
    public int PositionY { get; set; }

    [BsonElement("direction")]
    public EDirections Direction { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = null!;
    [BsonIgnore]
    public virtual User User { get; set; } = null!;

    [BsonElement("robot_id")]
    public string RobotId { get; set; } = null!;
    [BsonIgnore]
    public virtual Robot Robot { get; set; } = null!;

    [BsonElement("replaced_by_command_id")]
    public string? ReplacedByCommandId { get; set; }
}
