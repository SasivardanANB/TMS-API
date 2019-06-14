using TMS.DataGateway.Repositories;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BusinessGateway.Task
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
            //If needed write business logic here for request.

            UserResponse userData = _userRepository.CreateUpdateUser(user);

            //If needed write business logic here for response.
            return userData;
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
