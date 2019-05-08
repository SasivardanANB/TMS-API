using AutoMapper;
using NLog;
using OMS.DataGateway.Repositories.Iterfaces;
using OMS.DataGateway.DataModels;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataModel = OMS.DataGateway.DataModels;
using Domain = OMS.DomainObjects.Objects;
using System.Configuration;
using System.Data.Common;

namespace OMS.DataGateway.Repositories
{

    public class User : IUser
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserResponse LoginUser(LoginRequest login)
        {
            UserResponse userResponse = new UserResponse()
            {
                Data = new List<Domain.User>()
            };
            try
            {
                using (var context = new OMSDBContext())
                {
                    string encryptedPassword = Encryption.EncryptionLibrary.EncryptPassword(login.UserPassword);
                    var userData = (from user in context.Users
                                    where user.UserName == login.UserName
                                    && user.Password == encryptedPassword && !user.IsDelete
                                    select new Domain.User()
                                    {
                                        ID = user.ID,
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        IsActive = user.IsActive,
                                        UserName = user.UserName
                                    }).FirstOrDefault();

                    if (userData != null)
                    {
                        if (!userData.IsActive)
                        {
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserInActive;
                        }
                        else
                        {

                            userResponse.Data.Add(userData);

                            //Creating token for this user
                            Authenticate authenticate = new Authenticate();
                            string token = authenticate.GenerateToken(userData, DateTime.Now);

                            //Inserting token for this user in database
                            authenticate.InsertToken(new Domain.Authenticate()
                            {
                                TokenKey = token,
                                CreatedOn = DateTime.Now,
                                IssuedOn = DateTime.Now,
                                ExpiresOn = DateTime.Now.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiry"])),
                                UserID = userData.ID
                            });

                            //Get Token and add to response
                            var tokenData = (from tokens in context.TokensManagers
                                             where tokens.UserID == userData.ID
                                             //&&  tokens.TokenKey == token 
                                             select new Domain.Authenticate()
                                             {
                                                 UserID = tokens.UserID,
                                                 TokenKey = tokens.TokenKey,
                                                 CreatedOn = tokens.CreatedOn,
                                                 IssuedOn = tokens.IssuedOn,
                                                 ExpiresOn = tokens.ExpiresOn,
                                             }).FirstOrDefault();


                            userResponse.TokenKey = tokenData.TokenKey;
                            userResponse.TokenIssuedOn = tokenData.IssuedOn;
                            userResponse.TokenExpiresOn = tokenData.ExpiresOn;

                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.LoginSuccess;
                        }
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.LoginFail;
                    }
                }
            }
            catch (DbException ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }

        #region "User Application"

        public UserResponse CreateUpdateUser(UserRequest user)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    var userDataModelList = new List<DataModel.User>();
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.User, DataModel.User>().ReverseMap()
                        .ForMember(x => x.ApplicationNames, opt => opt.Ignore());
                    });

                    IMapper mapper = config.CreateMapper();

                    //Encrypt Password
                    foreach (var userData in user.Requests)
                    {
                       
                        var userDataModel = mapper.Map<Domain.User, DataModel.User>(userData);

                        if (!string.IsNullOrEmpty(userDataModel.Password))
                        {
                            userDataModel.Password = Encryption.EncryptionLibrary.EncryptPassword(userDataModel.Password);
                        }
                        if (userDataModel.ID > 0) //Update User
                        {
                            userDataModel.LastModifiedBy = user.LastModifiedBy;
                            userDataModel.LastModifiedTime = DateTime.Now;
                            context.Entry(userDataModel).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            var existedApplications = (from userApplication in context.UserApplications
                                                       where userApplication.UserID == userDataModel.ID
                                                       select userApplication).ToList();
                            if (existedApplications != null)
                            {
                                context.UserApplications.RemoveRange(existedApplications);
                                context.SaveChanges();
                            }

                            if (userData.Applications.Count > 0)
                            {
                                foreach (var applicationID in userData.Applications)
                                {
                                    var userApplication = new UserApplication();
                                    userApplication.UserID = userData.ID;
                                    userApplication.ApplicationID = applicationID;
                                    userApplication.CreatedBy = user.CreatedBy;

                                    context.UserApplications.Add(userApplication);
                                    context.SaveChanges();
                                }
                            }
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersUpdated;
                        }
                        else //Create User
                        {
                            userDataModel.CreatedBy = user.CreatedBy;
                            userDataModel.CreatedTime = DateTime.Now;
                            userDataModel.IsActive = true;
                            userDataModel.LastModifiedBy = "";
                            userDataModel.LastModifiedTime = null;
                            context.Users.Add(userDataModel);
                            context.SaveChanges();

                            if (userData.Applications.Count > 0)
                            {
                                foreach (var applicationID in userData.Applications)
                                {
                                    var userApplication = new UserApplication();
                                    userApplication.UserID = userDataModel.ID;
                                    userApplication.ApplicationID = applicationID;
                                    userApplication.CreatedBy = user.CreatedBy;

                                    context.UserApplications.Add(userApplication);
                                    context.SaveChanges();
                                }
                            }

                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersCreated;
                        }

                        userDataModelList.Add(userDataModel);
                    }

                    user.Requests = mapper.Map<List<DataModel.User>, List<Domain.User>>(userDataModelList);
                    userResponse.Data = user.Requests;
                    userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    userResponse.StatusCode = (int)HttpStatusCode.OK;

                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return userResponse;
        }

        public UserResponse DeleteUser(int userId)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    if (userId > 0)
                    {
                        var user = context.Users.Where(x => x.ID == userId).FirstOrDefault();
                        if (user != null)
                        {
                            user.IsDelete = true;
                            context.SaveChanges();
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersDeleted;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUser;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return userResponse;
        }

        public UserResponse GetUsers(UserRequest userReq)
        {
            UserResponse userResponse = new UserResponse();
            List<Domain.User> usersList = new List<Domain.User>();
            List<DataModels.User> users = new List<DataModels.User>();
            try
            {
                using (var context = new OMSDBContext())
                {
                    usersList =
                        (from user in context.Users
                         where !user.IsDelete
                         select new Domain.User
                         {
                             ID = user.ID,
                             UserName = user.UserName,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             IsActive = user.IsActive,
                             Applications = context.UserApplications.Where(userApp => userApp.UserID == user.ID).Select(userApp => userApp.ApplicationID).ToList(),
                             ApplicationNames = context.Applications.Where(a => (context.UserApplications.Where(userApp => userApp.UserID == user.ID).Select(userApp => userApp.ApplicationID).ToList()).Contains(a.ID)).Select(a => a.ApplicationName).ToList(),
                             Roles = context.Roles.Where(r => (context.UserRoles.Where(ur => ur.UserID == user.ID).Select(l => l.ID).ToList()).Contains(r.ID)).Select(fe => new Domain.Role
                             {
                                 ID = fe.ID,
                                 RoleCode = fe.RoleCode,
                                 RoleDescription = fe.RoleDescription
                             }).ToList(),
                             Regions = context.BusinessAreas.Where(r => (context.UserRoles.Where(ur => ur.UserID == user.ID).Select(l => l.ID).ToList()).Contains(r.ID)).Select(fe => new Domain.Region
                             {
                                 ID = fe.ID,
                                 BusinessAreaCode = fe.BusinessAreaCode,
                                 BusinessAreaDescription = fe.BusinessAreaDescription
                             }).ToList(),
                         }).ToList();
                }

                // Filter
                if (userReq.Requests.Count > 0)
                {
                    var userFilter = userReq.Requests[0];

                    if (userFilter.ID > 0)
                    {
                        usersList = usersList.Where(s => s.ID == userFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(userFilter.UserName))
                    {
                        usersList = usersList.Where(s => s.UserName.Contains(userFilter.UserName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(userFilter.FirstName))
                    {
                        usersList = usersList.Where(s => s.FirstName.Contains(userFilter.FirstName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(userFilter.LastName))
                    {
                        usersList = usersList.Where(s => s.LastName.Contains(userFilter.LastName)).ToList();
                    }

                    //if (userFilter.IsActive != null)
                    //{
                    //    usersList = usersList.Where(s => s.IsActive == userFilter.IsActive).ToList();
                    //}
                }

                // Sorting
                if (userReq.SortOrder != null)
                {
                    switch (userReq.SortOrder.ToLower())
                    {
                        case "username":
                            usersList = usersList.OrderBy(s => s.UserName).ToList();
                            break;
                        case "username_desc":
                            usersList = usersList.OrderByDescending(s => s.UserName).ToList();
                            break;
                        case "firstname":
                            usersList = usersList.OrderBy(s => s.FirstName).ToList();
                            break;
                        case "firstname_desc":
                            usersList = usersList.OrderByDescending(s => s.FirstName).ToList();
                            break;
                        case "lastname":
                            usersList = usersList.OrderBy(s => s.LastName).ToList();
                            break;
                        case "lastname_desc":
                            usersList = usersList.OrderByDescending(s => s.LastName).ToList();
                            break;
                        default:  // ID Descending 
                            usersList = usersList.OrderByDescending(s => s.ID).ToList();
                            break;
                    }
                }                

                // Total NumberOfRecords
                userResponse.NumberOfRecords = usersList.Count;

                // Paging
                int pageNumber = (userReq.PageNumber ?? 1);
                int pageSize = Convert.ToInt32(userReq.PageSize);
                if (pageSize > 0)
                {
                    usersList = usersList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                if (usersList.Count > 0)
                {
                    userResponse.Data = usersList;
                    userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    userResponse.StatusCode = (int)HttpStatusCode.OK;
                    userResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    userResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    userResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return userResponse;
        }

        #endregion

        #region "Role Management"

        public RoleResponse CreateUpdateRole(RoleRequest role)
        {
            RoleResponse roleResponse = new RoleResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Role, DataModel.Role>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var roles = mapper.Map<List<Domain.Role>, List<DataModel.Role>>(role.Requests);

                    if (role.Requests[0].ID > 0)
                    {
                        var roleDetails = context.Roles.Find(role.Requests[0].ID);
                        if (roleDetails != null)
                        {
                            int roleId = role.Requests[0].ID;
                            roleDetails.RoleCode = role.Requests[0].RoleCode;
                            roleDetails.RoleDescription = role.Requests[0].RoleDescription;
                            roleDetails.ValidFrom = role.Requests[0].ValidFrom;
                            roleDetails.ValidTo = role.Requests[0].ValidTo;
                            context.Entry(roleDetails).State = System.Data.Entity.EntityState.Modified;
                            roleDetails.LastModifiedBy = role.LastModifiedBy;
                            roleDetails.LastModifiedTime = DateTime.Now;
                            context.SaveChanges();
                            var existedRoleMenu = (from roleMenu in context.RoleMenus
                                                   where roleMenu.RoleID == roleId
                                                   select roleMenu).ToList();
                            if (existedRoleMenu != null)
                            {
                                foreach (var roleMenu in existedRoleMenu)
                                {
                                    var roleMenuActivities = (from roleMenuActivity in context.RoleMenuActivity
                                                              where roleMenuActivity.RoleMenuID == roleMenu.ID
                                                              select roleMenuActivity).ToList();
                                    if (roleMenuActivities != null)
                                    {
                                        context.RoleMenuActivity.RemoveRange(roleMenuActivities);
                                        context.SaveChanges();
                                    }
                                }
                                context.RoleMenus.RemoveRange(existedRoleMenu);
                                context.SaveChanges();
                            }

                            foreach (var menuItem in role.Requests[0].RoleMenus)
                            {
                                Domain.RoleMenu roleMenuObj = new Domain.RoleMenu()
                                {
                                    ID = menuItem.ID,
                                };
                                var configmenu = new MapperConfiguration(cfg =>
                                {
                                    cfg.CreateMap<Domain.RoleMenu, DataModel.RoleMenu>().ReverseMap()
                                    .ForMember(x => x.MenuCode, opt => opt.Ignore())
                                    .ForMember(x => x.MenuDescription, opt => opt.Ignore())
                                    .ForMember(x => x.MenuURL, opt => opt.Ignore())
                                    .ForMember(x => x.RoleMenuActivities, opt => opt.Ignore());
                                });
                                IMapper mappermenu = configmenu.CreateMapper();
                                var roleMenu = mappermenu.Map<Domain.RoleMenu, DataModel.RoleMenu>(roleMenuObj);
                                roleMenu.RoleID = role.Requests[0].ID;
                                roleMenu.CreatedBy = role.CreatedBy;
                                roleMenu.CreatedTime = DateTime.Now;
                                roleMenu.LastModifiedTime = null;
                                roleMenu.MenuID = menuItem.ID;
                                context.RoleMenus.Add(roleMenu);
                                context.SaveChanges();
                                foreach (var menuactivity in menuItem.RoleMenuActivities)
                                {
                                    Domain.RoleMenuActivity roleMenuActivityObj = new Domain.RoleMenuActivity()
                                    {
                                        ID = menuactivity.ID
                                    };
                                    var configActivity = new MapperConfiguration(cfg =>
                                    {
                                        cfg.CreateMap<Domain.RoleMenuActivity, DataModel.RoleMenuActivity>().ReverseMap()
                                        .ForMember(x => x.ActivityCode, opt => opt.Ignore())
                                        .ForMember(x => x.ActivityDescription, opt => opt.Ignore());
                                    });
                                    IMapper mapperActivity = configActivity.CreateMapper();
                                    var roleact = mapperActivity.Map<Domain.RoleMenuActivity, DataModel.RoleMenuActivity>(roleMenuActivityObj);
                                    roleact.ActivityID = menuactivity.ID;
                                    roleact.RoleMenuID = roleMenu.ID;
                                    roleact.CreatedBy = role.CreatedBy;
                                    roleact.CreatedTime = DateTime.Now;
                                    roleact.LastModifiedTime = null;
                                    context.RoleMenuActivity.Add(roleact);
                                    context.SaveChanges();
                                }
                            }
                            roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RolesUpdated;
                        }
                    }
                    else
                    {
                        foreach (var item in role.Requests)
                        {
                            var roleObject = mapper.Map<Domain.Role, DataModel.Role>(item);
                            roleObject.IsActive = true;
                            roleObject.CreatedBy = role.CreatedBy;
                            roleObject.CreatedTime = DateTime.Now;
                            roleObject.LastModifiedTime = null;
                            context.Roles.Add(roleObject);
                            context.SaveChanges();
                            foreach (var menuItem in item.RoleMenus)
                            {
                                Domain.RoleMenu roleMenuObj = new Domain.RoleMenu()
                                {
                                    ID = menuItem.ID,
                                };
                                var configmenu = new MapperConfiguration(cfg =>
                                {
                                    cfg.CreateMap<Domain.RoleMenu, DataModel.RoleMenu>().ReverseMap()
                                    .ForMember(x => x.MenuCode, opt => opt.Ignore())
                                    .ForMember(x => x.MenuDescription, opt => opt.Ignore())
                                    .ForMember(x => x.MenuURL, opt => opt.Ignore())
                                    .ForMember(x => x.RoleMenuActivities, opt => opt.Ignore());
                                });
                                IMapper mappermenu = configmenu.CreateMapper();
                                var roleMenu = mappermenu.Map<Domain.RoleMenu, DataModel.RoleMenu>(roleMenuObj);
                                roleMenu.RoleID = roleObject.ID;
                                roleMenu.MenuID = menuItem.ID;
                                roleMenu.CreatedBy = role.CreatedBy;
                                roleMenu.CreatedTime = DateTime.Now;
                                roleMenu.LastModifiedTime = null;
                                context.RoleMenus.Add(roleMenu);
                                context.SaveChanges();
                                foreach (var menuactivity in menuItem.RoleMenuActivities)
                                {

                                    Domain.RoleMenuActivity roleMenuActivityObj = new Domain.RoleMenuActivity()
                                    {
                                        ID = menuactivity.ID
                                    };
                                    var configActivity = new MapperConfiguration(cfg =>
                                    {
                                        cfg.CreateMap<Domain.RoleMenuActivity, DataModel.RoleMenuActivity>().ReverseMap()
                                        .ForMember(x => x.ActivityCode, opt => opt.Ignore())
                                        .ForMember(x => x.ActivityDescription, opt => opt.Ignore());
                                    });

                                    IMapper mapperActivity = configActivity.CreateMapper();
                                    var roleact = mapperActivity.Map<Domain.RoleMenuActivity, DataModel.RoleMenuActivity>(roleMenuActivityObj);
                                    roleact.ActivityID = menuactivity.ID;
                                    roleact.RoleMenuID = roleMenu.ID;
                                    roleact.CreatedBy = role.CreatedBy;
                                    roleact.CreatedTime = DateTime.Now;
                                    roleact.LastModifiedTime = null;
                                    context.RoleMenuActivity.Add(roleact);
                                    context.SaveChanges();
                                }

                            }
                        }
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RolesCreated;
                    }
                    role.Requests = mapper.Map<List<DataModel.Role>, List<Domain.Role>>(roles);
                    roleResponse.Data = role.Requests;
                    roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    roleResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return roleResponse;
        }

        public RoleResponse DeleteRole(int id)
        {
            RoleResponse roleResponse = new RoleResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    if (id > 0)
                    {
                        var role = context.Roles.Where(x => x.ID == id).FirstOrDefault();
                        if (role != null)
                        {
                            role.IsDelete = true;
                            context.SaveChanges();
                            roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RolesDeleted;
                            roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            roleResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            roleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                            roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return roleResponse;
        }

        public RoleResponse GetRoles(RoleRequest roles)
        {
            RoleResponse roleResponse = new RoleResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    List<Domain.Role> rolesList = context.Roles.Where(r => !r.IsDelete).Select(role => new Domain.Role
                    {
                        ID = role.ID,
                        RoleCode = role.RoleCode,
                        RoleDescription = role.RoleDescription,
                        ValidFrom = role.ValidFrom,
                        ValidTo = role.ValidTo,
                        IsActive = role.IsActive
                    }).ToList();

                    // Filter
                    if (roles.Requests.Count > 0)
                    {
                        var filter = roles.Requests[0];

                        if (filter.ID > 0)
                        {
                            rolesList = rolesList.Where(s => s.ID == filter.ID).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.RoleCode))
                        {
                            rolesList = rolesList.Where(s => s.RoleCode.Contains(filter.RoleCode)).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.RoleDescription))
                        {
                            rolesList = rolesList.Where(s => s.RoleDescription.Contains(filter.RoleDescription)).ToList();
                        }

                        if (filter.ValidFrom != DateTime.MinValue)
                        {
                            rolesList = rolesList.Where(s => s.ValidFrom >= filter.ValidFrom).ToList();
                        }

                        if (filter.ValidTo != DateTime.MinValue)
                        {
                            rolesList = rolesList.Where(s => s.ValidTo <= filter.ValidTo).ToList();
                        }
                        //if (!filter.IsActive)
                        //{
                        //    rolesList = rolesList.Where(s => s.IsActive == false).ToList();
                        //}
                        //else
                        //{
                        //    rolesList = rolesList.Where(s => s.IsActive).ToList();
                        //}
                    }

                    // Sorting
                    if (!string.IsNullOrEmpty(roles.SortOrder))
                    {
                        switch (roles.SortOrder.ToLower())
                        {
                            case "isactive":
                                rolesList = rolesList.OrderBy(s => s.IsActive).ToList();
                                break;
                            case "isactive_desc":
                                rolesList = rolesList.OrderByDescending(s => s.IsActive).ToList();
                                break;
                            case "rolecode":
                                rolesList = rolesList.OrderBy(s => s.RoleCode).ToList();
                                break;
                            case "rolecode_desc":
                                rolesList = rolesList.OrderByDescending(s => s.RoleCode).ToList();
                                break;
                            case "roledescription":
                                rolesList = rolesList.OrderBy(s => s.RoleDescription).ToList();
                                break;
                            case "roledescription_desc":
                                rolesList = rolesList.OrderByDescending(s => s.RoleDescription).ToList();
                                break;
                            case "validfrom":
                                rolesList = rolesList.OrderBy(s => s.ValidFrom).ToList();
                                break;
                            case "validfrom_desc":
                                rolesList = rolesList.OrderByDescending(s => s.ValidFrom).ToList();
                                break;
                            case "validto":
                                rolesList = rolesList.OrderBy(s => s.ValidTo).ToList();
                                break;
                            case "validto_desc":
                                rolesList = rolesList.OrderByDescending(s => s.ValidTo).ToList();
                                break;
                            default:  // ID Descending 
                                rolesList = rolesList.OrderByDescending(s => s.ID).ToList();
                                break;
                        }
                    }

                    // Total NumberOfRecords
                    roleResponse.NumberOfRecords = rolesList.Count;

                    // Paging
                    int pageNumber = (roles.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(roles.PageSize);
                    if (pageSize > 0)
                    {
                        rolesList = rolesList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (rolesList.Count > 0)
                    {
                        roleResponse.Data = rolesList;
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        roleResponse.StatusCode = (int)HttpStatusCode.OK;
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        roleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }
            return roleResponse;
        }

        public RoleResponse GetRoleDetails(int roleId)
        {
            RoleResponse roleResponse = new RoleResponse();
            List<Domain.Role> userRoles = new List<Domain.Role>();
            try
            {
                using (var context = new OMSDBContext())
                {
                    var roles = (from role in context.Roles
                                 where role.ID == roleId
                                 select new Domain.Role()
                                 {
                                     ID = role.ID,
                                     RoleCode = role.RoleCode,
                                     RoleDescription = role.RoleDescription,
                                     ValidFrom = role.ValidFrom,
                                     ValidTo = role.ValidTo,
                                     IsActive = role.IsActive
                                 }).FirstOrDefault();

                    if (roles != null)
                    {
                        List<Domain.RoleMenu> roleMenus = new List<Domain.RoleMenu>();
                        Domain.Role userRole = new Domain.Role();
                        userRole.ID = roles.ID;
                        userRole.RoleCode = roles.RoleCode;
                        userRole.RoleDescription = roles.RoleDescription;
                        userRole.ValidFrom = roles.ValidFrom;
                        userRole.ValidTo = roles.ValidTo;
                        userRole.IsActive = roles.IsActive;
                        var roleMenuData = (from roleMenu in context.RoleMenus
                                            where roleMenu.RoleID == roleId
                                            select new Domain.RoleMenu()
                                            {
                                                ID = roleMenu.ID,
                                                MenuCode = roleMenu.Menu.MenuCode,
                                                MenuDescription = roleMenu.Menu.MenuDescription,
                                                MenuURL = roleMenu.Menu.MenuURL
                                            }).ToList();
                        if (roleMenuData != null)
                        {
                            foreach (var roleMenu in roleMenuData)
                            {
                                Domain.RoleMenu obj = new Domain.RoleMenu();
                                obj.ID = roleMenu.ID;
                                obj.MenuCode = roleMenu.MenuCode;
                                obj.MenuDescription = roleMenu.MenuDescription;
                                var roleMenuActivities = (from roleMenuActivity in context.RoleMenuActivity
                                                          join roleMenud in context.RoleMenus on roleMenuActivity.RoleMenuID equals roleMenud.ID
                                                          where roleMenuActivity.RoleMenuID == roleMenu.ID
                                                          select new Domain.RoleMenuActivity()
                                                          {
                                                              ID = roleMenuActivity.Activity.ID,
                                                              ActivityCode = roleMenuActivity.Activity.ActivityCode,
                                                              ActivityDescription = roleMenuActivity.Activity.ActivityDescription
                                                          }).ToList();
                                if (roleMenuActivities != null)
                                {
                                    List<Domain.RoleMenuActivity> roleMenuActiviti = new List<Domain.RoleMenuActivity>();
                                    foreach (var activ in roleMenuActivities)
                                    {
                                        Domain.RoleMenuActivity roleMenuActivity = new Domain.RoleMenuActivity();
                                        roleMenuActivity.ActivityCode = activ.ActivityCode;
                                        roleMenuActivity.ID = activ.ID;
                                        roleMenuActivity.ActivityDescription = activ.ActivityDescription;
                                        roleMenuActiviti.Add(roleMenuActivity);
                                    }
                                    obj.RoleMenuActivities = roleMenuActivities;
                                }
                                roleMenus.Add(obj);
                            }
                        }
                        userRole.RoleMenus = roleMenus;
                        userRoles.Add(userRole);
                        roleResponse.Data = userRoles;
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        roleResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        roleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }

                }
            }
            catch (Exception ex)
            {
                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }
            return roleResponse;
        }

        #endregion

        #region "User Role"

        public UserRoleResponse CreateUpdateUserRole(UserRoleRequest userRoleRequest)
        {
            UserRoleResponse userRoleResponse = new UserRoleResponse();
            try
            {
                using (var tMSDBContext = new OMSDBContext())
                {
                    foreach (var userRoleDetail in userRoleRequest.Requests)
                    {
                        if (userRoleDetail.UserID > 0)
                        {
                            var isUserRoleAlreadyAssigned = tMSDBContext.UserRoles.Any(userRole => userRole.UserID==userRoleDetail.UserID && userRole.RoleID == userRoleDetail.RoleID && userRole.BusinessAreaID == userRoleDetail.BusinessAreaID && userRole.ID!=userRoleDetail.ID);
                            if (!isUserRoleAlreadyAssigned)
                            {
                                if (userRoleDetail.ID == 0)
                                {
                                    DataModel.UserRoles userRoleObject = new UserRoles()
                                    {
                                        UserID = userRoleDetail.UserID,
                                        RoleID = userRoleDetail.RoleID,
                                        BusinessAreaID = userRoleDetail.BusinessAreaID,
                                        CreatedBy = userRoleRequest.CreatedBy,
                                        CreatedTime = DateTime.Now
                                    };
                                    tMSDBContext.UserRoles.Add(userRoleObject);
                                    userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleCreated;
                                }
                                else
                                {
                                    var userAssignedRoleDetails = tMSDBContext.UserRoles.Where(userRole => userRole.RoleID == userRoleDetail.RoleID && userRole.BusinessAreaID == userRoleDetail.BusinessAreaID).FirstOrDefault();
                                    userAssignedRoleDetails.RoleID = userRoleDetail.RoleID;
                                    userAssignedRoleDetails.BusinessAreaID = userRoleDetail.BusinessAreaID;
                                    userAssignedRoleDetails.LastModifiedBy = userRoleRequest.LastModifiedBy;
                                    userAssignedRoleDetails.LastModifiedTime = userRoleRequest.LastModifiedTime;
                                    userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleUpdated;
                                }
                                tMSDBContext.SaveChanges();
                                userRoleResponse.StatusCode = (int)HttpStatusCode.OK;
                            }
                            else
                            {
                                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                                userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleAlreadyAssigned;
                                userRoleResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                            }
                        }
                        else
                        {
                            userRoleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUserID;
                            userRoleResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        }
                        userRoleResponse.Data = userRoleRequest.Requests;
                    }
                }
            }
            catch (Exception ex)
            {
                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userRoleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }
            return userRoleResponse;
        }

        public UserResponse DeleteUserRole(int userRoleID)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    if (userRoleID > 0)
                    {
                        var userRoleDetails = context.UserRoles.Where(i => i.ID == userRoleID).FirstOrDefault();
                        if (userRoleDetails != null)
                        {
                            userRoleDetails.IsDelete = true;
                            context.SaveChanges();
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleDeleted;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUserID;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUserID;
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    }
                }
            }
            catch (Exception ex)
            {
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }
            return userResponse;
        }

        public UserRoleResponse GetUserRoles(UserRoleRequest userRoleRequest)
        {

            UserRoleResponse userRoleResponse = new UserRoleResponse();

            try
            {
                using (var context = new OMSDBContext())
                {
                    List<Domain.UserRole> userRoleList = context.UserRoles.Where(userRole => !userRole.IsDelete).Select(userRole => new Domain.UserRole
                    {
                        ID = userRole.ID,
                        RoleID = userRole.RoleID,
                        RoleName = userRole.Role.RoleDescription,
                        UserID = userRole.UserID,
                        UserName = userRole.User.UserName,
                        BusinessAreaID = userRole.BusinessAreaID,
                        BusinessArea = userRole.BusinessArea.BusinessAreaDescription
                    }).ToList();

                    // Filter
                    if (userRoleRequest.Requests.Count > 0)
                    {
                        var filter = userRoleRequest.Requests[0];

                        if (filter.ID > 0)
                        {
                            userRoleList = userRoleList.Where(s => s.ID == filter.ID).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.BusinessArea))
                        {
                            userRoleList = userRoleList.Where(s => s.BusinessArea.Contains(filter.BusinessArea)).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.RoleName))
                        {
                            userRoleList = userRoleList.Where(s => s.RoleName.Contains(filter.RoleName)).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.UserName))
                        {
                            userRoleList = userRoleList.Where(s => s.UserName.Contains(filter.UserName)).ToList();
                        }


                    }

                    // Sorting
                    if (!string.IsNullOrEmpty(userRoleRequest.SortOrder))
                    {
                        switch (userRoleRequest.SortOrder.ToLower())
                        {
                            case "rolename":
                                userRoleList = userRoleList.OrderBy(s => s.RoleName).ToList();
                                break;
                            case "rolename_desc":
                                userRoleList = userRoleList.OrderByDescending(s => s.RoleName).ToList();
                                break;
                            case "username":
                                userRoleList = userRoleList.OrderBy(s => s.UserName).ToList();
                                break;
                            case "username_desc":
                                userRoleList = userRoleList.OrderByDescending(s => s.UserName).ToList();
                                break;
                            case "businessarea":
                                userRoleList = userRoleList.OrderBy(s => s.BusinessArea).ToList();
                                break;
                            case "businessarea_desc":
                                userRoleList = userRoleList.OrderByDescending(s => s.BusinessArea).ToList();
                                break;
                            default:  // ID Descending 
                                userRoleList = userRoleList.OrderByDescending(s => s.ID).ToList();
                                break;
                        }
                    }

                    // Total NumberOfRecords
                    userRoleResponse.NumberOfRecords = userRoleList.Count;

                    // Paging
                    int pageNumber = (userRoleRequest.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(userRoleRequest.PageSize);
                    if (pageSize > 0)
                    {
                        userRoleList = userRoleList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (userRoleList.Count > 0)
                    {
                        userRoleResponse.Data = userRoleList;
                        userRoleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userRoleResponse.StatusCode = (int)HttpStatusCode.OK;
                        userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        userRoleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userRoleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userRoleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }

            return userRoleResponse;
        }

        #endregion

        #region "Master Data Operations"

        public RegionResponse GetRegions(RegionRequest regions)
        {
            RegionResponse regionResponse = new RegionResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    List<Domain.Region> regionsList = context.BusinessAreas.Select(region => new Domain.Region
                    {
                        ID = region.ID,
                        Address = region.Address,
                        BusinessAreaDescription = region.BusinessAreaDescription,
                        BusinessAreaCode = region.BusinessAreaCode,
                        CompanyCodeID = region.CompanyCodeID ?? region.CompanyCodeID.Value,
                        PostalCodeID = region.PostalCodeID ?? region.PostalCodeID.Value
                    }).ToList();

                    // Filter
                    if (regions.Requests.Count > 0)
                    {
                        var filter = regions.Requests[0];

                        if (filter.ID > 0)
                        {
                            regionsList = regionsList.Where(s => s.ID == filter.ID).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.Address))
                        {
                            regionsList = regionsList.Where(s => s.Address.Contains(filter.Address)).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.BusinessAreaCode))
                        {
                            regionsList = regionsList.Where(s => s.BusinessAreaCode.Contains(filter.BusinessAreaCode)).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.BusinessAreaDescription))
                        {
                            regionsList = regionsList.Where(s => s.BusinessAreaDescription.Contains(filter.BusinessAreaDescription)).ToList();
                        }

                        if (filter.CompanyCodeID > 0)
                        {
                            regionsList = regionsList.Where(s => s.CompanyCodeID == filter.CompanyCodeID).ToList();
                        }

                        if (filter.PostalCodeID > 0)
                        {
                            regionsList = regionsList.Where(s => s.PostalCodeID == filter.PostalCodeID).ToList();
                        }
                    }

                    // Sorting
                    if (!string.IsNullOrEmpty(regions.SortOrder))
                    {
                        switch (regions.SortOrder.ToLower())
                        {
                            case "address":
                                regionsList = regionsList.OrderBy(s => s.Address).ToList();
                                break;
                            case "address_desc":
                                regionsList = regionsList.OrderByDescending(s => s.Address).ToList();
                                break;
                            case "businessareacode":
                                regionsList = regionsList.OrderBy(s => s.BusinessAreaCode).ToList();
                                break;
                            case "businessareacode_desc":
                                regionsList = regionsList.OrderByDescending(s => s.BusinessAreaCode).ToList();
                                break;
                            case "businessareadescription":
                                regionsList = regionsList.OrderBy(s => s.BusinessAreaDescription).ToList();
                                break;
                            case "businessareadescription_desc":
                                regionsList = regionsList.OrderByDescending(s => s.BusinessAreaDescription).ToList();
                                break;
                            case "companycodeid":
                                regionsList = regionsList.OrderBy(s => s.CompanyCodeID).ToList();
                                break;
                            case "companycodeid_desc":
                                regionsList = regionsList.OrderByDescending(s => s.CompanyCodeID).ToList();
                                break;
                            case "postalcodeid":
                                regionsList = regionsList.OrderBy(s => s.PostalCodeID).ToList();
                                break;
                            case "postalcodeid_desc":
                                regionsList = regionsList.OrderByDescending(s => s.PostalCodeID).ToList();
                                break;
                            default:  // ID Descending 
                                regionsList = regionsList.OrderByDescending(s => s.ID).ToList();
                                break;
                        }
                    }

                    // Total NumberOfRecords
                    regionResponse.NumberOfRecords = regionsList.Count;

                    // Paging
                    int pageNumber = (regions.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(regions.PageSize);
                    if (pageSize > 0)
                    {
                        regionsList = regionsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (regionsList.Count > 0)
                    {
                        regionResponse.Data = regionsList;
                        regionResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        regionResponse.StatusCode = (int)HttpStatusCode.OK;
                        regionResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        regionResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        regionResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        regionResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                regionResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                regionResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                regionResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }
            return regionResponse;
        }

        public RoleMenuResponse GetMenuWithActivities()
        {
            RoleMenuResponse roleMenuResponse = new RoleMenuResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    var menus = from menu in context.Menus
                                select menu;
                    if (menus != null)
                    {
                        List<Domain.RoleMenu> roleMenus = new List<Domain.RoleMenu>();
                        foreach (var item in menus)
                        {
                            Domain.RoleMenu obj = new Domain.RoleMenu();
                            obj.ID = item.ID;
                            obj.MenuCode = item.MenuCode;
                            obj.MenuDescription = item.MenuDescription;
                            var act = (from rmactivity in context.MenuActivities
                                       join activity in context.Activities on rmactivity.ActivityID equals activity.ID
                                       where rmactivity.MenuID == item.ID
                                       select new Domain.RoleMenuActivity()
                                       {
                                           ID = activity.ID,
                                           ActivityCode = activity.ActivityCode,
                                           ActivityDescription = activity.ActivityDescription
                                       }).ToList();
                            if (act != null)
                            {
                                List<Domain.RoleMenuActivity> roleMenuActivities = new List<Domain.RoleMenuActivity>();
                                foreach (var activ in act)
                                {
                                    Domain.RoleMenuActivity roleMenuActivity = new Domain.RoleMenuActivity();
                                    roleMenuActivity.ActivityCode = activ.ActivityCode;
                                    roleMenuActivity.ID = activ.ID;
                                    roleMenuActivity.ActivityDescription = activ.ActivityDescription;
                                    roleMenuActivities.Add(roleMenuActivity);
                                }
                                obj.RoleMenuActivities = roleMenuActivities;
                            }
                            roleMenus.Add(obj);
                        }
                        roleMenuResponse.Data = roleMenus;
                        roleMenuResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        roleMenuResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                roleMenuResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleMenuResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleMenuResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return roleMenuResponse;
        }

        public ApplicationResponse GetApplications()
        {
            ApplicationResponse applicationResponse = new ApplicationResponse();
            try
            {
                using (var context = new OMSDBContext())
                {
                    List<Domain.Application> applications = context.Applications.Select(application => new Domain.Application
                    {
                        ID = application.ID,
                        ApplicationCode = application.ApplicationCode,
                        ApplicationName = application.ApplicationName
                    }).ToList();
                    applicationResponse.Data = applications;
                    applicationResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    applicationResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                applicationResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                applicationResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                applicationResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
                _logger.Log(LogLevel.Error, ex);
            }
            return applicationResponse;
        }

        public CommonResponse GetUserNames()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<Domain.Common> commons = new List<Domain.Common>();
            try
            {
                using (var context = new OMSDBContext())
                {
                    commons =
                        (from user in context.Users
                         where !user.IsDelete
                         orderby user.UserName
                         select new Domain.Common
                         {
                             Id = user.ID,
                             Value = user.UserName,
                         }).ToList();
                }


                if (commons.Count > 0)
                {
                    commonResponse.Data = commons;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return commonResponse;
        }

        public CommonResponse GetRoleCodes()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<Domain.Common> commons = new List<Domain.Common>();
            try
            {
                using (var context = new OMSDBContext())
                {
                    commons =
                        (from role in context.Roles
                         where !role.IsDelete
                         orderby role.RoleCode
                         select new Domain.Common
                         {
                             Id = role.ID,
                             Value = role.RoleCode,
                         }).ToList();
                }


                if (commons.Count > 0)
                {
                    commonResponse.Data = commons;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return commonResponse;
        }

        public CommonResponse GetRegionCodes()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<Domain.Common> commons = new List<Domain.Common>();
            try
            {
                using (var context = new OMSDBContext())
                {
                    commons =
                        (from businessArea in context.BusinessAreas
                         orderby businessArea.BusinessAreaCode
                         select new Domain.Common
                         {
                             Id = businessArea.ID,
                             Value = businessArea.BusinessAreaCode
                         }).ToList();
                }


                if (commons.Count > 0)
                {
                    commonResponse.Data = commons;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return commonResponse;
        }

        #endregion
    }
}
