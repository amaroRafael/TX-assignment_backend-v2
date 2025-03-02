namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public abstract class ApiBaseController : ControllerBase
{
    public override OkObjectResult Ok([ActionResultObjectValue] object? value)
    {
        return base.Ok(CreateSuccessResponse(value));
    }

    public override CreatedAtActionResult CreatedAtAction(string? actionName, [ActionResultObjectValue] object? value)
    {
        return base.CreatedAtAction(actionName, CreateSuccessResponse(value));
    }

    public override AcceptedAtActionResult AcceptedAtAction(string? actionName, [ActionResultObjectValue] object? value)
    {
        return base.AcceptedAtAction(actionName, CreateSuccessResponse(value));
    }

    internal static ApiResponse<T> CreateSuccessResponse<T>(T? value)
    {
        return new ApiResponse<T> {Status = "success", Data = value };
    }

    internal static ErrorResponse CreateErrorResponse(string value)
    {
        return new ErrorResponse { Status = "error", Message = value };
    }

    internal static ApiResponse<T> CreateFailResponse<T>(T? value)
    {
        return new ApiResponse<T> { Status = "fail", Data = value };
    }
}

internal class ErrorResponse
{
    public string Status { get; init; } = "error";
    public string Message { get; init; } = null!;
}

internal class ApiResponse<T>
{
    public string Status { get; init; } = "success";
    public T? Data { get; init; }
}