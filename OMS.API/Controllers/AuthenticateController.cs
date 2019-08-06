using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Objects;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OMS.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/Authenticate")]
    public class AuthenticateController : ApiController
    {
        [Route("GenerateandSaveToken")]
        [AllowAnonymous, HttpPost]
        public HttpResponseMessage GenerateandSaveToken(User clientkeys)
        {
            IAuthenticateTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().AuthenticateTask;
            var IssuedOn = DateTime.Now;
            var newToken = userTask.GenerateToken(clientkeys, IssuedOn);
            Authenticate token = new Authenticate();
            token.TokenID = 0;
            token.TokenKey = newToken;
            token.UserID = clientkeys.ID;
            token.IssuedOn = IssuedOn;
            token.ExpiresOn = DateTime.Now.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["TokenExpiry"]));
            token.CreatedOn = DateTime.Now;
            var result = userTask.InsertToken(token);

            if (result == 1)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
                response.Headers.Add("Token", newToken);
                response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["TokenExpiry"]);
                response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
                return response;
            }
            else
            {
                var message = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                message.Content = new StringContent("Error in Creating Token");
                return message;
            }
        }
    }
}
