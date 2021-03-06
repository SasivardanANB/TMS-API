﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.DomainObjects.Objects
{
    public class TripStatusEventLog
    {
        public int ID { get; set; }
        public int StopPointId { get; set; }
        public DateTime StatusDate { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }
        public int TripStatusId { get; set; }
        public string LocationName { get; set; }
        public List<string> ShipmentImageGuIds { get; set; }
        public bool? IsLoad { get; set; }
        public int ImageTypeCode { get; set; }
        public int SequenceNumber { get; set; }

    }
}
