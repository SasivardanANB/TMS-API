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
    public partial class BusinessVehicleTask : VehicleTask
    {
        private readonly IVehicle _vehicleRepository;

        public BusinessVehicleTask(IVehicle vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public override VehicleResponse CreateUpdateVehicle(VehicleRequest vehicleRequest)
        {
            VehicleResponse vehicleResponse = _vehicleRepository.CreateUpdateVehicle(vehicleRequest);
            return vehicleResponse;
        }

        public override VehicleResponse DeleteVehicle(int vehicleID)
        {
            VehicleResponse vehicleResponse = _vehicleRepository.DeleteVehicle(vehicleID);
            return vehicleResponse;
        }

        public override VehicleResponse GetVehicles(VehicleRequest vehicleRequest)
        {
            VehicleResponse vehicleResponse = _vehicleRepository.GetVehicles(vehicleRequest);
            return vehicleResponse;
        }
        public override CommonCodeResponse GetVehiclesPlateNumbers(string searchText)
        {
            return _vehicleRepository.GetVehiclesPlateNumbers(searchText);
        }
    }
}
