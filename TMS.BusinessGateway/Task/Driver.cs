using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessDriverTask : DriverTask
    {
        private readonly IDriver _driverRepository;

        public BusinessDriverTask(IDriver userRepository)
        {
            _driverRepository = userRepository;
        }

        public override DriverResponse CreateUpdateDriver(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = _driverRepository.CreateUpdateDriver(driverRequest);
            return driverResponse;
        }

        public override DriverResponse DeleteDriver(int driverID)
        {
            DriverResponse driverResponse = _driverRepository.DeleteDriver(driverID);
            return driverResponse;
        }

        public override DriverResponse GetDrivers(DriverRequest driverRequest)
        {
            DriverResponse driverResponse = _driverRepository.GetDrivers(driverRequest);
            return driverResponse;
        }
    }
}
