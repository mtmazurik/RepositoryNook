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
    [Route("/admin")]
    public class AdminController : Controller
    {
        [HttpPut("kill")]   // Kills the main thread, effectively shutting it down
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult Kill([FromServices]IAdminService instrument)
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
        public IActionResult GetVersion([FromServices]IAdminService instrumentation)
        {
            return ResponseFormatter.ResponseOK((new JProperty("Version", instrumentation.version())));
        }
    }
}
