using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CCA.Services.RepositoryNook.HelperClasses;
using CCA.Services.RepositoryNook.Models;
using CCA.Services.RepositoryNook.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CCA.Services.RepositoryNook.Controllers
{
    [Route("/")]
    public class RepositoryNookController : Controller
    {

        [HttpPost("{repository}/{collection}")]  // create
        [AllowAnonymous]    // allow anonymous (for tier 2 services), API Manager or Gateway should handle AUTH
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRepositoryObject([FromServices]IRepositoryService repositoryService, string repository, string collection, [FromBody]Repository repoObject)
        {
            try
            {
                return ResponseFormatter.ResponseOK(await repositoryService.Create(repository, collection, repoObject), "Created");
            }
            catch(Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc);
            }

        }
        [HttpGet("{repository}/{collection}/{_id}")]   // read
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRepositoryObject([FromServices]IRepositoryService repositoryService, string repository, string collection, string _id, [FromBody]Repository repoObject)
        {
            try
            {
                Repository found = await repositoryService.Read(_id, repository, collection, repoObject);

                return ResponseFormatter.ResponseOK(found);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Read failed. _id not found; or check repository name and collection name.");
            }

        }
        [HttpPut("{repository}/{collection}/{_id}")]  // update
        [AllowAnonymous]    // allow anonymous as Tier 2, and API manager/gateway handle auth otherwise - we'll omit middleware from the Microservice API methods (for now)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRepositoryObject([FromServices]IRepositoryService repositoryService, string repository, string collection, string _id, [FromBody]Repository repoObject)
        {
            try
            {
                await repositoryService.Update(_id, repository, collection, repoObject);

                return ResponseFormatter.ResponseOK(new JProperty(repoObject._id.ToString(), "Updated"));
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Update failed. _id not found; or check repository name and collection name.");
            }

        }
        [HttpDelete("{repository}/{collection}/{_id}")]    // delete
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRepositoryObject([FromServices]IRepositoryService repositoryService, string repository, string collection, string _id, [FromBody]Repository repoObject)
        {
            try
            {
                await repositoryService.Delete(_id, repository, collection);

                return ResponseFormatter.ResponseOK(new JProperty(repoObject._id.ToString(), "Deleted"));
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Delete failed. _id not found; or check repository name and collection name.");
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
