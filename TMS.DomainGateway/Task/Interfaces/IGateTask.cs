﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IGateTask
    {
        GateResponse CreateGateInGateOut(GateRequest gateRequest);
        GateResponse GetGateList(GateRequest gateRequest);
    }
}
