using System.Net.Http;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Objects;

namespace TMS.DomainGateway.Task
{
    public abstract class MediaTask : IMediaTask
    {
        public abstract Task<HttpResponseMessage> Get(string fileGuid);
        public abstract Task<ResponseDataForFileUpload> UploadFile(HttpRequestMessage request);
    }
}
