using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.UI.WebControls;
using TMS.BusinessGateway.Classes;
using TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessDriverTask : DriverTask
    {
        private readonly IDriver _driverRepository;

        public BusinessDriverTask(IDriver userRepository)
        {
            _driverRepository = userRepository;
        }

        public override DriverResponse CreateUpdateDriver(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = _driverRepository.CreateUpdateDriver(driverRequest);
            return driverResponse;
        }

        public override DriverResponse DeleteDriver(int driverID)
        {
            DriverResponse driverResponse = _driverRepository.DeleteDriver(driverID);
            #region delete driver in DMS
            string username = string.Empty;
            using (var tMSDBContext = new TMSDBContext())
            {
                Driver driverDetails = tMSDBContext.Drivers.Where(i => i.ID == driverID).FirstOrDefault();
                username = driverDetails.UserName;
            }
            string JsonBody = "{\"Requests\": [{\"ID\": " + driverID + ",\"IsActive\": 0,\"UserName\": '" + username + "'}]}";

            string Response = Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                        + "v1/user/createupdateuser", Method.POST, JsonBody, null);
            #endregion
            return driverResponse;
        }

        public override DriverResponse GetDrivers(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = _driverRepository.GetDrivers(driverRequest);
            return driverResponse;
        }
    }
}
