using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace DMS.BusinessGateway.Task
{
    public partial class BusinessUserTask : UserTask
    {
        private readonly IUser _userRepository;
        public BusinessUserTask(IUser userRepository)
        {
            _userRepository = userRepository;
        }

        public override UserResponse LoginUser(LoginRequest login)
        {
            UserResponse userData = _userRepository.LoginUser(login);
            return userData;
        }

        public override UserResponse CreateUpdateUser(UserRequest user)
        {
            UserResponse userData = _userRepository.CreateUpdateUser(user);
            return userData;
        }
        public override UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest, string type)
        {
            UserResponse userData = _userRepository.ChangePassword(changePasswordRequest, type);
            return userData;
        }

        public override UserResponse GetProfileDetails(int userID)
        {
            UserResponse userData = _userRepository.GetProfileDetails(userID);
            return userData;
        }

        public override HttpResponseMessage ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(forgotPasswordRequest.Email);
                string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                mail.From = new MailAddress(emailFrom);
                mail.Subject = ConfigurationManager.AppSettings["EmailSubject"];
                string Body = "To reset your password click the link : " + forgotPasswordRequest.URLLink;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                string loginEmailId = ConfigurationManager.AppSettings["SmtpUserName"];
                string emailPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                SmtpClient smtp = new SmtpClient(smtpHost)
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(loginEmailId, emailPassword),
                    EnableSsl = true
                };
                smtp.Send(mail);
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Email sent successfully")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    Content = new StringContent("Some issues occured while sending email. " + ex.Message),

                };
            }
        }
    }
}
