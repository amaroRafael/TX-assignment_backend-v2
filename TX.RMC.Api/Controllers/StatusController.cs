namespace TX.RMC.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TX.RMC.BusinessLogic;

[Route("[controller]")]
[ApiController]
[Authorize]
public class StatusController(StatusService statusService) : ControllerBase
{
    private readonly StatusService statusService = statusService;

    [HttpGet("{robot}")]
    public async Task<IActionResult> Get(string robot)
    {
        string status = await this.statusService.GetStatusAsync(robot);

        return new OkObjectResult(new { robot, status });
    }
}
