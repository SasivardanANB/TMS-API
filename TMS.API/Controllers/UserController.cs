using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using TMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.API.Classes;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;
using NLog;
using System.Net.Mail;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/user")]
    public class UserController : ApiController
    {
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

        [Route("samalogin")]
        [AllowAnonymous, HttpGet]
        public IHttpActionResult Login(string key)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userData = userTask.LoginUser(key);
            return Ok(userData);
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

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.CreateUpdateUser(user);

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
