using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessGateTask : GateTask
    {
        private readonly IGate _gateRepository;

        public BusinessGateTask(IGate gateRepository)
        {
            _gateRepository = gateRepository;
        }

        public override GateResponse CreateGateInGateOut(GateRequest gateRequest)
        {
            GateResponse gateResponse = _gateRepository.CreateGateInGateOut(gateRequest);
            return gateResponse;
        }

        public override GateResponse GetGateList(GateRequest gateRequest)
        {
            GateResponse gateResponse = _gateRepository.GetGateList(gateRequest);
            return gateResponse;
        }
    }
}
