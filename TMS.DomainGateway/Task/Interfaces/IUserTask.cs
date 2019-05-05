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
        UserResponse DeleteUser(int UserID);
        RoleResponse CreateUpdateRole(RoleRequest role);
        RoleResponse DeleteRole(int Id);
        UserResponse GetUsers(UserRequest userReq);
        RegionResponse GetRegions(RegionRequest regions);
        RoleResponse GetRoles(RoleRequest roles);
        UserRoleResponse CreateUpdateUserRole(UserRoleRequest assignUserRoleRequest);
        UserResponse DeleteUserRole(int userRoleID);
        RoleMenuResponse GetMenuWithActivities();
        UserResponse CreateUpdateUser(UserRequest user);
        ApplicationResponse GetApplications();
        RoleResponse GetRoleDetails(int roleId);
        CommonResponse GetUserNames();
        CommonResponse GetRoleCodes();
        CommonResponse GetRegionCodes();
        DashboardResponse GetUserDashboard(UserRequest user);
    }
}
