using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMS.API.Classes;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    public class MediaController : ApiController
    {
        [RoutePrefix("api/v1/media")]
        public class UploadMediaController : ApiController
        {
            [HttpPost, Route("uploadfile")]
            public async Task<IHttpActionResult> UploadFile()
            {
                IMediaTask mediaTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MediaTask;
                ResponseDataForFileUpload response = await mediaTask.UploadFile(Request);
                return Ok(response);
            }

            [AllowAnonymous, HttpGet, Route("downloadfile")]
            public async Task<HttpResponseMessage> Get(string fileGuid)
            {
                IMediaTask mediaTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MediaTask;
                return await mediaTask.Get(fileGuid);
            }
        }
    }
}
