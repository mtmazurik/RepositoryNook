using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.HelperClasses
{
    public class AllowAnonymousFilter : IActionFilter
    {

        private ILogger _logger;
        public AllowAnonymousFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //do nothing
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (IsAnonymous(context))
            {
                return;
            }
            else
            {
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        Content = "Not Authorized"
                    };
                    _logger.Log(LogLevel.Error, "Unauthorized API call: check bearer token of request.");
                }
            }
        }


        private static bool IsAnonymous(ActionExecutingContext actionContext)
        {
            var endpointMethod = actionContext.ActionDescriptor as ControllerActionDescriptor;
            
            if (endpointMethod != null)
            {
                if (endpointMethod.MethodInfo.CustomAttributes.Any(attr => attr.AttributeType == typeof(AllowAnonymousAttribute)))
                {
                    return true;
                }
            }
            return false;           
        }
    }
}
