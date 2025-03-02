namespace TX.RMC.FunctionApp;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json.Serialization;

public abstract class ApiBase
{
    protected OkObjectResult Ok([ActionResultObjectValue] object? value)
    {
        return new OkObjectResult(CreateSuccessResponse(value));
    }

    protected CreatedResult Created(string? location, [ActionResultObjectValue] object? value)
    {
        return new CreatedResult(location, CreateSuccessResponse(value));
    }

    protected AcceptedResult AcceptedAtAction(string? location, [ActionResultObjectValue] object? value)
    {
        return new AcceptedResult(location, CreateSuccessResponse(value));
    }

    protected BadRequestObjectResult BadRequest([ActionResultObjectValue] ErrorResponse? value)
    {
        return new BadRequestObjectResult(value);
    }

    protected BadRequestObjectResult BadRequest<T>([ActionResultObjectValue] FailResponse<T>? value)
    {
        return new BadRequestObjectResult(value);
    }

    internal static ApiResponse<T> CreateSuccessResponse<T>(T? value)
    {
        return new ApiResponse<T> { Data = value };
    }

    internal static ErrorResponse CreateErrorResponse(string value)
    {
        return new ErrorResponse { Message = value };
    }

    internal static FailResponse<T> CreateFailResponse<T>(T? value)
    {
        return new FailResponse<T> { Data = value };
    }
}

public class ErrorResponse : ApiResponse<object>
{
    public override string Status { get; } = "error";
    public string Message { get; init; } = null!;
}

public class FailResponse<T> : ApiResponse<T>
{
    public override string Status { get; } = "fail";
}

public class ApiResponse<T>
{
    public virtual string Status { get; } = "success";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }
}