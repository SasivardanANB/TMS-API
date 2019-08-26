using TMS.DataGateway.Repositories;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System.Collections.Generic;
using System.Linq;
using Domain = TMS.DomainObjects.Objects;
using System.Configuration;
using Newtonsoft.Json;
using RestSharp;
using TMS.BusinessGateway.Classes;
using System;
using System.Net;
using System.Net.Http;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessUserTask : UserTask
    {
        private readonly IUser _userRepository;

        public const int OMS = 1;
        public const int TMS = 1;

        public BusinessUserTask(IUser userRepository)
        {
            _userRepository = userRepository;
        }

        public override UserResponse LoginUser(LoginRequest login)
        {
            UserResponse userData = _userRepository.LoginUser(login);
            return userData;
        }

        public override UserResponse LoginUser(string key)
        {
            var userName = GetUserNameFromToken(key);

            if (String.IsNullOrEmpty(userName))
            {
                UserResponse userResponse = new UserResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Status = DomainObjects.Resource.ResourceData.Failure,
                    StatusMessage = DomainObjects.Resource.ResourceData.UnAuthorized
                };
                return userResponse;
            }
            else
            {
                LoginRequest loginRequest = new LoginRequest
                {
                    UserName = userName,
                    UserPassword = string.Empty,
                    IsSAMALogin = true
                };

                var TmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                       + "/v1/user/login", Method.POST, loginRequest, null));

                return TmsLoginResponse;
            }
        }

        #region "User Application"
        public override UserResponse CreateUpdateUser(UserRequest user)
        {
            UserResponse userResponse = new UserResponse();

            foreach (Domain.User userDetails in user.Requests)
            {
                #region Create OMSUserRequest
                UserRequest omsRequest = null;
                if (userDetails.Applications.Contains(1))
                {
                    omsRequest = new UserRequest()
                    {
                        Requests = new List<Domain.User>()
                        {
                            new Domain.User()
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

                #region CreateUpdateUser in TMS
                if (userDetails.Applications.Contains(2))
                {
                    userResponse = _userRepository.CreateUpdateUser(user);
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
                    var OmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (OmsLoginResponse != null && OmsLoginResponse.Data.Count > 0)
                    {
                        token = OmsLoginResponse.TokenKey;
                    }

                    UserResponse omsUserResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
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
            }

            return userResponse;
        }

        public override UserResponse DeleteUser(int UserID)
        {
            UserResponse userData = _userRepository.DeleteUser(UserID);

            return userData;
        }

        public override UserResponse GetUsers(UserRequest userReq)
        {
            UserResponse usersList = _userRepository.GetUsers(userReq);
            return usersList;
        }

        public override UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest,string type)
        {
            UserResponse userResponse = _userRepository.ChangePassword(changePasswordRequest,type);
            return userResponse;
        }

        public override UserResponse ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            UserResponse userResponse = _userRepository.ForgotPassword(forgotPasswordRequest);
            return userResponse;
        }

        #endregion

        #region "Role Management"

        public override RoleResponse CreateUpdateRole(RoleRequest role)
        {
            RoleResponse roleResponse = _userRepository.CreateUpdateRole(role);
            return roleResponse;
        }

        public override RoleResponse DeleteRole(int Id)
        {
            RoleResponse roleResponse = _userRepository.DeleteRole(Id);
            return roleResponse;
        }

        public override RoleResponse GetRoles(RoleRequest roles)
        {
            RoleResponse rolesList = _userRepository.GetRoles(roles);
            return rolesList;
        }

        public override RoleResponse GetRoleDetails(int roleId)
        {
            RoleResponse roleResponse = _userRepository.GetRoleDetails(roleId);
            return roleResponse;
        }

        #endregion

        #region "User Role"

        public override UserRoleResponse CreateUpdateUserRole(UserRoleRequest userRoleRequest)
        {
            UserRoleResponse userRoleResponse = _userRepository.CreateUpdateUserRole(userRoleRequest);
            return userRoleResponse;
        }

        public override UserResponse DeleteUserRole(int userRoleID)
        {
            UserResponse userData = _userRepository.DeleteUserRole(userRoleID);
            return userData;
        }

        public override UserRoleResponse GetUserRoles(UserRoleRequest userRoleRequest)
        {
            UserRoleResponse userRoleResponse = _userRepository.GetUserRoles(userRoleRequest);
            return userRoleResponse;
        }

        #endregion

        #region "Master Data Operations"

        public override RegionResponse GetRegions(RegionRequest regions)
        {
            RegionResponse regionsList = _userRepository.GetRegions(regions);
            return regionsList;
        }

        public override RoleMenuResponse GetMenuWithActivities()
        {
            RoleMenuResponse roleMenuResponse = _userRepository.GetMenuWithActivities();
            return roleMenuResponse;
        }

        public override ApplicationResponse GetApplications()
        {
            ApplicationResponse applicationResponse = _userRepository.GetApplications();
            return applicationResponse;
        }

        public override CommonResponse GetUserNames()
        {
            CommonResponse commonResponse = _userRepository.GetUserNames();
            return commonResponse;
        }

        public override CommonCodeAndDecsriptionResponse GetRoleCodes()
        {
            CommonCodeAndDecsriptionResponse commonResponse = _userRepository.GetRoleCodes();
            return commonResponse;
        }

        public override CommonResponse GetRegionCodes()
        {
            CommonResponse commonResponse = _userRepository.GetRegionCodes();
            return commonResponse;
        }

        public override string GetUserNameFromToken(string token)
        {
            return _userRepository.GetUserNameFromToken(token);
        }

        public override string AuthenticateUser(SAMATokenRequest request)
        {
            var access_token = request.Key;
            if (string.IsNullOrEmpty(access_token))
            {
                return string.Empty;
            }

            string result = string.Empty;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", access_token));
            var response = client.GetAsync(new Uri(ConfigurationManager.AppSettings["SAMA_ACCOUNT_URL"])).Result;
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                var samaResult = response.Content.ReadAsStringAsync().Result;
                var samaResponse = JsonConvert.DeserializeObject<SAMATokenResponse>(samaResult);
                if (samaResponse != null)
                {
                    //Login to OMS and get Token

                    LoginRequest loginRequest = new LoginRequest
                    {
                        UserName = samaResponse.UserName,
                        UserPassword = string.Empty,
                        IsSAMALogin = true
                    };

                    var TmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));

                    string token = string.Empty;
                    if (TmsLoginResponse != null && TmsLoginResponse.Data.Count > 0)
                    {
                        token = TmsLoginResponse.TokenKey;
                    }

                    var LandingLoginURL = ConfigurationManager.AppSettings["TMS_UI_URL"];
                    result = LandingLoginURL + token;
                }
            }
            return result;
        }

        #endregion

        public override DashboardResponse GetUserDashboard(UserRequest user)
        {
            DashboardResponse userDashboard = _userRepository.GetUserDashboard(user);
            return userDashboard;
        }

        public override RoleResponse GetUserMenus(int userId)
        {
            RoleResponse roleResponse = _userRepository.GetUserMenus(userId);
            return roleResponse;
        }       
    }
}
