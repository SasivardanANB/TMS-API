using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.DomainGateway.Task.Interfaces
{
    public interface IVehicleTask
    {
        VehicleResponse CreateUpdateVehicle(VehicleRequest vehicleRequest);
        VehicleResponse DeleteVehicle(int vehicleID);
        VehicleResponse GetVehicles(VehicleRequest vehicleRequest);
        CommonCodeResponse GetVehiclesPlateNumbers(string searchText,int transporterId);
    }
}
