﻿using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainGateway.Task.Interfaces
{
    public interface IUserTask
    {
        UserResponse LoginUser(LoginRequest login);
        UserResponse LoginUser(string key);

        // User Application
        UserResponse CreateUpdateUser(UserRequest user);
        UserResponse DeleteUser(int userID);
        UserResponse GetUsers(UserRequest userReq);
        UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest);

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
        string GetUserNameFromToken(string token);
        string AuthenticateUser(SAMATokenRequest request);
    }
}
