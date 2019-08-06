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
    public abstract class VehicleTask : IVehicleTask
    {
        public abstract VehicleResponse CreateUpdateVehicle(VehicleRequest vehicleRequest);
        public abstract VehicleResponse DeleteVehicle(int vehicleID);
        public abstract VehicleResponse GetVehicles(VehicleRequest vehicleRequest);
        public abstract CommonCodeResponse GetVehiclesPlateNumbers(string searchText, int transporterId);
    }
}
