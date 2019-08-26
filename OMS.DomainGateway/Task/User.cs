using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Task
{

    public abstract class UserTask : IUserTask
    {
        public abstract UserResponse LoginUser(LoginRequest login);
        public abstract UserResponse LoginUser(string key);

        // User Application
        public abstract UserResponse CreateUpdateUser(UserRequest user);
        public abstract UserResponse DeleteUser(int UserID);
        public abstract UserResponse GetUsers(UserRequest userReq);
        public abstract UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest);

        // Role Management
        public abstract RoleResponse CreateUpdateRole(RoleRequest role);
        public abstract RoleResponse DeleteRole(int Id);
        public abstract RoleResponse GetRoles(RoleRequest roles);
        public abstract RoleResponse GetRoleDetails(int roleId);

        // User Role
        public abstract UserRoleResponse CreateUpdateUserRole(UserRoleRequest assignUserRoleRequest);
        public abstract UserResponse DeleteUserRole(int userRoleID);
        public abstract UserRoleResponse GetUserRoles(UserRoleRequest userRoleRequest); 

        // Master Data Operations
        public abstract RegionResponse GetRegions(RegionRequest regions);
        public abstract RoleMenuResponse GetMenuWithActivities();
        public abstract ApplicationResponse GetApplications();
        public abstract CommonResponse GetUserNames();
        public abstract CommonResponse GetRoleCodes();
        public abstract CommonResponse GetRegionCodes();
        public abstract string GetUserNameFromToken(string token);
        public abstract string AuthenticateUser(SAMATokenRequest request);
    }
}
