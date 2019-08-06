using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.API.Classes;

namespace DMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/user")]
    public class UserController : ApiController
    {
        [Route("login")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult LoginUser(LoginRequest login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userData = userTask.LoginUser(login);
            return Ok(userData);
        }

        [Route("getprofiledetails")]
        [HttpPost]
        public IHttpActionResult GetProfileDetails(int userID)
        {
            if (userID <= 0)
            {
                return Ok("Invalid userID");
            }
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.GetProfileDetails(userID);
            return Ok(userResponse);
        }

        [Route("createupdateuser")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult CreateUpdateUser(UserRequest user)
        {
            if (user.Requests != null && user.Requests.Count > 0 && user.Requests[0].ID > 0)
            {
                ModelState.Remove("user.Requests[0].Password");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.CreateUpdateUser(user);
            return Ok(userResponse);
        }

        [Route("changepassword")]
        public IHttpActionResult ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.ChangePassword(changePasswordRequest, "changepassword");
            return Ok(userResponse);
        }

        [Route("forgotpassword")]
        [AllowAnonymous, HttpPost]
        public HttpResponseMessage ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            return userTask.ForgotPassword(forgotPasswordRequest);
        }

        [Route("resetpassword")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult ResetPassword(ChangePasswordRequest changePasswordRequest)
        {
            ModelState.Remove("changePasswordRequest.OldPassword");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.ChangePassword(changePasswordRequest, "resetpassword");
            return Ok(userResponse);
        }
    }
}
