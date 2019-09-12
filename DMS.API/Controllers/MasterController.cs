using DMS.API.Classes;
using DMS.DomainGateway.Gateway.Interfaces;
using DMS.DomainGateway.Task.Interfaces;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System.Web.Http;

namespace DMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/partner")]
    public class MasterController : ApiController
    {
        [Route("createupdatepartner")]
        [HttpPost]
        public IHttpActionResult CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IMasterTask masterTask = Helper.Model.DependencyResolver.DependencyResolver.GetImplementationOf<ITaskGateway>().MasterTask;
            PartnerResponse partnerResponse = masterTask.CreateUpdatePartner(partnerRequest);
            return Ok(partnerResponse);
        }
    }
}
