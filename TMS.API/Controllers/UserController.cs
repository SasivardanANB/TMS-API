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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userData = userTask.LoginUser(login);
            return Ok(userData);
        }

        #region "User Application"

        [Route("createupdateuser")]
        [AllowAnonymous, HttpPost]
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
                            ModelState.AddModelError($"{nameof(user)}.{nameof(user.Requests)}.[{i}].{nameof(TMS.DomainObjects.Objects.User.Applications)}", DomainObjects.Resource.ResourceData.InvalidApplication);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError($"{nameof(user)}.{nameof(user.Requests)}.[{i}].{nameof(TMS.DomainObjects.Objects.User.Applications)}", DomainObjects.Resource.ResourceData.InvalidApplication);
                }

            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
            ModelState.Remove("user.Requests[0].UserName");
            ModelState.Remove("user.Requests[0].Password");
            ModelState.Remove("user.Requests[0].FirstName");
            ModelState.Remove("user.Requests[0].LastName");

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

        [Route("dashboard")]
        [HttpPost]
        public IHttpActionResult GetUserDashboard(UserRequest user)
        {
            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            DashboardResponse userDashboard = userTask.GetUserDashboard(user);
            return Ok(userDashboard);
        }
    }
}
