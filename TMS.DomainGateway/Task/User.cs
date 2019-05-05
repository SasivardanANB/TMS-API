using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainGateway.Task
{
    
    public abstract class UserTask : IUserTask
    {
        public abstract UserResponse LoginUser(LoginRequest login);
        public abstract UserResponse DeleteUser(int UserID);
        public abstract RoleResponse CreateUpdateRole(RoleRequest role);
        public abstract RoleResponse DeleteRole(int Id);
        public abstract UserResponse GetUsers(UserRequest userReq);
        public abstract RegionResponse GetRegions(RegionRequest regions);
        public abstract RoleResponse GetRoles(RoleRequest roles);
        public abstract UserRoleResponse CreateUpdateUserRole(UserRoleRequest assignUserRoleRequest);
        public abstract UserResponse DeleteUserRole(int userRoleID);
        public abstract RoleMenuResponse GetMenuWithActivities();
        public abstract UserResponse CreateUpdateUser(UserRequest user);
        public abstract ApplicationResponse GetApplications();
        public abstract RoleResponse GetRoleDetails(int roleId);
        public abstract CommonResponse GetUserNames();
        public abstract CommonResponse GetRoleCodes();
        public abstract CommonResponse GetRegionCodes();
        public abstract DashboardResponse GetUserDashboard(UserRequest user);
    }
}
