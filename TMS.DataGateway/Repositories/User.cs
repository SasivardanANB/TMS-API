using AutoMapper;
using NLog;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DataGateway.DataModels;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DataModel = TMS.DataGateway.DataModels;
using Domain = TMS.DomainObjects.Objects;
using System.Configuration;
using System.Data.Common;

namespace TMS.DataGateway.Repositories
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
                using (var context = new TMSDBContext())
                {
                    Domain.User userData;

                    if (login.IsSAMALogin)
                    {
                        userData = (from user in context.Users
                                    where user.UserName == login.UserName
                                    && !user.IsDelete
                                    select new Domain.User()
                                    {
                                        ID = user.ID,
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        IsActive = user.IsActive,
                                        UserName = user.UserName
                                    }).FirstOrDefault();
                    }
                    else
                    {
                        string encryptedPassword = Encryption.EncryptionLibrary.EncryptPassword(login.UserPassword);
                        userData = (from user in context.Users
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
                    }
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
                            var tokenData = (from tokens in context.Tokens
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

                            //Sending Role details
                            userResponse.RoleData = GetUserRoleMenus(userData.ID);


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
                using (var context = new TMSDBContext())
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
                            //userDataModel.Password = context.Users.Where(d => d.ID == userDataModel.ID).Select(p => p.Password).FirstOrDefault();
                            context.Entry(userDataModel).State = System.Data.Entity.EntityState.Modified;
                            //context.Entry(userDataModel).Property(p => p.Password).IsModified = false;
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
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        }
                        else //Create User
                        {
                            var checkUserName = context.Users.Where(u => u.UserName == userDataModel.UserName && !u.IsDelete).FirstOrDefault();
                            if (checkUserName == null)
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
                                userResponse.StatusCode = (int)HttpStatusCode.OK;
                                userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            }
                            else
                            {
                                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserNameExisted;
                                userResponse.StatusCode = (int)HttpStatusCode.OK;
                                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                            }
                        }

                        userDataModelList.Add(userDataModel);
                    }

                    user.Requests = mapper.Map<List<DataModel.User>, List<Domain.User>>(userDataModelList);
                    userResponse.Data = user.Requests;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message + ex.InnerException == null ? "" : ex.InnerException.Message;
            }
            return userResponse;
        }

        public UserResponse DeleteUser(int userId)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    if (userId > 0)
                    {
                        var user = context.Users.Where(x => x.ID == userId).FirstOrDefault();
                        if (user != null)
                        {
                            var userRole = context.UserRoles.Where(x => x.UserID == user.ID && x.IsDelete == false).FirstOrDefault();
                            if (userRole != null)
                            {

                                user.IsDelete = true;
                                context.SaveChanges();
                                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersDeleted;
                                userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                                userResponse.StatusCode = (int)HttpStatusCode.OK;
                            }
                            else
                            {
                                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.ActiveUserRoleExists;
                                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                userResponse.StatusCode = (int)HttpStatusCode.OK;
                            }
                        }
                        else
                        {
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUser;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                    }
                }
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

        public UserResponse GetUsers(UserRequest userReq)
        {
            UserResponse userResponse = new UserResponse();
            List<Domain.User> usersList = new List<Domain.User>();
            List<DataModels.User> users = new List<DataModels.User>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    usersList =
                        (from user in context.Users
                         where !user.IsDelete
                         select new Domain.User
                         {
                             ID = user.ID,
                             UserName = user.UserName,
                             Email = user.Email,
                             PhoneNumber = user.PhoneNumber,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             IsActive = user.IsActive,
                             Password = user.Password,
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
                    if (usersList.Count > 0)
                    {
                        foreach (var item in usersList)
                        {
                            item.Password = Encryption.EncryptionLibrary.DecrypPassword(item.Password);
                        }
                    }
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
                        usersList = usersList.Where(s => s.UserName.ToLower().Contains(userFilter.UserName.ToLower())).ToList();
                    }
                    if (!String.IsNullOrEmpty(userFilter.Email))
                    {
                        usersList = usersList.Where(s => s.Email.ToLower().Contains(userFilter.Email.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(userFilter.FirstName))
                    {
                        usersList = usersList.Where(s => s.FirstName.ToLower().Contains(userFilter.FirstName.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(userFilter.LastName))
                    {
                        usersList = usersList.Where(s => s.LastName.ToLower().Contains(userFilter.LastName.ToLower())).ToList();
                    }

                    //if (userFilter.IsActive != null)
                    //{
                    //    usersList = usersList.Where(s => s.IsActive == userFilter.IsActive).ToList();
                    //}
                }

                // Sorting
                if (usersList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(userReq.SortOrder))
                    {


                        switch (userReq.SortOrder.ToLower())
                        {
                            case "username":
                                usersList = usersList.OrderBy(s => s.UserName).ToList();
                                break;
                            case "username_desc":
                                usersList = usersList.OrderByDescending(s => s.UserName).ToList();
                                break;
                            case "email":
                                usersList = usersList.OrderBy(s => s.Email).ToList();
                                break;
                            case "email_desc":
                                usersList = usersList.OrderByDescending(s => s.Email).ToList();
                                break;
                            case "phonenumber":
                                usersList = usersList.OrderBy(s => s.PhoneNumber).ToList();
                                break;
                            case "phonenumber_desc":
                                usersList = usersList.OrderByDescending(s => s.PhoneNumber).ToList();
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
                    else
                    {
                        usersList = usersList.OrderByDescending(s => s.ID).ToList();
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
                userResponse.StatusMessage = ex.Message;
            }
            return userResponse;
        }

        public UserResponse UpdateUserProfile(UserRequest user)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                Domain.User userData = user.Requests[0];

                using (var context = new TMSDBContext())
                {
                    var userDataModelList = new List<DataModel.User>();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.User, DataModel.User>().ReverseMap()
                        .ForMember(x => x.ApplicationNames, opt => opt.Ignore());
                    });

                    IMapper mapper = config.CreateMapper();
                    var userDetails = context.Users.Where(u => u.ID == userData.ID).FirstOrDefault();
                    if (userDetails != null)
                    {

                        if (userData.ID > 0) //Update User
                        {

                            userDetails.FirstName = userData.FirstName;
                            userDetails.LastName = userData.LastName;
                            userDetails.Email = userData.Email;
                            userDetails.PhoneNumber = userData.PhoneNumber;
                            userDetails.LastModifiedBy = user.LastModifiedBy;
                            userDetails.LastModifiedTime = DateTime.Now;

                            context.SaveChanges();

                            userDataModelList.Add(userDetails);
                        }

                        user.Requests = mapper.Map<List<DataModel.User>, List<Domain.User>>(userDataModelList);
                        userResponse.Data = user.Requests;

                        userResponse.StatusCode = (int)HttpStatusCode.OK;
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersUpdated;
                    }
                }
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

        public UserResponse ChangePassword(ChangePasswordRequest changePasswordRequest, string type)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var userdetails = context.Users.Where(u => u.ID == changePasswordRequest.Id).FirstOrDefault();
                    if (userdetails != null)
                    {
                        if (type == "changepassword")
                        {
                            var userpassword = Encryption.EncryptionLibrary.DecrypPassword(userdetails.Password);
                            if (userpassword != changePasswordRequest.OldPassword)
                            {
                                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                userResponse.StatusCode = (int)HttpStatusCode.OK;
                                userResponse.StatusMessage = DomainObjects.Resource.ResourceData.IncorrectOldPassword;
                            }
                            else
                            {
                                if (userpassword == changePasswordRequest.NewPassword)
                                {
                                    userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                    userResponse.StatusCode = (int)HttpStatusCode.OK;
                                    userResponse.StatusMessage = DomainObjects.Resource.ResourceData.NewPasswordMustbeDifferent;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(changePasswordRequest.NewPassword))
                                    {
                                        changePasswordRequest.NewPassword = Encryption.EncryptionLibrary.EncryptPassword(changePasswordRequest.NewPassword);
                                    }
                                    userdetails.Password = changePasswordRequest.NewPassword;
                                    userdetails.LastModifiedBy = userdetails.UserName;
                                    context.SaveChanges();
                                    userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                                    userResponse.StatusCode = (int)HttpStatusCode.OK;
                                    userResponse.StatusMessage = DomainObjects.Resource.ResourceData.PasswordUpdated;
                                }
                            }
                        }
                        if (type == "resetpassword")
                        {
                            if (!string.IsNullOrEmpty(changePasswordRequest.NewPassword))
                            {
                                changePasswordRequest.NewPassword = Encryption.EncryptionLibrary.EncryptPassword(changePasswordRequest.NewPassword);
                            }
                            userdetails.Password = changePasswordRequest.NewPassword;
                            userdetails.LastModifiedBy = userdetails.UserName;
                            context.SaveChanges();
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.PasswordUpdated;
                        }
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        userResponse.StatusCode = (int)HttpStatusCode.OK;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserDetailsNotFound;
                    }
                }
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

        public UserResponse ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var userdetails = context.Users
                      .Where(u => String.IsNullOrEmpty(forgotPasswordRequest.Email) || u.Email.ToLower().Contains(forgotPasswordRequest.Email.ToLower()))
                      .Where(u => String.IsNullOrEmpty(forgotPasswordRequest.UserName) || u.UserName.ToLower().Contains(forgotPasswordRequest.UserName.ToLower())).FirstOrDefault();

                    if (userdetails != null)
                    {
                        List<Domain.User> userData = new List<Domain.User>()
                        {
                            new Domain.User()
                            {
                                ID=userdetails.ID,
                                UserName = userdetails.UserName,
                                Email = userdetails.Email
                            }
                        };

                        userResponse.Data = userData;
                        userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        userResponse.StatusCode = (int)HttpStatusCode.OK;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        userResponse.StatusCode = (int)HttpStatusCode.OK;
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserDetailsNotFound;
                    }
                }
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

        #endregion

        #region "Role Management"

        public RoleResponse CreateUpdateRole(RoleRequest role)
        {
            RoleResponse roleResponse = new RoleResponse();
            try
            {
                using (var context = new TMSDBContext())
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
                            var checkRoleCode = context.Roles.Where(r => r.RoleCode == roleObject.RoleCode && !r.IsDelete).FirstOrDefault();
                            if (checkRoleCode == null)
                            {
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
                                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RolesCreated;
                            }
                            else
                            {
                                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RoleCodeExists;
                            }
                        }
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
                roleResponse.StatusMessage = ex.Message;
            }
            return roleResponse;
        }

        public RoleResponse DeleteRole(int id)
        {
            RoleResponse roleResponse = new RoleResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    if (id > 0)
                    {
                        var role = context.Roles.Where(x => x.ID == id).FirstOrDefault();
                        if (role != null)
                        {
                            var userRole = context.UserRoles.Where(ur => ur.RoleID == id && ur.IsDelete == false).FirstOrDefault();
                            if (userRole == null)
                            {
                                role.IsDelete = true;
                                context.SaveChanges();
                                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RolesDeleted;
                                roleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                                roleResponse.StatusCode = (int)HttpStatusCode.OK;
                            }
                            else
                            {
                                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                roleResponse.StatusCode = (int)HttpStatusCode.OK;
                                roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.RoleCannontBeDeleted;
                            }
                        }
                        else
                        {
                            roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
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
                roleResponse.StatusMessage = ex.Message;
            }
            return roleResponse;
        }

        public RoleResponse GetRoles(RoleRequest roles)
        {
            RoleResponse roleResponse = new RoleResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    List<Domain.Role> rolesList = context.Roles.Where(r => !r.IsDelete).Select(role => new Domain.Role
                    {
                        ID = role.ID,
                        RoleCode = role.RoleCode,
                        RoleDescription = role.RoleDescription,
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
                            rolesList = rolesList.Where(s => s.RoleCode.ToLower().Contains(filter.RoleCode.ToLower())).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.RoleDescription))
                        {
                            rolesList = rolesList.Where(s => s.RoleDescription.ToLower().Contains(filter.RoleDescription.ToLower())).ToList();
                        }
                    }

                    // Sorting
                    if (rolesList.Count > 0)
                    {
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
                                default:  // ID Descending 
                                    rolesList = rolesList.OrderByDescending(s => s.ID).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            rolesList = rolesList.OrderByDescending(s => s.ID).ToList();
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
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        roleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = ex.Message;
            }
            return roleResponse;
        }

        public RoleResponse GetRoleDetails(int roleId)
        {
            RoleResponse roleResponse = new RoleResponse();
            List<Domain.Role> userRoles = new List<Domain.Role>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var roles = (from role in context.Roles
                                 where role.ID == roleId
                                 select new Domain.Role()
                                 {
                                     ID = role.ID,
                                     RoleCode = role.RoleCode,
                                     RoleDescription = role.RoleDescription,
                                     IsActive = role.IsActive
                                 }).FirstOrDefault();

                    if (roles != null)
                    {
                        List<Domain.RoleMenu> roleMenus = new List<Domain.RoleMenu>();
                        Domain.Role userRole = new Domain.Role();
                        userRole.ID = roles.ID;
                        userRole.RoleCode = roles.RoleCode;
                        userRole.RoleDescription = roles.RoleDescription;
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
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        roleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }

                }
            }
            catch (Exception ex)
            {
                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = ex.Message;
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
                using (var tMSDBContext = new TMSDBContext())
                {
                    foreach (var userRoleDetail in userRoleRequest.Requests)
                    {
                        if (userRoleDetail.ID == 0) // Create
                        {
                            var isUserRoleAlreadyAssigned = tMSDBContext.UserRoles.Any(userRole => userRole.UserID == userRoleDetail.UserID && userRole.RoleID == userRoleDetail.RoleID && userRole.BusinessAreaID == userRoleDetail.BusinessAreaID && userRole.IsDelete == false);
                            if (!isUserRoleAlreadyAssigned)
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
                                tMSDBContext.SaveChanges();
                                userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleCreated;
                                userRoleResponse.StatusCode = (int)HttpStatusCode.OK;
                                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Success;

                            }
                            else
                            {
                                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleAlreadyAssigned;
                                userRoleResponse.StatusCode = (int)HttpStatusCode.OK;
                            }
                        }
                        else // Update
                        {
                            var userAssignedRoleDetails = tMSDBContext.UserRoles.Where(userRole => userRole.ID == userRoleDetail.ID).FirstOrDefault();
                            if (userAssignedRoleDetails != null)
                            {
                                userAssignedRoleDetails.RoleID = userRoleDetail.RoleID;
                                userAssignedRoleDetails.BusinessAreaID = userRoleDetail.BusinessAreaID;
                                userAssignedRoleDetails.LastModifiedBy = userRoleRequest.LastModifiedBy;
                                userAssignedRoleDetails.LastModifiedTime = DateTime.Now;

                                tMSDBContext.SaveChanges();
                                userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleUpdated;
                                userRoleResponse.StatusCode = (int)HttpStatusCode.OK;
                                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userRoleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userRoleResponse.StatusMessage = ex.Message;
                _logger.Log(LogLevel.Error, ex);
            }
            return userRoleResponse;
        }

        public UserResponse DeleteUserRole(int userRoleID)
        {
            UserResponse userResponse = new UserResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    if (userRoleID > 0)
                    {
                        var userRoleDetails = context.UserRoles.Where(i => i.ID == userRoleID).FirstOrDefault();
                        if (userRoleDetails != null)
                        {
                            userRoleDetails.IsDelete = true;
                            context.SaveChanges();
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.UserRoleDeleted;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            userResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            userResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUserID;
                            userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                            userResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        }
                    }
                    else
                    {
                        userResponse.StatusMessage = DomainObjects.Resource.ResourceData.InvalidUserID;
                        userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        userResponse.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    }
                }
            }
            catch (Exception ex)
            {
                userResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userResponse.StatusMessage = ex.Message;
                _logger.Log(LogLevel.Error, ex);
            }
            return userResponse;
        }

        public UserRoleResponse GetUserRoles(UserRoleRequest userRoleRequest)
        {

            UserRoleResponse userRoleResponse = new UserRoleResponse();

            try
            {
                using (var context = new TMSDBContext())
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
                            userRoleList = userRoleList.Where(s => s.BusinessArea.ToLower().Contains(filter.BusinessArea.ToLower())).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.RoleName))
                        {
                            userRoleList = userRoleList.Where(s => s.RoleName.ToLower().Contains(filter.RoleName.ToLower())).ToList();
                        }

                        if (!String.IsNullOrEmpty(filter.UserName))
                        {
                            userRoleList = userRoleList.Where(s => s.UserName.ToLower().Contains(filter.UserName.ToLower())).ToList();
                        }


                    }

                    // Sorting
                    if (userRoleList.Count > 0)
                    {
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
                        else
                        {
                            userRoleList = userRoleList.OrderByDescending(s => s.ID).ToList();
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
                        userRoleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        userRoleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        userRoleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                userRoleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                userRoleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                userRoleResponse.StatusMessage = ex.Message;
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
                using (var context = new TMSDBContext())
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
                        regionResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        regionResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        regionResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                regionResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                regionResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                regionResponse.StatusMessage = ex.Message;
                _logger.Log(LogLevel.Error, ex);
            }
            return regionResponse;
        }

        public RoleMenuResponse GetMenuWithActivities()
        {
            RoleMenuResponse roleMenuResponse = new RoleMenuResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var menus = from menu in context.Menus
                                select menu;
                    if (menus != null)
                    {
                        List<Domain.RoleMenu> roleMenus = new List<Domain.RoleMenu>();
                        foreach (var item in menus.ToList())
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
                        roleMenuResponse.StatusMessage = DomainObjects.Resource.ResourceData.MenuActivityRetrived;
                        roleMenuResponse.NumberOfRecords = roleMenus.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                roleMenuResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleMenuResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleMenuResponse.StatusMessage = ex.Message;
            }
            return roleMenuResponse;
        }

        public ApplicationResponse GetApplications()
        {
            ApplicationResponse applicationResponse = new ApplicationResponse();
            try
            {
                using (var context = new TMSDBContext())
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
                    applicationResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    applicationResponse.NumberOfRecords = applications.Count;
                    if (applications.Count == 0)
                    {
                        applicationResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                        applicationResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                applicationResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                applicationResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                applicationResponse.StatusMessage = ex.Message;
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
                using (var context = new TMSDBContext())
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

                //Number of records
                commonResponse.NumberOfRecords = commons.Count;

                if (commons.Count > 0)
                {
                    commonResponse.Data = commons;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = ex.Message;
            }
            return commonResponse;
        }

        public CommonResponse GetRoleCodes()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<Domain.Common> commons = new List<Domain.Common>();
            try
            {
                using (var context = new TMSDBContext())
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

                //Number of records
                commonResponse.NumberOfRecords = commons.Count;

                if (commons.Count > 0)
                {
                    commonResponse.Data = commons;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = ex.Message;
            }
            return commonResponse;
        }

        public CommonResponse GetRegionCodes()
        {
            CommonResponse commonResponse = new CommonResponse();
            List<Domain.Common> commons = new List<Domain.Common>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    commons =
                        (from businessArea in context.BusinessAreas
                         orderby businessArea.BusinessAreaCode
                         select new Domain.Common
                         {
                             Id = businessArea.ID,
                             Value = businessArea.BusinessAreaCode + (!string.IsNullOrEmpty(businessArea.BusinessAreaDescription) ? " : " + businessArea.BusinessAreaDescription : "")
                         }).ToList();
                }

                //Number of records
                commonResponse.NumberOfRecords = commons.Count;

                if (commons.Count > 0)
                {
                    commonResponse.Data = commons;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = ex.Message;
            }
            return commonResponse;
        }

        #endregion

        public DashboardResponse GetUserDashboard(UserRequest user)
        {
            DashboardResponse dashboardResponse = new DashboardResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    #region Filter Data on the basis of request and Role
                    var orders = context.OrderHeaders.ToList();
                    #endregion

                    dashboardResponse.AllOrderCount = orders.Count();
                    dashboardResponse.BookedCount = orders.Where(o => o.OrderStatusID == 1).Count();   // For status booked
                    dashboardResponse.ConfirmedCount = orders.Where(o => o.OrderStatusID == 2).Count();   // For status confirmed
                    dashboardResponse.Acceptedcount = orders.Where(o => o.OrderStatusID == 15).Count();   // For status accepted
                    dashboardResponse.PODCount = orders.Where(o => o.OrderStatusID == 11).Count();   // For status pod
                    dashboardResponse.CancelledCount = orders.Where(o => o.OrderStatusID == 13).Count();   // For status cancelled
                    dashboardResponse.LoadingCount = GetLoadingUnloadingCount(orders, "Load");
                    dashboardResponse.UnloadingCount = GetLoadingUnloadingCount(orders, "Unload");
                    dashboardResponse.LoadingCount = GetPrickupDropOffCount(orders, "Load");
                    dashboardResponse.UnloadingCount = GetPrickupDropOffCount(orders, "Unload");
                }
                dashboardResponse.Status = DomainObjects.Resource.ResourceData.Success;
                dashboardResponse.StatusCode = (int)HttpStatusCode.OK;
                dashboardResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
            }
            catch (Exception ex)
            {
                dashboardResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                dashboardResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                dashboardResponse.StatusMessage = ex.Message;
            }

            return dashboardResponse;
        }

        private int GetLoadingUnloadingCount(List<OrderHeader> orders, string type)
        {
            int count = 0;
            foreach (var item in orders)
            {
                using (var context = new TMSDBContext())
                {
                    int confirmArraive = context.OrderStatuses.FirstOrDefault(t => t.OrderStatusCode == "5").ID;
                    var lastStatus = (from o in orders
                                      join od in context.OrderDetails on o.ID equals od.OrderHeaderID
                                      join h in context.OrderStatusHistories on od.ID equals h.OrderDetailID
                                      where o.ID == item.ID && h.OrderStatusID != confirmArraive
                                      orderby h.StatusDate descending
                                      select new
                                      {
                                          IsLoad = h.IsLoad,
                                          StatusId = h.OrderStatusID,
                                          OrderDetailId = h.OrderDetailID,
                                          StatusDate = h.StatusDate
                                      }).FirstOrDefault();

                    if (lastStatus != null && type == "Load" && lastStatus.IsLoad == true)
                        count++;
                    else if (lastStatus != null && type == "Unload" && lastStatus.IsLoad == false)
                        count++;
                }
            }
            return count;

        }

        private int GetPrickupDropOffCount(List<OrderHeader> orders, string type)
        {
            int count = 0;
            foreach (var item in orders)
            {
                using (var context = new TMSDBContext())
                {
                    int confirmArraive = context.OrderStatuses.FirstOrDefault(t => t.OrderStatusCode == "5").ID;
                    var lastStatus = (from o in orders
                                      join od in context.OrderDetails on o.ID equals od.OrderHeaderID
                                      join h in context.OrderStatusHistories on od.ID equals h.OrderDetailID
                                      where o.ID == item.ID && h.OrderStatusID == confirmArraive
                                      orderby h.StatusDate descending
                                      select new
                                      {
                                          IsLoad = h.IsLoad,
                                          StatusId = h.OrderStatusID,
                                          OrderDetailId = h.OrderDetailID,
                                          StatusDate = h.StatusDate
                                      }).FirstOrDefault();

                    if (lastStatus != null && type == "Load" && lastStatus.IsLoad == true)
                        count++;
                    else if (lastStatus != null && type == "Unload" && lastStatus.IsLoad == false)
                        count++;
                }
            }
            return count;

        }

        public RoleResponse GetUserMenus(int userId)
        {
            RoleResponse roleResponse = new RoleResponse();
            List<Domain.Role> userRoles = new List<Domain.Role>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var roles = (from role in context.Roles
                                 join user in context.UserRoles on role.ID equals user.RoleID
                                 where user.UserID == userId
                                 select new Domain.Role()
                                 {
                                     ID = role.ID,
                                     RoleCode = role.RoleCode,
                                     RoleDescription = role.RoleDescription,
                                     IsActive = role.IsActive
                                 }).FirstOrDefault();

                    if (roles != null)
                    {
                        List<Domain.RoleMenu> roleMenus = new List<Domain.RoleMenu>();
                        Domain.Role userRole = new Domain.Role();
                        userRole.ID = roles.ID;
                        userRole.RoleCode = roles.RoleCode;
                        userRole.RoleDescription = roles.RoleDescription;
                        userRole.IsActive = roles.IsActive;
                        var roleMenuData = (from roleMenu in context.RoleMenus
                                            where roleMenu.RoleID == roles.ID
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
                        roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        roleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        roleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }

                }
            }
            catch (Exception ex)
            {
                roleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                roleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                roleResponse.StatusMessage = ex.Message;
                _logger.Log(LogLevel.Error, ex);
            }
            return roleResponse;
        }

        public List<Domain.Role> GetUserRoleMenus(int userId)
        {
            List<Domain.Role> userRoles = new List<Domain.Role>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var roles = (from role in context.Roles
                                 join user in context.UserRoles on role.ID equals user.RoleID
                                 where user.UserID == userId
                                 select new Domain.Role()
                                 {
                                     ID = role.ID,
                                     RoleCode = role.RoleCode,
                                     RoleDescription = role.RoleDescription,
                                     IsActive = role.IsActive
                                 }).FirstOrDefault();

                    if (roles != null)
                    {
                        List<Domain.RoleMenu> roleMenus = new List<Domain.RoleMenu>();
                        Domain.Role userRole = new Domain.Role();
                        userRole.ID = roles.ID;
                        userRole.RoleCode = roles.RoleCode;
                        userRole.RoleDescription = roles.RoleDescription;
                        userRole.IsActive = roles.IsActive;
                        var roleMenuData = (from roleMenu in context.RoleMenus
                                            where roleMenu.RoleID == roles.ID
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
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
            }
            return userRoles;
        }

    }
}
