using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CCA.Services.RepositoryNook.JsonHelpers;
using CCA.Services.RepositoryNook.Models;
using Microsoft.Extensions.Logging;
using CCA.Services.RepositoryNook.Service;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace CCA.Services.RepositoryNook.Controllers
{
    [Route("/")]
    public class RepositoryNookController : Controller
    {
        [HttpPost("")]
        [AllowAnonymous]    // no Auth needed - for now
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult CreateRepositoryObject([FromServices]IRepositoryNookService service, [FromBody]Repository repoObject, [FromQuery(Name ="validate")] bool validate=false)
        {
            try
            {
                // TO-DO: validate: read raw body, check if validation set (?validation=true) on querystring and perform schema validation against incoming repo object

                return ResponseFormatter.ResponseOK(service.Create(repoObject), "Created");
            }
            catch(ApplicationException exc)
            {
                return BadRequest(exc.InnerException);
            }

        }
        [HttpPost("schema")]
        [AllowAnonymous]    // no Auth needed - for now
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult CreateSchemaObject([FromServices]IRepositoryNookService service, [FromBody]Repository repoObject)
        {
            try
            {
                return ResponseFormatter.ResponseOK(service.Create(repoObject), "Created");
            }
            catch (ApplicationException exc)
            {
                return BadRequest(exc);
            }

        }
        [HttpPut("kill")]   // Kills the main thread, effectively shutting it down
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult Kill([FromServices]IRepositoryNookService service)
        {
            return ResponseFormatter.ResponseOK(service.kill());
        }
        [HttpGet("ping")]   // ping
        [AllowAnonymous]    // no Auth needed 
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetPing()
        {
            return ResponseFormatter.ResponseOK((new JProperty("Ping", "Success")));
        }
        [HttpGet("version")]   // service version (from compiled assembly version)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetVersion()
        {
            var assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();
            return ResponseFormatter.ResponseOK((new JProperty("Version", assemblyVersion)));
        }
    }
}
