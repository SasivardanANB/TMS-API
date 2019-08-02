using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Objects;
using System.Net.Http;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task
{
    public abstract class MediaTask : IMediaTask
    {
        public abstract Task<HttpResponseMessage> Get(string fileGuid);
        public abstract Task<ResponseDataForFileUpload> UploadFile(HttpRequestMessage request);
    }
}
