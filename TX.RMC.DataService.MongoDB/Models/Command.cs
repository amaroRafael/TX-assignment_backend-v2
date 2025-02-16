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
    public Guid Id { get; set; }

    public ECommands Action { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = null!;

    [BsonElement("replace_by_command")]
    public Guid? ReplacedByCommandId { get; set; }

    [BsonElement("log_state")]
    public LogStateData LogState { get; set; } = null!;

    public class LogStateData
    {
        public Guid Id { get; set; }
        public State BeforeExecution { get; set; } = null!; 
        public State AfterExecution { get; set; } = null!;
    }
}
