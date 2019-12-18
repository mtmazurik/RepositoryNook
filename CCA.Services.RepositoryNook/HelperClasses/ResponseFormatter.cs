using CCA.Services.RepositoryNook.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.HelperClasses
{
    public static class ResponseFormatter
    {
        public static JsonResult ResponseOK(JProperty property)
        {
            Response response = new Response();
            response.Meta.Add(property.Name, property.Value.ToString());
            JsonResult result = new JsonResult(response)
            {
                StatusCode = 200
            };
            return result;
        }
        public static JsonResult ResponseOK(String stringResponse)
        {
            Response response = new Response();
            response.Meta.Add("Message", stringResponse);
            JsonResult result = new JsonResult(response)
            {
                StatusCode = 200
            };
            return result;
        }
        public static JsonResult ResponseOK(object theObject, string descr="Success" )
        {
            Response response = new Response(theObject);
            response.Meta.Add("Message", descr);
            JsonResult result = new JsonResult(response)
            {
                StatusCode = 200
            };
            return result;
        }
        public static JsonResult ResponseNotFound(string descr = "Failed")
        {
            Response response = new Response();
            response.Meta.Add("Message", descr);
            JsonResult result = new JsonResult(response)
            {
                StatusCode = 404
            };
            return result;
        }
        public static JsonResult ResponseBadRequest(object theObject, string descr = "Failed")
        {
            Response response = new Response(theObject);
            response.Meta.Add("Message", descr);
            JsonResult result = new JsonResult(response)
            {
                StatusCode = 400
            };
            return result;
        }
        public static JsonResult Format(int code, IResponse response)
        {
            JsonResult result = new JsonResult(response)
            {
                StatusCode = code
            };
            return result;
        }
        public static JsonResult Format(int code, string message)
        {
            JsonResult result = new JsonResult(populateMeta(code, message))
            {
                StatusCode = code
            };
            return result;
        }
        public static JsonResult Format(int code, Exception exc)
        {
            JsonResult result = new JsonResult(populateMeta(code, exc))
            {
                StatusCode = code
            };
            return result;
        }
        private static Response populateMeta(int code, string message)
        {
            Response response = new Response();
            response.Meta.Add("status", code.ToString());
            response.Meta.Add("message", message);
            return response;
        }
        private static Response populateMeta(int code, Exception exc)
        {
            Response response = new Response();
            response.Meta.Add("status", code.ToString());
            response.Meta.Add("message", exc.Message);
            response.Meta.Add("details", exc.ToString());
            if (exc.InnerException != null)
            {
                response.Meta.Add("InnerException", exc.InnerException.ToString());
            }
            return response;
        }
    }
}
