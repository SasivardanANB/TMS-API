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
    public class OrdersInDay: OrderNo
    {
        public string Pallet { get; set; }
    }
    public class AssignmentInDay : OrderNo
    {
        public string VehicleNumber { get; set; }
        public string Pallet { get; set; }
    }
    public class JalanInDay : OrderNo
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class GatinInDay : OrderNo
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class BongkarInDay : OrderNo
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class FinishInDay : OrderNo
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class MuatInDay : OrderNo
    {
        public string VehicleNumber { get; set; }
        public string Collie { get; set; }
    }
    public class OrderNo
    {
        public string OrderNumber { get; set; }
    }
}
