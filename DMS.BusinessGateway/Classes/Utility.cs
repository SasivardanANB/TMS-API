using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.BusinessGateway.Classes
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
    }
}
