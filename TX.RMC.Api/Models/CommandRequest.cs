namespace TX.RMC.Api.Models;

using TX.RMC.BusinessLogic.Enumerators;

public class CommandRequest
{
    public EComands Command { get; set; }
    public required string Robot { get; set; }
}
