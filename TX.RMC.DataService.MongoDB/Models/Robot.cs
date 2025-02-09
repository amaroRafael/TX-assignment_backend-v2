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

public class Robot
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name_identity")]
    public string NameIdentity { get; set; } = null!;

    [BsonIgnore]
    public virtual List<Command> Commands { get; set; } = null!;
}
