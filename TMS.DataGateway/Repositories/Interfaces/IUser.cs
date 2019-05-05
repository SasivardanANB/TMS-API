using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = TMS.DomainObjects.Objects;

namespace TMS.DataGateway.Repositories.Iterfaces
{
    public interface IUser
    {
        UserResponse LoginUser(LoginRequest login);

        UserResponse DeleteUser(int userId);
        RoleResponse CreateUpdateRole(RoleRequest role);
        RoleResponse DeleteRole(int id);
        UserResponse GetUsers(UserRequest userReq);
        RegionResponse GetRegions(RegionRequest regions);
        RoleResponse GetRoles(RoleRequest roles);
        UserRoleResponse CreateUpdateUserRole(UserRoleRequest userRoleRequest);
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
