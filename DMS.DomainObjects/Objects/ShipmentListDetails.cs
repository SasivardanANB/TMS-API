using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class ShipmentListDetails
    {
        public int ID { get; set; }
        public int NumberOfBoxes { get; set; }
        public string Note { get; set; }
        public string PackingSheetNumber { get; set; }
        public int StopPointId { get; set; }        
    }
}
