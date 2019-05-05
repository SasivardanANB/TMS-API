using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainGateway.Task.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task
{
    public abstract class PICTask : IPICTask
    {
        public abstract PICResponse CreateUpdatePIC(PICRequest picRequest);
        public abstract PICResponse DeletePIC(int picId);
        public abstract PICResponse GetPICs(PICRequest picRequest);
    }
}
