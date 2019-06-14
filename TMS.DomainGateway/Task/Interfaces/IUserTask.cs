using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IUserTask
    {
        UserResponse LoginUser(LoginRequest login);

        // User Application
        UserResponse CreateUpdateUser(UserRequest user);
        UserResponse DeleteUser(int userID);
        UserResponse GetUsers(UserRequest userReq);
        UserResponse UpdateUserProfile(UserRequest user);

        // Role Management
        RoleResponse CreateUpdateRole(RoleRequest role);
        RoleResponse DeleteRole(int roleID);
        RoleResponse GetRoles(RoleRequest roles);
        RoleResponse GetRoleDetails(int roleId);

        // User Role
        UserRoleResponse CreateUpdateUserRole(UserRoleRequest assignUserRoleRequest);
        UserResponse DeleteUserRole(int userRoleID);
        UserRoleResponse GetUserRoles(UserRoleRequest userRoleRequest);

        // Master Data Operations
        RegionResponse GetRegions(RegionRequest regions);
        RoleMenuResponse GetMenuWithActivities();
        ApplicationResponse GetApplications();
        CommonResponse GetUserNames();
        CommonResponse GetRoleCodes();
        CommonResponse GetRegionCodes();
        DashboardResponse GetUserDashboard(UserRequest user);
        RoleResponse GetUserMenus(int userId);
    }
}
