using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.DomainObjects.Enum
{
    enum OrderStatus
    {
        Booked = 1,
        Confirmed = 2,
        Assigned = 3,
        StartTrip = 4,
        ConfirmArrived = 5,
        StartLoad = 6,
        FinishLoad = 7,
        ConfirmPickup = 8,
        StartUnload = 9,
        FinishUnload = 10,
        POD = 11,
        Complete = 12,
        Cancel = 13,
        Billed = 14,
        DriverAccepted = 15,
        DriverRejected = 16
    }
}
