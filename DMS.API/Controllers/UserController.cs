using DMS.DomainGateway.Gateway;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Helper.Model.DependencyResolver;
using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.API.Classes;
using System.Net.Mail;
using System.Configuration;

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
            if (user.Requests != null && user.Requests.Count > 0)
            {
                if (user.Requests[0].ID > 0)
                {
                    ModelState.Remove("user.Requests[0].Password");
                }
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
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            if (!ModelState.IsValid)
            {
                 responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
                return responseMessage;
            }
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(forgotPasswordRequest.Email);
                string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                mail.From = new MailAddress(emailFrom);
                mail.Subject = ConfigurationManager.AppSettings["EmailSubject"]; // "Test-case";
                string Body = "To reset your password click the link : " + forgotPasswordRequest.URLLink;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                string loginEmailId = ConfigurationManager.AppSettings["SmtpUserName"];
                string emailPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                SmtpClient smtp = new SmtpClient(smtpHost);
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(loginEmailId, emailPassword); // Enter senders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
                 responseMessage = Request.CreateResponse(HttpStatusCode.OK, "Email sent successfully");
               return responseMessage;
            }
            catch (Exception ex )
            {
                 responseMessage = Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Some issues occured while sending email",ex.Message);
                return responseMessage;
            }
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
