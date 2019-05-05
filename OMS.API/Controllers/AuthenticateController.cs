using OMS.DomainGateway.Gateway;
using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OMS.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/Authenticate")]
    public class AuthenticateController : ApiController
    {

        //[Route("Authenticate")]
        //[AllowAnonymous, HttpPost]
        //public string LoginUser(User login)
        //{
        //    IAuthenticateTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().AuthenticateTask;
        //    var IssuedOn = DateTime.Now;
        //    string token = userTask.GenerateToken(login, IssuedOn);


        //    return userData;
        //}

        //public HttpResponseMessage Authenticate([FromBody]  ClientKeys)
        //{
        //    if (string.IsNullOrEmpty(ClientKeys.ClientID) && string.IsNullOrEmpty(ClientKeys.ClientSecret))
        //    {
        //        var message = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        //        message.Content = new StringContent("Not Valid Request");
        //        return message;
        //    }
        //    else
        //    {
        //        if (_IAuthenticate.ValidateKeys(ClientKeys))
        //        {
        //            var clientkeys = _IAuthenticate.GetClientKeysDetailsbyCLientIDandClientSecert(ClientKeys.ClientID, ClientKeys.ClientSecret);

        //            if (clientkeys == null)
        //            {
        //                var message = new HttpResponseMessage(HttpStatusCode.NotFound);
        //                message.Content = new StringContent("InValid Keys");
        //                return message;
        //            }
        //            else
        //            {
        //                if (_IAuthenticate.IsTokenAlreadyExists(clientkeys.CompanyID))
        //                {
        //                    _IAuthenticate.DeleteGenerateToken(clientkeys.CompanyID);

        //                    return GenerateandSaveToken(clientkeys);
        //                }
        //                else
        //                {
        //                    return GenerateandSaveToken(clientkeys);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var message = new HttpResponseMessage(HttpStatusCode.NotFound);
        //            message.Content = new StringContent("InValid Keys");
        //            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable };
        //        }
        //    }
        //}


        //[NonAction]
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
                HttpResponseMessage response = new HttpResponseMessage();
                response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
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
