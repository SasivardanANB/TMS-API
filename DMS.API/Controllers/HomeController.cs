using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DMS.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/home")]
    public class HomeController : ApiController
    {
        [Route("index")]
        [AllowAnonymous, HttpGet]
        public string Index()
        {
            return "Home/Index";
        }
    }
}
