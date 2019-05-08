﻿using DMS.DomainGateway.Gateway;
using DMS.DomainGateway.Task;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using DMS.DomainObjects.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Helper.Model.DependencyResolver;
using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.API.Classes;

namespace DMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/user")]
    public class UserController : ApiController
    {
        [Route("login")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult LoginUser(LoginRequest login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userData = userTask.LoginUser(login);
            return Ok(userData);
        }

        [Route("createupdateuser")]
        [AllowAnonymous, HttpPost]
        public IHttpActionResult CreateUpdateUser(UserRequest user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUserTask userTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().UserTask;
            UserResponse userResponse = userTask.CreateUpdateUser(user);
            return Ok(userResponse);
        }
    }
}