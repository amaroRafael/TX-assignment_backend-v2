namespace TX.RMC.Api.Models;

using TX.RMC.BusinessLogic.Enumerators;

/// <summary>
/// Command request model
/// </summary>
public class CommandRequest
{
    /// <summary>
    /// Command to execute
    /// </summary>
    public EComands Command { get; set; }

    /// <summary>
    /// Robot name
    /// </summary>
    public required string Robot { get; set; }
}
