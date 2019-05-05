using NLog;
using TMS.DomainGateway.Task.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace TMS.API.Classes
{
    public class CustomAuthorize: AuthorizeAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext)) return;
            if (Authorize(actionContext))
            {
                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }
        private bool Authorize(HttpActionContext actionContext)
        {
            try
            {
                var encodedString = "";
                bool isValidFlag = false;
                if (actionContext.Request.Headers.Contains("Token")) { 
                 encodedString = actionContext.Request.Headers.GetValues("Token").First();
                }
                else
                {
                    return isValidFlag;
                }
                if (!string.IsNullOrEmpty(encodedString))
                {
                    IAuthenticateTask authenticateTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<DomainGateway.Gateway.Interfaces.ITaskGateway>().AuthenticateTask;
                    isValidFlag = authenticateTask.ValidateToken(encodedString);
                    return isValidFlag;
                }
                return isValidFlag;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                return false;
            }
        }
    }
}