using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class AdminBoardReport
    {
        public string OrderType { get; set; }
        public List<OrdersInDay> OrdersInDays { get; set; }
        public List<AssignmentInDay> AssignmentInDays { get; set; }
        public List<JalanInDay> JalanInDays { get; set; }
        public List<GatinInDay> GatinInDays { get; set; }
        public List<BongkarInDay> BongkarInDays { get; set; }
        public List<FinishInDay> FinishInDays { get; set; }
        public List<MuatInDay> MuatInDays { get; set; }
    }
    public class OrdersInDay
    {
        public string OrderNumber { get; set; }
        public string Pallet { get; set; }
    }
    public class AssignmentInDay
    {
        public string VehicleNumber { get; set; }
        public string Pallet { get; set; }
    }
    public class JalanInDay
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class GatinInDay
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class BongkarInDay
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class FinishInDay
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class MuatInDay
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
}
