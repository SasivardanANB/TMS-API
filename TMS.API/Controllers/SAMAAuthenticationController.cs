using System.Web.Http;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;

namespace TMS.API.Controllers
{
    [AllowAnonymous]
    public class SAMAAuthenticationController : ApiController
    {
        [HttpPost]
        public string AuthenticateUser(SAMATokenRequest request)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            return userTask.AuthenticateUser(request);
        }
    }
}