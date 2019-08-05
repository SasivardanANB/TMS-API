using Newtonsoft.Json;
using OMS.BusinessGateway.Classes;
using OMS.DataGateway.Repositories;
using OMS.DataGateway.Repositories.Iterfaces;
using OMS.DomainGateway.Task;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Domain = OMS.DomainObjects.Objects;

namespace OMS.BusinessGateway.Task
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
            //If needed write business logic here for request.

            UserResponse userData = _userRepository.LoginUser(login);

            //If needed write business logic here for response.
            return userData;
        }

        #region "User Application"

        public override UserResponse CreateUpdateUser(UserRequest user)
        {
            UserResponse userResponse = new UserResponse();

            foreach (Domain.User userDetails in user.Requests)
            {
                #region Create TMSUserRequest
                UserRequest tmsRequest = null;
                if (userDetails.Applications.Contains(2))
                {
                    tmsRequest = new UserRequest()
                    {
                        Requests = new List<Domain.User>()
                        {
                            new Domain.User()
                            {
                                Applications = new List<int>(){
                                    2
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

                #region CreateUpdateUser in OMS
                if (userDetails.Applications.Contains(1))
                {
                    userResponse = _userRepository.CreateUpdateUser(user);
                }
                #endregion

                #region CreateUpdateUser in TMS
                if (tmsRequest != null) //For TMS Application - Integrate Azure API Gateway
                {
                    LoginRequest loginRequest = new LoginRequest();
                    //Login to TMS and get Token
                    string token = string.Empty;
                    loginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
                    loginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
                    var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                        + "/v1/user/login", Method.POST, loginRequest, null));
                    if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                    {
                        token = tmsLoginResponse.TokenKey;
                    }

                    UserResponse tmsUserResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                         + "/v1/user/createupdateuser", Method.POST, tmsRequest, token));

                    if (!userDetails.Applications.Contains(1))
                    {
                        userResponse = tmsUserResponse;
                    }
                    else
                    {
                        userResponse.StatusMessage = userResponse.StatusMessage + ". " + tmsUserResponse.StatusMessage;
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

        public override UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            UserResponse userResponse = _userRepository.ChangePassword(changePasswordRequest);
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

        public override CommonResponse GetRoleCodes()
        {
            CommonResponse commonResponse = _userRepository.GetRoleCodes();
            return commonResponse;
        }

        public override CommonResponse GetRegionCodes()
        {
            CommonResponse commonResponse = _userRepository.GetRegionCodes();
            return commonResponse;
        }

        #endregion
    }
}
