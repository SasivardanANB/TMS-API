using OMS.API.Classes;
using OMS.DomainGateway.Gateway.Interfaces;
using OMS.DomainGateway.Task.Interfaces;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System.Web.Http;

namespace OMS.API.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("api/v1/master")]
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
