using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CCA.Services.RepositoryNook.JsonHelpers;
using CCA.Services.RepositoryNook.Models;
using CCA.Services.RepositoryNook.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using SchemaRegistry = CCA.Services.RepositoryNook.Models.SchemaRegistry;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Controllers
{
    [Route("/")]
    public class RepositoryNookController : Controller
    {
        [HttpPost("")]
        [AllowAnonymous]    // allow anonymous as Tier 2, and API manager/gateway handle auth otherwise - we'll omit middleware from the Microservice API methods (for now)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRepositoryObject([FromServices]IRepositoryService repositoryService, [FromBody]Repository repoObject)
        {
            try
            {
                return ResponseFormatter.ResponseOK(await repositoryService.Create(repoObject), "Created");
            }
            catch(ApplicationException exc)
            {
                return BadRequest(exc.InnerException);
            }

        }
        [HttpDelete("")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRepositoryObject([FromServices]IRepositoryService repositoryService, [FromBody]Repository repoObject)
        {
            try
            {
                await repositoryService.Delete(repoObject);
                return ResponseFormatter.ResponseOK(new JProperty(repoObject._id.ToString(), "Deleted"));
            }
            catch (ApplicationException exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc,"Failed. _id not found. or, check repository name, and collection name.");
            }

        }
        [HttpPost("schema")]
        [AllowAnonymous]    // no Auth needed - for now
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult CreateSchemaRegistryObject([FromServices]ISchemaRegistryService service, [FromBody]SchemaRegistry schemaRegistry)
        {
            try
            {
                return ResponseFormatter.ResponseOK(service.Create(schemaRegistry), "Created");
            }
            catch (ApplicationException exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPut("kill")]   // Kills the main thread, effectively shutting it down
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult Kill([FromServices]IPlumbingService instrument)
        {
            return ResponseFormatter.ResponseOK(instrument.kill());
        }
        [HttpGet("ping")]   // ping
        [AllowAnonymous]    // no Auth / controller response only (not service level responses)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetPing()
        {
            return ResponseFormatter.ResponseOK((new JProperty("Ping", "Success")));
        }
        [HttpGet("version")]   // service version (from compiled assembly version)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetVersion([FromServices]IPlumbingService instrumentation)
        {
            return ResponseFormatter.ResponseOK((new JProperty("Version", instrumentation.version())));
        }
    }
}
