using ActiveUp.Net.Mail;
using RestSharp;
using System;
using System.Configuration;

namespace TMS.BusinessGateway.Classes
{
    public static class Utility
    {
        public static string GetApiResponse(string apiRoute, Method method, object requestQueryParameter, string token, Parameter item = null)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["ApiGatewayBaseURL"]);
            client.AddDefaultHeader("Content-Type", "application/json");
            if (token != null)
                client.AddDefaultHeader("Token", token);
            var request = new RestRequest(apiRoute, method) { RequestFormat = DataFormat.Json };
            request.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["RequestTimeout"]);
            if (requestQueryParameter != null)
            {
                request.AddJsonBody(requestQueryParameter);
            }

            if (item != null)
            {
                request.Parameters.Add(item);
            }
            var result = client.Execute(request);
            return result.Content;
        }

        public static string GetFileUploadApiResponse(string apiRoute, Method method, MimePart mimePart, string token)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["ApiGatewayBaseURL"]);
            if (token != null)
                client.AddDefaultHeader("Token", token);
            var request = new RestRequest(apiRoute, method) { RequestFormat = DataFormat.Json, AlwaysMultipartFormData = true };
            request.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["RequestTimeout"]);
            if (mimePart != null)
            {
                client.AddDefaultHeader("Content-Type", "multipart/form-data");
                request.AddFileBytes("file", mimePart.BinaryContent, mimePart.Filename, mimePart.ContentType.MimeType);// upload from file byte array
            }
            var result = client.Execute(request);
            return result.Content;
        }
    }
}
