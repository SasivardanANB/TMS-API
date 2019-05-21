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
    public abstract class GateTask : IGateTask
    {
        public abstract GateResponse CreateGateInGateOut(GateRequest gateRequest);
        public abstract GateResponse GetGateList(GateRequest gateRequest);
    }
}
