using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessPICTask : PICTask
    {
        private readonly IPIC _picRepository;

        public BusinessPICTask(IPIC picRepository)
        {
            _picRepository = picRepository;
        }

        public override PICResponse CreateUpdatePIC(PICRequest picRequest)
        {
            PICResponse transporterResponse = _picRepository.CreateUpdatePIC(picRequest);
            return transporterResponse;
        }

        public override PICResponse DeletePIC(int picId)
        {
            PICResponse transporterResponse = _picRepository.DeletePIC(picId);
            return transporterResponse;
        }

        public override PICResponse GetPICs(PICRequest picRequest)
        {
            PICResponse transporterResponse = _picRepository.GetPICs(picRequest);
            return transporterResponse;
        }
    }
}
