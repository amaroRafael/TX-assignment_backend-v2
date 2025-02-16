using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using TX.RMC.BusinessLogic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TX.RMC.FunctionApp
{
    public class GetStatus(RobotService robotService, ILogger<GetStatus> logger) : ApiBase
    {
        const double revenuePerkW = 0.12;
        const double technicianCost = 250;
        const double turbineCost = 100;
        const string error = "Operation could not be executed at this moment.";
        private readonly RobotService robotService = robotService;
        private readonly ILogger<GetStatus> _logger = logger;

        [Function("GetStatus")]
        [OpenApiOperation(operationId: "Run")]
        [OpenApiSecurity("function_key", SecuritySchemeType.Http, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(StatusResponse),
            Description = "JSON request body containing { hours, capacity}")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string),
            Description = "The OK response message containing a JSON result.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "status/{robot}")] HttpRequest req, [FromRoute] string robot,
            ILogger log)
        {
            try
            {
                string status = await this.robotService.GetStatusAsync(robot, req.HttpContext.RequestAborted);
                return this.Ok(new StatusResponse { Robot = robot, Status = status });
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error executing [Get] method to retrieve robot status.");
                return BadRequest(CreateErrorResponse(error));
            }
        }

        internal class StatusResponse
        {
            public string Robot { get; init; } = null!;
            public string Status { get; init; } = null!;
        }
    }
}
