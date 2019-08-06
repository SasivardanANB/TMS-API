using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using DMS.API.Classes;
using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.DomainObjects.Objects;

namespace DMS.API.Controllers
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

            [HttpPost, Route("downloadfile")]
            public async Task<HttpResponseMessage> Get(string fileGuid)
            {
                IMediaTask mediaTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MediaTask;
                return await mediaTask.Get(fileGuid);
            }
        }
    }
}
