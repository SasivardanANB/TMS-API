using RestSharp;
using System.Configuration;
using System.Net;
using TMS.BusinessGateway.Classes;
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

            if (driverResponse.StatusCode == (int)HttpStatusCode.OK && driverResponse.Status == DomainObjects.Resource.ResourceData.Success)
            {
                #region delete driver in DMS
                string username = driverResponse.Data[0].UserName;

                string JsonBody = "{\"Requests\": [{\"ID\": " + driverID + ",\"IsActive\": 0,\"UserName\": '" + username + "'}]}";

                string Response = Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                            + "v1/user/createupdateuser", Method.POST, JsonBody, null);
                #endregion
            }
            return driverResponse;
        }

        public override DriverResponse GetDrivers(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = _driverRepository.GetDrivers(driverRequest);
            return driverResponse;
        }
    }
}
