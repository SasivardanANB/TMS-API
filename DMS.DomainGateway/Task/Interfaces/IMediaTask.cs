using DMS.DomainObjects.Objects;
using System.Net.Http;
using System.Threading.Tasks;

namespace DMS.DomainGateway.Task.Interfaces
{
    public interface IMediaTask
    {
        Task<ResponseDataForFileUpload> UploadFile(HttpRequestMessage request);
        Task<HttpResponseMessage> Get(string fileGuid);
    }
}
