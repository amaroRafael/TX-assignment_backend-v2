namespace TX.RMC.DataService.MongoDB.Models;

using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;
using global::MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Collection("robots")]
internal class Robot
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    [BsonElement("current_state")]
    public State CurrentState { get; set; } = null!;

    public List<Command> Commands { get; set; } = null!;
}
