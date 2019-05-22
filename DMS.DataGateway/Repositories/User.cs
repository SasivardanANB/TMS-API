using AutoMapper;
using NLog;
using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DataGateway.DataModels;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataModel = DMS.DataGateway.DataModels;
using Domain = DMS.DomainObjects.Objects;
using System.Configuration;
using System.Net.Mail;

namespace DMS.DataGateway.Repositories
{

    public class User : IUser
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserResponse LoginUser(LoginRequest login)
        {
            UserResponse userResponse = new UserResponse()
            {
                Data = new List<Domain.User>()
            };
            try
            {
                using (var context = new DMSDBContext())
                {
                    string encryptedPassword = Encryption.EncryptionLibrary.EncryptPassword(login.UserPassword);
                    var userData = (from user in context.Users
                                    where user.UserName == login.UserName
                                    && user.Password == encryptedPassword
                                    select new Domain.User()
                                    {
                                        ID = user.ID,
                                        IsActive = user.IsActive,
                                        UserName = user.UserName
                                    }).FirstOrDefault();

                    if (userData != null)
                    {
                        if (!userData.IsActive)
                        {
                            userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                            userResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserInActive;
                        }
                        else
                        {

                            userResponse.Data.Add(userData);

                            //Creating token for this user
                            Authenticate authenticate = new Authenticate();
                            string token = authenticate.GenerateToken(userData, DateTime.Now);

                            //Inserting token for this user in database
                            authenticate.InsertToken(new Domain.Authenticate()
                            {
                                TokenKey = token,
                                CreatedOn = DateTime.Now,
                                IssuedOn = DateTime.Now,
                                ExpiresOn = DateTime.Now.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiry"])),
                                UserID = userData.ID
                            });

                            //Get Token and add to response
                            var tokenData = (from tokens in context.TokenManagers
                                             where tokens.TokenKey == token
                                             && tokens.UserID == userData.ID
                                             select new Domain.Authenticate()
                                             {
                                                 UserID = tokens.UserID,
                                                 TokenKey = tokens.TokenKey,
                                                 CreatedOn = tokens.CreatedOn,
                                                 IssuedOn = tokens.IssuedOn,
                                                 ExpiresOn = tokens.ExpiresOn,
                                             }).FirstOrDefault();


                            userResponse.TokenKey = tokenData.TokenKey;
                            userResponse.TokenIssuedOn = tokenData.IssuedOn;
                            userResponse.TokenExpiresOn = tokenData.ExpiresOn;

                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.LoginSuccess;
                        }
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        userResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.LoginFail;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }

        public UserResponse CreateUpdateUser(UserRequest user)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new DMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.User, DataModel.User>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var users = mapper.Map<List<Domain.User>, List<DataModel.User>>(user.Requests);

                    //Encrypt Password
                    foreach (var userData in users)
                    {
                        if (!string.IsNullOrEmpty(userData.Password))
                        {
                            userData.Password = Encryption.EncryptionLibrary.EncryptPassword(userData.Password);
                        }

                        if (userData.ID > 0) //Update User
                        {
                            var existingUserData = context.Users.FirstOrDefault(t => t.ID == userData.ID);
                            existingUserData.FirstName = userData.FirstName;
                            existingUserData.LastName = userData.LastName;
                            existingUserData.Email = userData.Email;
                            existingUserData.PhoneNumber = userData.PhoneNumber;
                            existingUserData.LastModifiedBy = user.LastModifiedBy;
                            existingUserData.LastModifiedTime = DateTime.Now;

                            context.Entry(existingUserData).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersUpdated;
                        }
                        else //Create User
                        {
                            context.Users.Add(userData);
                            context.SaveChanges();
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersCreated;
                        }
                    }

                    user.Requests = mapper.Map<List<DataModel.User>, List<Domain.User>>(users);
                    userResponse.Data = user.Requests;
                    userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    userResponse.StatusCode = (int)HttpStatusCode.OK;

                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }
        public UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest,string type)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new DMSDBContext())
                {
                    var userDetails = context.Users.Where(u => u.ID == changePasswordRequest.Id).FirstOrDefault();
                    if (userDetails != null)
                    {
                        if (type == "changepassword")
                        {
                            var userPassword= Encryption.EncryptionLibrary.DecrypPassword(userDetails.Password);
                            if(userPassword!= changePasswordRequest.OldPassword)
                            {
                                userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                                userResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.OldPasswordMismatch;
                            }
                            else
                            {
                                if (userPassword == changePasswordRequest.NewPassword)
                                {
                                    userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                    userResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                                    userResponse.StatusMessage = DomainObjects.Resource.ResourceData.NewPasswordMustbeDifferent;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(changePasswordRequest.NewPassword))
                                    {
                                        changePasswordRequest.NewPassword = Encryption.EncryptionLibrary.EncryptPassword(changePasswordRequest.NewPassword);
                                    }
                                    userDetails.Password = changePasswordRequest.NewPassword;
                                    context.SaveChanges();
                                    userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                                    userResponse.StatusCode = (int)HttpStatusCode.OK;
                                    userResponse.StatusMessage = DomainObjects.Resource.ResourceData.PasswordUpdated;
                                }
                            }
                            
                        }
                        if (type == "resetpassword")
                        {
                            if (!string.IsNullOrEmpty(changePasswordRequest.NewPassword))
                            {
                                changePasswordRequest.NewPassword = Encryption.EncryptionLibrary.EncryptPassword(changePasswordRequest.NewPassword);
                            }
                            userDetails.Password = changePasswordRequest.NewPassword;
                            context.SaveChanges();
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.PasswordUpdated;
                        }
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserDetailsNotFound;
                    }
                }                
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }
        public UserResponse ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new DMSDBContext())
                {
                    var userDetails = context.Users.Where(u => u.Email == forgotPasswordRequest.Email).FirstOrDefault();
                    if(userDetails !=null)
                    {
                        MailMessage mail = new MailMessage();
                        mail.To.Add(forgotPasswordRequest.Email);
                        string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                        mail.From = new MailAddress(emailFrom);
                        mail.Subject = ConfigurationManager.AppSettings["EmailSubject"]; // "Test-case";
                        string Body = "To reset your password click the link : " + ConfigurationManager.AppSettings["ResetPassword"]+"?userId="+userDetails.ID;
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

                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.OK;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserDetailsNotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }

        public UserResponse GetProfileDetails(int userID)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new DMSDBContext())
                {
                    var userDetails = context.Users.Where(i => i.ID == userID && i.IsActive).Select(user=>new Domain.User {
                        ID=user.ID,
                        UserName=user.UserName,
                        FirstName=user.FirstName,
                        LastName=user.LastName,
                        Email=user.Email,
                       PhoneNumber=user.PhoneNumber
                    }).FirstOrDefault();
                    if (userDetails != null)
                    {
                        List<Domain.User> users = new List<Domain.User>();
                        users.Add(userDetails);
                        userResponse.Data = users;
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.OK;
                        userResponse.StatusMessage= DomainObjects.Resource.ResourceData.Success; 
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserDetailsNotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }
    }
    
}





