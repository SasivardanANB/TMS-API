using OMS.DomainGateway.Gateway;
using OMS.DomainGateway.Task;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using OMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Helper.Model.DependencyResolver;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainGateway.Gateway.Interfaces;
using OMS.API.Classes;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using NLog;

namespace OMS.API.Controllers
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
        [HttpPost]
        public IHttpActionResult GetSAMAUser(string token)
        {
            SAMAUserResponse samaUserResponse = new SAMAUserResponse();

            if (String.IsNullOrEmpty(token))
                return BadRequest(ModelState);

            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["SAMAApiGatewayBaseURL"]);
                RestRequest request = new RestRequest("/users/getmyuserinfo", Method.GET) { RequestFormat = DataFormat.Json };
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
            for (int i = 0; i < user.Requests.Count; i++)
            {
                if (user.Requests[i].Applications.Count > 0)
                {
                    foreach (var application in user.Requests[i].Applications)
                    {
                        if (application == 0)
                        {
                            ModelState.AddModelError($"{nameof(user)}.{nameof(user.Requests)}.[{i}].{nameof(OMS.DomainObjects.Objects.User.Applications)}", DomainObjects.Resource.ResourceData.InvalidApplication);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError($"{nameof(user)}.{nameof(user.Requests)}.[{i}].{nameof(OMS.DomainObjects.Objects.User.Applications)}", DomainObjects.Resource.ResourceData.InvalidApplication);
                }
                if (user.Requests[i].ID > 0)
                {
                    ModelState.Remove("user.Requests[" + i + "].Password");
                    ModelState.Remove("user.Requests[" + i + "].ConfirmPassword");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            UserResponse userResponse = new UserResponse();

            //Create new request object to hold status
            UserRequest tmsRequest = new UserRequest()
            {
                Requests = new List<User>() {
                    new User(){
                        Applications = new List<int>(){
                            2
                        },
                        FirstName = user.Requests[0].FirstName,
                        LastName = user.Requests[0].LastName,
                        UserName = user.Requests[0].UserName,
                        Password = user.Requests[0].Password,
                        ConfirmPassword = user.Requests[0].ConfirmPassword
                    }
                },
                CreatedBy = "SYSTEM",
                LastModifiedBy = user.LastModifiedBy,
                CreatedTime = user.CreatedTime,
                LastModifiedTime = user.LastModifiedTime
            };

            UserRequest dmsRequest = new UserRequest()
            {
                Requests = new List<User>()
                        {
                            new User()
                            {
                                FirstName = user.Requests[0].FirstName,
                                LastName = user.Requests[0].LastName,
                                UserName = user.Requests[0].UserName,
                                Password = user.Requests[0].Password,
                                IsActive = true
                            }
                        },
                CreatedBy = "SYSTEM",
                LastModifiedBy = user.LastModifiedBy,
                CreatedTime = user.CreatedTime,
                LastModifiedTime = user.LastModifiedTime
            };

            foreach (var application in user.Requests[0].Applications)
            {
                if (application == 1)
                {
                    IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
                    userResponse = userTask.CreateUpdateUser(user);
                }

                LoginRequest loginRequest = new LoginRequest();
                string token = "";
                if (application == 2) //For TMS Application - Integrate Azure API Gateway
                {
                    //Login to TMS and get Token
                    loginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
                    var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                    {
                        token = tmsLoginResponse.TokenKey;
                    }

                    userResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                        + "/v1/user/createupdateuser", Method.POST, tmsRequest, token));
                }

                if (application == 3) //For DMS Application - Integrate Azure API Gateway
                {
                    //Login to DMS and get Token
                    loginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
                    var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                    {
                        token = dmsLoginResponse.TokenKey;
                    }

                    userResponse = JsonConvert.DeserializeObject<UserResponse>(GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "/v1/user/createupdateuser", Method.POST, dmsRequest, token));
                }
            }

            return Ok(userResponse);
        }

        [Route("deleteuser")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(int userID)
        {
            if (userID == 0)
            {
                ModelState.AddModelError($"{nameof(userID)}.{nameof(userID)}.[{userID}].{nameof(OMS.DomainObjects.Objects.User.ID)}", DomainObjects.Resource.ResourceData.InvalidUserID);
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
                ModelState.AddModelError($"{nameof(roleID)}.{nameof(roleID)}.[{roleID}].{nameof(OMS.DomainObjects.Objects.User.ID)}", DomainObjects.Resource.ResourceData.InvalidUserID);
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
            CommonResponse commonResponse = userTask.GetRoleCodes();
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
    }
}
