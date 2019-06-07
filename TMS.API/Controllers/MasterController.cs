using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TMS.API.Classes;
using TMS.DomainGateway.Gateway.Interfaces;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/master")]
    public class MasterController : ApiController
    {
        [Route("getpartners")]
        [HttpPost]
        public IHttpActionResult GetPartners(PartnerSearchRequest partnerSearchRequest)
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            PartnerSearchResponse partnerSearchResponse = masterTask.GetPartners(partnerSearchRequest);
            return Ok(partnerSearchResponse);
        }

        [Route("getpartnerdetails")]
        [HttpGet]
        public IHttpActionResult GetPartnersDetails(int partnerId)
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            PartnerDetilasResponse partnerDetilasResponse = masterTask.GetPartnersDetails(partnerId);
            return Ok(partnerDetilasResponse);
        }


        [Route("getdrivernames")]
        [HttpGet]
        public IHttpActionResult GetDriverNames()
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonCodeResponse commonResponse = masterTask.GetDriverNames();
            return Ok(commonResponse);
        }

        [Route("getvehicletypenames")]
        [HttpGet]
        public IHttpActionResult GetVehicleTypeNames()
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetVehicleTypeNames();
            return Ok(commonResponse);
        }

        [Route("getfleettypenames")]
        [HttpGet]
        public IHttpActionResult GetFleetTypeNames()
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetFleetTypeNames();
            return Ok(commonResponse);
        }

        [Route("getsubdistrictdetails")]
        [HttpGet]
        public IHttpActionResult GetSubDistrictDetails(string searchText="")
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            SubDistrictDetailsResponse subDistrictDetailsResponse = masterTask.GetSubDistrictDetails(searchText);
            return Ok(subDistrictDetailsResponse);
        }

        [Route("getpoolnames")]
        [HttpGet]
        public IHttpActionResult GetPoolNames(string searchText = "")
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetPoolNames(searchText);
            return Ok(commonResponse);
        }

        [Route("getshippernames")]
        [HttpGet]
        public IHttpActionResult GetShipperNames(string searchText = "")
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetShipperNames(searchText);
            return Ok(commonResponse);
        }

        [Route("getcitynames")]
        [HttpGet]
        public IHttpActionResult GetCityNames(string searchText = "")
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetCityNames(searchText);
            return Ok(commonResponse);
        }

        [Route("getgatenames")]
        [HttpGet]
        public IHttpActionResult GetGateNamesByBusinessArea(int businessAreaId,int gateTypeId)
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetGateNamesByBusinessArea(businessAreaId, gateTypeId);
            return Ok(commonResponse);
        }

        [Route("gettripstatusnames")]
        [HttpGet]
        public IHttpActionResult GetTripStatusNames()
        {
            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            CommonResponse commonResponse = masterTask.GetTripStatusNames();
            return Ok(commonResponse);
        }
    }
}
