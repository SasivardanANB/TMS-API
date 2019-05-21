using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DataGateway.Repositories.Interfaces
{
    public interface IGate
    {
        GateResponse CreateGateInGateOut(GateRequest gateRequest);
        GateResponse GetGateList(GateRequest gateRequest);
    }
}
