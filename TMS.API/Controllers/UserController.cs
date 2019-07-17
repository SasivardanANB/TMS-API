using TMS.DomainGateway.Gateway;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using TMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Helper.Model.DependencyResolver;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.API.Classes;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using RestSharp.Serialization.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Net.Mail;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/user")]
    public class UserController : ApiController
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Private Methods
        private static string GetApiResponse(string apiRoute, Method method, object requestQueryParameter, string token)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["ApiGatewayBaseURL"]);
            client.AddDefaultHeader("Content-Type", "application/json");
            if (token != null)
                client.AddDefaultHeader("Token", token);
            var request = new RestRequest(apiRoute, method) { RequestFormat = DataFormat.Json };
            request.Timeout = 500000;
            if (requestQueryParameter != null)
            {
                request.AddJsonBody(requestQueryParameter);
            }
            var result = client.Execute(request);
            return result.Content;
        }
        #endregion

        [Route("login")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult LoginUser(LoginRequest login)
        {
            if (login.IsSAMALogin)
            {
                ModelState.Remove("login.UserPassword");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userData = userTask.LoginUser(login);
            return Ok(userData);
        }

        [Route("getsamauser")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult GetSAMAUser(string token)
        {
            SAMAUserResponse samaUserResponse = new SAMAUserResponse();

            if (String.IsNullOrEmpty(token))
                return BadRequest(ModelState);

            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                RestRequest request = new RestRequest("/api/users/getmyuserinfo", Method.GET) { RequestFormat = DataFormat.Json };
                request.AddParameter("Authorization", string.Format("Bearer " + token), ParameterType.HttpHeader);
                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var SAMAUsers = JsonConvert.DeserializeObject<List<SAMAUser>>(response.Content);
                    if (SAMAUsers != null && SAMAUsers.Count == 1)
                    {
                        samaUserResponse.Data = SAMAUsers[0];
                        samaUserResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        samaUserResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                        samaUserResponse.StatusCode = (int)HttpStatusCode.OK;
                        samaUserResponse.NumberOfRecords = 1;
                    }
                    else
                    {
                        samaUserResponse.Data = null;
                        samaUserResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        samaUserResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                        samaUserResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                }
                else
                {
                    samaUserResponse.Data = null;
                    samaUserResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    samaUserResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidToken;
                    samaUserResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                samaUserResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                samaUserResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                samaUserResponse.StatusMessage = ex.Message;
            }
            return Ok(samaUserResponse);
        }

        #region "User Application"

        [Route("createupdateuser")]
        [HttpPost]
        public IHttpActionResult CreateUpdateUser(UserRequest user)
        {
            #region Model validation
            for (int i = 0; i < user.Requests.Count; i++)
            {
                if (user.Requests[i].Applications.Count > 0)
                {
                    foreach (var application in user.Requests[i].Applications)
                    {
                        if (application == 0)
                        {
                            ModelState.AddModelError($"{nameof(user)}.{nameof(user.Requests)}.[{i}].{nameof(TMS.DomainObjects.Objects.User.Applications)}", DomainObjects.Resource.ResourceData.InvalidApplication);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError($"{nameof(user)}.{nameof(user.Requests)}.[{i}].{nameof(TMS.DomainObjects.Objects.User.Applications)}", DomainObjects.Resource.ResourceData.InvalidApplication);
                }
                if (user.Requests[i].ID > 0)
                {
                    ModelState.Remove("user.Requests[" + i + "].Password");
                    ModelState.Remove("user.Requests[" + i + "].ConfirmPassword");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            #endregion

            UserResponse userResponse = new UserResponse();

            foreach (User userDetails in user.Requests)
            {
                #region Create OMSUserRequest
                UserRequest omsRequest = null;
                if (userDetails.Applications.Contains(1))
                {
                    omsRequest = new UserRequest()
                    {
                        Requests = new List<User>()
                        {
                            new User()
                            {
                                Applications = new List<int>(){
                                    1
                                },
                                FirstName = userDetails.FirstName,
                                LastName = userDetails.LastName,
                                UserName = userDetails.UserName,
                                Password = userDetails.Password,
                                ConfirmPassword = userDetails.ConfirmPassword,
                                Email = userDetails.Email,
                                PhoneNumber = userDetails.PhoneNumber
                            }
                        },
                        CreatedBy = "SYSTEM",
                        LastModifiedBy = user.LastModifiedBy,
                        CreatedTime = user.CreatedTime,
                        LastModifiedTime = user.LastModifiedTime
                    };
                }
                #endregion

                #region Create SAMAUserRequest
                SAMAUser samaUser = null;
                if (userDetails.ID == 0) // New User
                {
                    // Encrypt new user password
                    var client = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                    client.AddDefaultHeader("Content-Type", "application/json");

                    RestRequest request1 = new RestRequest("api/Accounts/EncryptPassword", Method.POST) { RequestFormat = DataFormat.Json };
                    request1.AddJsonBody(userDetails.Password);
                    IRestResponse response1 = client.Execute(request1);
                    dynamic data = JObject.Parse(response1.Content);
                    var encryptedUserPassword = data.result;

                    // Create SAMA Request
                    samaUser = new SAMAUser()
                    {
                        FirstName = userDetails.FirstName,
                        LastName = userDetails.LastName,
                        UserName = userDetails.UserName,
                        Password = encryptedUserPassword,
                        Email = "SAMATestEmail@SAMA.com"
                    };
                }
                #endregion

                #region CreateUpdateUser in TMS
                if (userDetails.Applications.Contains(2))
                {
                    IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
                    userResponse = userTask.CreateUpdateUser(user);
                }
                #endregion

                #region CreateUpdateUser in OMS
                if (omsRequest != null) //For OMS Application - Integrate Azure API Gateway
                {
                    LoginRequest loginRequest = new LoginRequest();
                    //Login to TMS and get Token
                    string token = string.Empty;
                    loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                    var OmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (OmsLoginResponse != null && OmsLoginResponse.Data.Count > 0)
                    {
                        token = OmsLoginResponse.TokenKey;
                    }

                    UserResponse omsUserResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                         + "/v1/user/createupdateuser", Method.POST, omsRequest, token));

                    if (!userDetails.Applications.Contains(2))
                    {
                        userResponse = omsUserResponse;
                    }
                    else
                    {
                        userResponse.StatusMessage = userResponse.StatusMessage + ". " + omsUserResponse.StatusMessage;
                    }

                }
                #endregion

                #region Create User in SAMA
                if (samaUser != null && Convert.ToBoolean(ConfigurationManager.AppSettings["CreateSAMAUser"]))
                {
                    // Encrypt SAMA default password
                    var client = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                    client.AddDefaultHeader("Content-Type", "application/json");
                    RestRequest request1 = new RestRequest("api/Accounts/EncryptPasswordAndUrlSafeEncoded", Method.POST) { RequestFormat = DataFormat.Json };
                    request1.AddJsonBody(ConfigurationManager.AppSettings["SAMAPassword"]);
                    IRestResponse response1 = client.Execute(request1);
                    dynamic data = JObject.Parse(response1.Content);
                    var encryptedDefaultPassword = data.result;


                    // Login to SAMA with default Username, encryptedDefaultPassword and get bearer token
                    var client2 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                    RestRequest request2 = new RestRequest("token", Method.POST);
                    request2.AddParameter("Authorization", "Basic NDFFNTBEM0ItMDU5Ni00REY5LUE5OUItNTVCN0JDM0VCNDFCOndUN2VVRFpLM1VFWlNOeGR1YXl4dzFzdWRnenN3VHdNOTBNOHMrUVJaYk09", ParameterType.HttpHeader);
                    request2.AddParameter("text/xml", "grant_type=password&username=" + ConfigurationManager.AppSettings["SAMALogin"] + "&password=" + encryptedDefaultPassword, ParameterType.RequestBody);
                    IRestResponse response2 = client.Execute(request2);
                    dynamic data2 = JObject.Parse(response2.Content);
                    var defaultUserBearerToken = data2.access_token;


                    // Create new SAMA User using defaultUserBearerToken
                    var client3 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                    client3.AddDefaultHeader("Content-Type", "application/json");
                    RestRequest request3 = new RestRequest("api/users/createuser", Method.POST) { RequestFormat = DataFormat.Json };
                    request3.AddParameter("Authorization", string.Format("Bearer " + defaultUserBearerToken), ParameterType.HttpHeader);
                    request3.AddJsonBody(samaUser);
                    IRestResponse response3 = client.Execute(request3);
                    dynamic data3 = JObject.Parse(response3.Content);


                    // Login to SAMA with New Username, encryptedUserPassword and get bearer token
                    string newUserBearerToken = string.Empty;


                    // Get Created User Details using NewUserBearerToken
                    var client4 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                    RestRequest request4 = new RestRequest("/api/users/getmyuserinfo", Method.GET) { RequestFormat = DataFormat.Json };
                    request4.AddParameter("Authorization", string.Format("Bearer " + newUserBearerToken), ParameterType.HttpHeader);
                    IRestResponse response4 = client.Execute(request4);
                    var SAMAUsers = JsonConvert.DeserializeObject<List<SAMAUser>>(response4.Content);
                    int newUserID = SAMAUsers[0].ID;


                    // Map OMS Application
                    if (userDetails.Applications.Contains(1)) // OMS
                    {
                        // Get OMS ClientID
                        dynamic obj = new { ApplicationCode = ConfigurationManager.AppSettings["SAMAOMSCode"] };
                        var client5 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                        client5.AddDefaultHeader("Content-Type", "application/json");
                        RestRequest request5 = new RestRequest("api/apps/searchapp", Method.POST) { RequestFormat = DataFormat.Json };
                        request5.AddParameter("Authorization", string.Format("Bearer " + defaultUserBearerToken), ParameterType.HttpHeader);
                        request5.AddJsonBody(obj);
                        IRestResponse response5 = client.Execute(request5);
                        var apps = JsonConvert.DeserializeObject<dynamic[]>(response5.Content);
                        string OMSSAMAClientID = apps[0].ClientID;

                        // Map User - OMS
                        dynamic userAppobj = new { ClientID = OMSSAMAClientID, UserID = newUserID, AddedOrRemovedBy = ConfigurationManager.AppSettings["SAMAOMSCode"] };
                        var client6 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                        client6.AddDefaultHeader("Content-Type", "application/json");
                        RestRequest request6 = new RestRequest("api/apps/searchapp", Method.POST) { RequestFormat = DataFormat.Json };
                        request6.AddParameter("Authorization", string.Format("Bearer " + defaultUserBearerToken), ParameterType.HttpHeader);
                        request6.AddJsonBody(userAppobj);
                        IRestResponse response6 = client.Execute(request6);
                    }


                    // Map TMS Application
                    if (userDetails.Applications.Contains(2)) // TMS
                    {
                        // Get TMS ClientID
                        dynamic obj = new { ApplicationCode = ConfigurationManager.AppSettings["SAMATMSCode"] };
                        var client7 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                        client7.AddDefaultHeader("Content-Type", "application/json");
                        RestRequest request7 = new RestRequest("api/apps/searchapp", Method.POST) { RequestFormat = DataFormat.Json };
                        request7.AddParameter("Authorization", string.Format("Bearer " + defaultUserBearerToken), ParameterType.HttpHeader);
                        request7.AddJsonBody(obj);
                        IRestResponse response7 = client.Execute(request7);
                        var appInfo = JsonConvert.DeserializeObject<dynamic[]>(response7.Content);
                        string TMSSAMAClientID = appInfo[0].ClientID;

                        // Map User - TMS
                        dynamic userAppobj = new { ClientID = TMSSAMAClientID, UserID = newUserID, AddedOrRemovedBy = ConfigurationManager.AppSettings["SAMATMSCode"] };
                        var client8 = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                        client8.AddDefaultHeader("Content-Type", "application/json");
                        RestRequest request8 = new RestRequest("api/apps/searchapp", Method.POST) { RequestFormat = DataFormat.Json };
                        request8.AddParameter("Authorization", string.Format("Bearer " + defaultUserBearerToken), ParameterType.HttpHeader);
                        request8.AddJsonBody(userAppobj);
                        IRestResponse response8 = client.Execute(request8);
                    }
                }
                #endregion
            }

            return Ok(userResponse);
        }

        [Route("deleteuser")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(int userID)
        {
            if (userID == 0)
            {
                ModelState.AddModelError($"{nameof(userID)}.{nameof(userID)}.[{userID}].{nameof(TMS.DomainObjects.Objects.User.ID)}", DomainObjects.Resource.ResourceData.InvalidUserID);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.DeleteUser(userID);
            return Ok(userResponse);
        }

        [Route("getusers")]
        [HttpPost]
        public IHttpActionResult GetUsers(UserRequest user)
        {
            for (int i = 0; i < user.Requests.Count; i++)
            {
                ModelState.Remove("user.Requests[" + i + "].UserName");
                ModelState.Remove("user.Requests[" + i + "].Email");
                ModelState.Remove("user.Requests[" + i + "].PhoneNumber");
                ModelState.Remove("user.Requests[" + i + "].Password");
                ModelState.Remove("user.Requests[" + i + "].ConfirmPassword");
                ModelState.Remove("user.Requests[" + i + "].FirstName");
                ModelState.Remove("user.Requests[" + i + "].LastName");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse usersList = userTask.GetUsers(user);
            return Ok(usersList);
        }

        [Route("changepassword")]
        [HttpPost]
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
        public IHttpActionResult ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.ForgotPassword(forgotPasswordRequest);

            if (userResponse.StatusCode == (int)HttpStatusCode.OK && userResponse.Status == DomainObjects.Resource.ResourceData.Success)
            {
                User userDetails = userResponse.Data[0];
                if (userDetails != null && Convert.ToBoolean(ConfigurationManager.AppSettings["EmailFeature"]) == true)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]);
                    mail.To.Add(userDetails.Email);
                    mail.Subject = ConfigurationManager.AppSettings["EmailSubject"];
                    string resetpasswordlink = ConfigurationManager.AppSettings["ResetPasswordLink"] + "?userId=" + userDetails.ID;

                    //Fetching Email Body Text from EmailTemplate File.  
                    var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplates/ResetPasswordTemplate.html");
                    string FilePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    //Repalce [newusername] = signup user name   
                    MailText = MailText.Trim().Replace("[namauser]", userDetails.UserName).Replace("[resetpasswordlink]", resetpasswordlink);

                    mail.Body = MailText;
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
                }
            }
            return Ok(userResponse);
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

        [Route("updateuserprofile")]
        [HttpPost]
        public IHttpActionResult UpdateUserProfile(UserRequest user)
        {
            User userProfile = user.Requests[0];

            if (userProfile.ID > 0)
            {
                ModelState.Remove("user.Requests[0].UserName");
                ModelState.Remove("user.Requests[0].FirstName");
                ModelState.Remove("user.Requests[0].LastName");
                ModelState.Remove("user.Requests[0].Password");
                ModelState.Remove("user.Requests[0].ConfirmPassword");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Update User Profile in OMS
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.UpdateUserProfile(user);

            // Prepare TMS Request
            UserRequest omsRequest = new UserRequest()
            {
                Requests = new List<User>() {
                    new User(){
                        FirstName = userProfile.FirstName,
                        LastName = userProfile.LastName,
                        UserName = userResponse.Data[0].UserName,
                        Email    = userProfile.Email,
                        PhoneNumber    = userProfile.PhoneNumber,
                    }
                },
                LastModifiedBy = user.LastModifiedBy,
            };

            // Update UserProfile in OMS if user exists
            if (userResponse.StatusCode == (int)HttpStatusCode.OK && userResponse.Status == DomainObjects.Resource.ResourceData.Success)
            {
                LoginRequest loginRequest = new LoginRequest();
                string token = "";
                if (userResponse.Data[0].Applications != null && userResponse.Data[0].Applications.Contains(2)) //For TMS Application - Integrate Azure API Gateway
                {
                    //Login to OMS and get Token
                    loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                    var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                    {
                        token = tmsLoginResponse.TokenKey;
                    }

                    // Update UserProfile in TMS
                    UserResponse tmsUserResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                        + "/v1/user/updateuserprofile", Method.POST, omsRequest, token));

                    if (tmsUserResponse.StatusCode == (int)HttpStatusCode.OK && tmsUserResponse.Status == DomainObjects.Resource.ResourceData.Success)
                    {
                        userResponse.StatusMessage = userResponse.StatusMessage + ". " + tmsUserResponse.StatusMessage;
                    }
                }
            }

            return Ok(userResponse);
        }

        #endregion

        #region "Role Management"

        [Route("createupdaterole")]
        [HttpPost]
        public IHttpActionResult CreateUpdateRole(RoleRequest role)
        {
            for (int i = 0; i < role.Requests.Count; i++)
            {
                if (role.Requests[i].RoleMenus.Count > 0)
                {
                    foreach (var menu in role.Requests[i].RoleMenus)
                    {
                        if (menu.ID == 0)
                        {
                            ModelState.AddModelError($"{nameof(role)}.{nameof(role.Requests)}.[{i}].{nameof(Role.RoleMenus)}", "Invalid Menu ID");
                        }


                        foreach (var roleMenuActivity in menu.RoleMenuActivities)
                        {
                            // Todo:Validations for requirefields
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError($"{nameof(role)}.{nameof(role.Requests)}.[{i}].{nameof(Role.RoleMenus)}", "Invalid Menu ID");
                }

            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RoleResponse userResponse = userTask.CreateUpdateRole(role);
            return Ok(userResponse);
        }

        [Route("deleterole")]
        [HttpDelete]
        public IHttpActionResult DeleteRole(int roleID)
        {
            if (roleID == 0)
            {
                ModelState.AddModelError($"{nameof(roleID)}.{nameof(roleID)}.[{roleID}].{nameof(TMS.DomainObjects.Objects.User.ID)}", DomainObjects.Resource.ResourceData.InvalidUserID);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RoleResponse roleResponse = userTask.DeleteRole(roleID);
            return Ok(roleResponse);
        }

        [Route("getroles")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult GetRoles(RoleRequest roles)
        {
            ModelState.Remove("roles.Requests[0].RoleCode");
            ModelState.Remove("roles.Requests[0].ValidFrom");
            ModelState.Remove("roles.Requests[0].ValidTo");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RoleResponse rolesList = userTask.GetRoles(roles);
            return Ok(rolesList);
        }

        [Route("getroledetails")]
        [HttpGet]
        public IHttpActionResult GetRoleDetails(int roleId)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RoleResponse roleResponse = userTask.GetRoleDetails(roleId);
            return Ok(roleResponse);
        }
        #endregion

        #region "User Role"

        [Route("createupdateuserrole")]
        [HttpPost]
        public IHttpActionResult CreateUpdateUserRole(UserRoleRequest userRoleRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserRoleResponse userRoleResponse = userTask.CreateUpdateUserRole(userRoleRequest);
            return Ok(userRoleResponse);
        }

        [Route("deleteuserrole")]
        [HttpDelete]
        public IHttpActionResult DeleteUserRole(int userRoleID)
        {
            if (userRoleID <= 0)
                return BadRequest(DomainObjects.Resource.ResourceData.InvalidUserID);
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.DeleteUserRole(userRoleID);
            return Ok(userResponse);
        }

        [Route("getuserroles")]
        [HttpPost]
        public IHttpActionResult GetUserRoles(UserRoleRequest userRoleRequest)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserRoleResponse userRoleResponse = userTask.GetUserRoles(userRoleRequest);
            return Ok(userRoleResponse);
        }


        #endregion

        #region "Master Data Operations"

        [Route("getregions")]
        [HttpPost]
        public IHttpActionResult GetRegions(RegionRequest region)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RegionResponse regionsList = userTask.GetRegions(region);
            return Ok(regionsList);
        }

        [Route("getmenuwithactivities")]
        [HttpGet]
        public IHttpActionResult GetMenuWithActivities()
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RoleMenuResponse roleMenuResponse = userTask.GetMenuWithActivities();
            return Ok(roleMenuResponse);
        }

        [Route("getapplications")]
        [HttpGet]
        public IHttpActionResult GetApplications()
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            ApplicationResponse roleMenuResponse = userTask.GetApplications();
            return Ok(roleMenuResponse);
        }

        [Route("getusernames")]
        [HttpGet]
        public IHttpActionResult GetUserNames()
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            CommonResponse commonResponse = userTask.GetUserNames();
            return Ok(commonResponse);
        }

        [Route("getrolecodes")]
        [HttpGet]
        public IHttpActionResult GetRoleCodes()
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            CommonCodeAndDecsriptionResponse commonResponse = userTask.GetRoleCodes();
            return Ok(commonResponse);
        }

        [Route("getregioncodes")]
        [HttpGet]
        public IHttpActionResult GetRegionCodes()
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            CommonResponse commonResponse = userTask.GetRegionCodes();
            return Ok(commonResponse);
        }

        #endregion

        [Route("dashboard")]
        [HttpPost]
        public IHttpActionResult GetUserDashboard(UserRequest user)
        {
            if (user.Requests.Count <= 0 || user.Requests[0].ID <= 0)
            {
                return BadRequest(DomainObjects.Resource.ResourceData.InvalidUserID);
            }
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            DashboardResponse userDashboard = userTask.GetUserDashboard(user);
            return Ok(userDashboard);
        }

        [Route("getusermenus")]
        [HttpGet]
        public IHttpActionResult GetUserMenus(int userId)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            RoleResponse roleResponse = userTask.GetUserMenus(userId);
            return Ok(roleResponse);
        }

    }
}
