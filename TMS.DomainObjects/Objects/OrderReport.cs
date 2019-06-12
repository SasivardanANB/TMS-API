﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.DomainObjects.Objects
{
    public class OrderReport
    {
        public int OrderTypeId { get; set; }
        public int MainDealerId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int OrderAvgLoadingTypeId { get; set; }
        public List<OrdersByDate> OrdersByDates { get; set; }
        public List<OrderProgress> OrderProgresses { get; set; }
        public List<LoadUnloacOrdersByDate> LoadUnloacOrdersByDates { get; set; }
        public List<OrderCompletedDates> OrderCompletedDates { get; set; }
    }
    public class OrdersByDate
    {
        public int Day { get; set; }
        public int OrderCount { get; set; }
    }
    public class OrderProgress
    {
        public int OrderId { get; set; }
        public int PartnerId { get; set; }
        [MaxLength(15)]
        public string OrderNo { get; set; }
        public string Transporter { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        [MaxLength(30)]
        public string Drivername { get; set; }
        public string Vehicle { get; set; }
        public string OrderCreatedDate { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
    }
    public class OrderCompletedDates : OrderProgress
    {
        public DateTime ShippingTime { get; set; }
        public DateTime LoadingTime { get; set; }
        public DateTime TravellingTime { get; set; }
        public DateTime UnloadingTime { get; set; }
        public DateTime ETA { get; set; }
        public DateTime FinishDelivery { get; set; }
        public string ServiceRate { get; set; }
    }
    public class LoadUnloacOrdersByDate
    {
        public int Day { get; set; }
        public int OrderCount { get; set; }
        public string TotlLoadingTime { get; set; }
        public string AvgTotlLoadingTime { get; set; }
    }
}
