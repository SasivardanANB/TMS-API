using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Request;
using System.Web.Http;

namespace OMS.API.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/v1/sama")]
    public class SAMAAuthenticationController : ApiController
    {
        [Route("authenticateuser")]
        [HttpPost]       
        public string AuthenticateUser(SAMATokenRequest request)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            return userTask.AuthenticateUser(request);
        }
    }
}