using System.Net.Http;
using System.Threading.Tasks;
using TMS.DomainObjects.Objects;

namespace TMS.DomainGateway.Task.Interfaces
{
   public interface IMediaTask
    {
        Task<ResponseDataForFileUpload> UploadFile(HttpRequestMessage request);
        Task<HttpResponseMessage> Get(string fileGuid);
    }
}
