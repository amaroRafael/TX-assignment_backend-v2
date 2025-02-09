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

    private static object CreateSuccessResponse(object? value)
    {
        return new { status = "success", data = value };
    }

    protected static object CreateErrorResponse(string value)
    {
        return new { status = "error", message = value };
    }

    protected static object CreateFailResponse(object? value)
    {
        return new { status = "fail", data = value };
    }
}
