using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = TMS.DomainObjects.Objects;

namespace TMS.DataGateway.Repositories.Iterfaces
{
    public interface IPIC
    {
        PICResponse CreateUpdatePIC(PICRequest picRequest);
        PICResponse DeletePIC(int picId);
        PICResponse GetPICs(PICRequest picRequest);
    }
}
