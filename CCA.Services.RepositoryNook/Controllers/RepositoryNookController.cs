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
using System.Collections.Generic;

namespace CCA.Services.RepositoryNook.Controllers
{
    [Route("/")]
    public class RepositoryNookController : Controller
    {
        [HttpGet]   // GET all databases
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDatabases([FromServices]IRepositoryService repositoryService)
        {
            try
            {
                List<string> found = await repositoryService.GetDatabases();
                return ResponseFormatter.ResponseOK(found);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Get databases failed.");
            }
        }
        [HttpGet("{database}")]   // GET all collections
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCollections([FromServices]IRepositoryService repositoryService, string database)
        {
            try
            {
                List<string> found = await repositoryService.GetCollections(database);
                return ResponseFormatter.ResponseOK(found);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Get collections failed.");
            }
        }
        [HttpPost("{database}/{collection}")]  // POST (C)reate Repository object - CRUD operation: Create
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, [FromBody]Repository repoObject)
        {
            try
            {
                return ResponseFormatter.ResponseOK(await repositoryService.Create(database, collection, repoObject), "Created");
            }
            catch(Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Create failed.");
            }

        }
        //[HttpGet("{database}/{collection}/{_id}")]   // GET Repository object-by-id (Query by Id)      application should care about query by key or tag (not id) Delete by ID only
        //[SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> GetRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string _id)
        //{
        //    try
        //    {
        //        Repository found = await repositoryService.Read(_id, database, collection);

        //        return ResponseFormatter.ResponseOK(found);
        //    }
        //    catch (Exception exc)
        //    {
        //        return ResponseFormatter.ResponseBadRequest(exc, "Read failed.");
        //    }

        //}
        [HttpGet("{database}/{collection}")]   // GET All Repository objects (Query by "*" wildcard operation, or default: all records API call)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRepositoryObjects([FromServices]IRepositoryService repositoryService, string database, string collection, string _id)
        {
            try
            {
                List<Repository> found = repositoryService.ReadAll(database, collection);

                return ResponseFormatter.ResponseOK(found);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Read All failed.");
            }

        }
        // GET query by key
        [HttpGet("{database}/{collection}/{key}")]   
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> QueryByKeyRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string key)
        {
            try
            {
                List<Repository> found = repositoryService.QueryByKey(database, collection, key);

                if( found.Count == 0)
                {
                    return ResponseFormatter.ResponseNotFound(string.Format("check query string argument key={0}",key));
                }

                return ResponseFormatter.ResponseOK(found);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Query exception.");
            }

        }
        [HttpGet("{database}/{collection}/tag/{tag}")]   // query by tagName = tagValue
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> QueryByTagRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string tag)
        {
            try
            {
                List<Repository> found = repositoryService.QueryByTag(database, collection, tag);

                if (found.Count == 0)
                {
                    return ResponseFormatter.ResponseNotFound(string.Format("check query string argument tag={0}", tag));
                }

                return ResponseFormatter.ResponseOK(found);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Query failed.");
            }

        }
        [HttpPut("{database}/{collection}/{_id}")]  // update
        //[AllowAnonymous]    // allow anonymous as Tier 2, and API manager/gateway handle auth otherwise - we'll omit middleware from the Microservice API methods (for now)
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string _id, [FromBody]Repository repoObject)
        {
            try
            {
                await repositoryService.Update(_id, database, collection, repoObject);
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Update failed.");
            }
            try
            {
                Repository found = await repositoryService.Read(_id, database, collection);

                return ResponseFormatter.ResponseOK(found, "Updated");
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, "Retreiving Update failed. Record may still have been written.");
            }

        }
        [HttpDelete("{database}/{collection}/{_id}")]    // delete
        //[AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRepositoryObject([FromServices]IRepositoryService repositoryService, string database, string collection, string _id, [FromBody]Repository repoObject)
        {
            try
            {
                await repositoryService.Delete(_id, database, collection);

                return ResponseFormatter.ResponseOK($"_id: {_id} deleted.");
            }
            catch (Exception exc)
            {
                return ResponseFormatter.ResponseBadRequest(exc, $"Delete failed for _id: {_id}.");
            }

        }
    }
}
