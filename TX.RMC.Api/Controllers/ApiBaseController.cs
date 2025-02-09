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

    internal static object CreateSuccessResponse(object? value)
    {
        return new ApiResponse {Status = "success", Data = value };
    }

    internal static object CreateErrorResponse(string value)
    {
        return new ErrorResponse { Status = "error", Message = value };
    }

    internal static object CreateFailResponse(object? value)
    {
        return new ApiResponse { Status = "fail", Data = value };
    }
}

internal class ErrorResponse
{
    public string Status { get; init; } = "error";
    public string Message { get; init; } = null!;
}

internal class ApiResponse
{
    public string Status { get; init; } = "success";
    public object? Data { get; init; } = null!;
}