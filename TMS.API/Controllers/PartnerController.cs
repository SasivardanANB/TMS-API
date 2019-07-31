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
    [RoutePrefix("api/v1/partner")]
    public class PartnerController : ApiController
    {
        [Route("createupdatepartner")]
        [HttpPost]
        public IHttpActionResult CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IPartnerTask partnerTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().PartnerTask;
            PartnerResponse partnerResponse = partnerTask.CreateUpdatePartner(partnerRequest);
            return Ok(partnerResponse);
        }

        [Route("deletepartner")]
        [HttpDelete]
        public IHttpActionResult DeletePartner(int partnerId)
        {
            if (partnerId > 0)
            {
                IPartnerTask partnerTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().PartnerTask;
                PartnerResponse partnerResponse = partnerTask.DeletePartner(partnerId);
                return Ok(partnerResponse);
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("getpartners")]
        [HttpPost]
        public IHttpActionResult GetPartners(PartnerRequest partnerRequest)
        {
            var tmsToken = Request.Headers.GetValues("Token").FirstOrDefault();
            partnerRequest.Token = tmsToken;
            IPartnerTask partnerTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().PartnerTask;
            PartnerResponse partnerResponse = partnerTask.GetPartners(partnerRequest);
            return Ok(partnerResponse);
        }
    }
}
