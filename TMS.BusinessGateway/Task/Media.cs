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
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Objects;
using TMS.BusinessGateway.Classes;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessMediaTask : MediaTask
    {
        public override async Task<HttpResponseMessage> Get(string fileGuid)
        {
            try
            {
                var accountName = ConfigurationManager.AppSettings["storage:AccountName"];
                var accountKey = ConfigurationManager.AppSettings["storage:AccountKey"];
                var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                var Blob = await blobClient.GetBlobReferenceFromServerAsync(new Uri(ConfigurationManager.AppSettings["FileStorageURL"] + fileGuid));
                var isExist = await Blob.ExistsAsync();
                if (!isExist)
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Content = new StringContent("file not found")
                    };
                }
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
                Stream blobStream = await Blob.OpenReadAsync();
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StreamContent(blobStream);
                message.Content.Headers.ContentLength = Blob.Properties.Length;
                message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Blob.Properties.ContentType);
                message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline");
                return message;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public override async Task<ResponseDataForFileUpload> UploadFile(HttpRequestMessage request)
        {
            ResponseDataForFileUpload responseDataForFileUpload = new ResponseDataForFileUpload();
            if (!request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var accountName = ConfigurationManager.AppSettings["storage:AccountName"];
            var accountKey = ConfigurationManager.AppSettings["storage:AccountKey"];
            var containerName = ConfigurationManager.AppSettings["storage:AccountContainer"];
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
            var provider = new AzureStorageMultipartFormDataStreamProvider(blobContainer);
            try
            {
                await request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception ex)
            {
                responseDataForFileUpload.Status = DomainObjects.Resource.ResourceData.Failure;
                responseDataForFileUpload.StatusMessage = ex.Message;
                responseDataForFileUpload.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                return responseDataForFileUpload;
            }

            // Retrieve the filename of the file you have uploaded
            var fileGuid = provider.FileData.FirstOrDefault()?.LocalFileName;
            if (string.IsNullOrEmpty(fileGuid))
            {
                responseDataForFileUpload.Status = DomainObjects.Resource.ResourceData.Failure;
                responseDataForFileUpload.StatusMessage = DomainObjects.Resource.ResourceData.FileUploadError;
                responseDataForFileUpload.StatusCode = (int)HttpStatusCode.BadRequest;
                return responseDataForFileUpload;
            }
            responseDataForFileUpload.Status = DomainObjects.Resource.ResourceData.Success;
            responseDataForFileUpload.StatusMessage = DomainObjects.Resource.ResourceData.FileUploadSuccess;
            responseDataForFileUpload.StatusCode = (int)HttpStatusCode.OK;
            responseDataForFileUpload.Guid = fileGuid;
            return responseDataForFileUpload;
        }
    }
}
