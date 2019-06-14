using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = OMS.DomainObjects.Objects;

namespace OMS.DataGateway.Repositories.Iterfaces
{
    public interface IUser
    {
        UserResponse LoginUser(LoginRequest login);

        // User Application
        UserResponse CreateUpdateUser(UserRequest user);
        UserResponse DeleteUser(int userId);
        UserResponse GetUsers(UserRequest userReq);
        UserResponse UpdateUserProfile(UserRequest user);

        // Role Management
        RoleResponse CreateUpdateRole(RoleRequest role);
        RoleResponse DeleteRole(int id);
        RoleResponse GetRoles(RoleRequest roles);
        RoleResponse GetRoleDetails(int roleId);

        // User Role
        UserRoleResponse CreateUpdateUserRole(UserRoleRequest userRoleRequest);
        UserResponse DeleteUserRole(int userRoleID);
        UserRoleResponse GetUserRoles(UserRoleRequest userRoleRequest);

        // Master Data Operations
        RegionResponse GetRegions(RegionRequest regions);
        RoleMenuResponse GetMenuWithActivities();
        ApplicationResponse GetApplications();
        CommonResponse GetUserNames();
        CommonResponse GetRoleCodes();
        CommonResponse GetRegionCodes();
    }
}
