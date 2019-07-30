using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using Domain = TMS.DomainObjects.Objects;
using DataModel = TMS.DataGateway.DataModels;
using System.Data.Entity;

namespace TMS.DataGateway.Repositories
{
    public class Report : IReport
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderReportResponse OrdersDayWiseReport(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = new OrderReportResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var orderReports = (from oh in tMSDBContext.OrderHeaders
                                        join opd in tMSDBContext.OrderPartnerDetails on tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).FirstOrDefault() equals opd.OrderDetailID
                                        where opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year && oh.OrderType == orderReportRequest.Request.OrderTypeId
                                        group oh by new { oh.OrderDate.Day } into ord
                                        select new Domain.OrdersByDate
                                        {
                                            Day = ord.Key.Day,
                                            OrderCount = ord.Count()
                                        }).ToList();

                    List<Domain.OrdersByDate> ordersByDates = new List<Domain.OrdersByDate>();

                    for (int dayValue = 1; dayValue <= DateTime.DaysInMonth(orderReportRequest.Request.Year, orderReportRequest.Request.Month); dayValue++)
                    {
                        Domain.OrdersByDate ordersByDate = new Domain.OrdersByDate();
                        if (orderReports.Any(i => i.Day == dayValue))
                        {
                            ordersByDate.Day = dayValue;
                            ordersByDate.OrderCount = orderReports.Where(i => i.Day == dayValue).Select(d => d.OrderCount).FirstOrDefault();
                        }
                        else
                        {
                            ordersByDate.Day = dayValue;
                            ordersByDate.OrderCount = 0;
                        }
                        ordersByDates.Add(ordersByDate);
                    }

                    orderReportResponse.Data = new Domain.OrderReport()
                    {
                        OrdersByDates = ordersByDates,
                        Month = orderReportRequest.Request.Month,
                        Year = orderReportRequest.Request.Year,
                        OrderTypeId = orderReportRequest.Request.OrderTypeId,
                        MainDealerId = orderReportRequest.Request.MainDealerId
                    };
                    orderReportResponse.NumberOfRecords = orderReports.Sum(i => i.OrderCount);
                    orderReportResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    orderReportResponse.StatusCode = (int)HttpStatusCode.OK;
                    orderReportResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderReportResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderReportResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderReportResponse.StatusMessage = ex.Message;
            }
            return orderReportResponse;
        }

        public OrderReportResponse OrdersProgress(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = new OrderReportResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var orderprogresses = (from oh in tMSDBContext.OrderHeaders
                                           join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                           join opd in tMSDBContext.OrderPartnerDetails on tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).FirstOrDefault() equals opd.OrderDetailID
                                           where oh.OrderStatusID != 12 && oh.OrderType == orderReportRequest.Request.OrderTypeId && opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
                                           select new Domain.OrderProgress
                                           {
                                               PartnerId = opd.PartnerID,
                                               OrderId = oh.ID,
                                               OrderNo = oh.OrderNo,
                                               OrderCreatedDate = oh.OrderDate.ToString(),
                                               Vehicle = tMSDBContext.VehicleTypes.Where(v => v.ID.ToString() == oh.VehicleShipment).Select(d => d.VehicleTypeDescription).FirstOrDefault(),
                                               Drivername = oh.DriverName,

                                               Transporter = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 1).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                               Source = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 2).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                               Destination = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 3).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),

                                               //Transporter=tMSDBContext.Partners.Where(pa=>opd.PartnerTypeId==1 && pa.ID==opd.PartnerID && opd.OrderDetailID==tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault()).Select(pn=>pn.PartnerName).FirstOrDefault(),
                                               //Source= tMSDBContext.Partners.Where(pa => opd.OrderDetailID == tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && opd.PartnerTypeId == 2 /*&& pa.ID==opd.PartnerID*/).Select(pn => pn.PartnerName).FirstOrDefault(),
                                               //Destination = tMSDBContext.Partners.Where(pa => opd.PartnerTypeId == 3 && pa.ID == opd.PartnerID && opd.OrderDetailID == tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                               OrderStatusId = oh.OrderStatusID,
                                               OrderStatus = tMSDBContext.OrderStatuses.Where(i => i.ID == oh.OrderStatusID).Select(s => s.OrderStatusValue).FirstOrDefault()
                                           }).Distinct().ToList();
                    orderReportResponse.Data = new Domain.OrderReport()
                    {
                        OrderProgresses = orderprogresses,
                        Month = orderReportRequest.Request.Month,
                        Year = orderReportRequest.Request.Year,
                        OrderTypeId = orderReportRequest.Request.OrderTypeId,
                        MainDealerId = orderReportRequest.Request.MainDealerId
                    };
                    orderReportResponse.NumberOfRecords = orderprogresses.Count;
                    orderReportResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    orderReportResponse.StatusCode = (int)HttpStatusCode.OK;
                    orderReportResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderReportResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderReportResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderReportResponse.StatusMessage = ex.Message;
            }
            return orderReportResponse;
        }

        public OrderReportResponse FinishedOrderReports(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = new OrderReportResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var comletedOrderprogresses = (from oh in tMSDBContext.OrderHeaders
                                                   join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                                   join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                   join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                   where oh.OrderStatusID == 12 && opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                   oh.OrderType == orderReportRequest.Request.OrderTypeId && oh.OrderDate.Month == orderReportRequest.Request.Month &&
                                                   oh.OrderDate.Year == orderReportRequest.Request.Year
                                                   group new { oh, od, opd, osh } by new { oh.ID } into ord
                                                   select new Domain.OrderCompletedDates
                                                   {
                                                       PartnerId = ord.Select(p => p.opd.PartnerID).FirstOrDefault(),
                                                       OrderId = ord.Select(p => p.oh.ID).FirstOrDefault(),
                                                       OrderNo = ord.Select(p => p.oh.OrderNo).FirstOrDefault(),
                                                       Transporter = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == ord.Select(id => id.oh.ID).FirstOrDefault()).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 1).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                                       //Source = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == ord.Select(id => id.oh.ID).FirstOrDefault()).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 2).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                                       //Destination = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == ord.Select(id => id.oh.ID).FirstOrDefault()).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 3).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                                       Drivername = ord.Select(p => p.oh.DriverName).FirstOrDefault(),
                                                       OrderCreatedDate = ord.Select(p => p.oh.OrderDate).FirstOrDefault().ToString(),
                                                       //Vehicle = tMSDBContext.VehicleTypes.Where(v => v.ID.ToString() == ord.Select(p => p.oh.VehicleShipment).FirstOrDefault()).Select(d => d.VehicleTypeDescription).FirstOrDefault(),

                                                       //ShippingTime = DbFunctions.DiffHours(ord.Where(i => i.osh.ID == 12).Select(o => o.osh.StatusDate).FirstOrDefault(), ord.Where(i => i.osh.ID == 4 && i.osh.IsLoad == false).Select(o => o.osh.StatusDate).FirstOrDefault()).ToString(),
                                                       ETA = ord.OrderByDescending(i => i.od.ID).Select(o => o.od.EstimationShipmentDate).FirstOrDefault().ToString(),
                                                       FinishDelivery = ord.Where(od => od.osh.OrderStatusID == 12).Select(o => o.osh.StatusDate).FirstOrDefault().ToString(),
                                                       //ServiceRate = oh.Harga.ToString(),                                                       
                                                   }).ToList();

                    foreach (var item in comletedOrderprogresses)
                    {
                        item.ETA = Convert.ToDateTime(item.ETA).ToString("MMM dd yyyy hh:mmtt");
                        item.FinishDelivery = Convert.ToDateTime(item.FinishDelivery).ToString("MMM dd yyyy hh:mmtt");

                        decimal loadingTime = 0;
                        decimal unLoadingTime = 0;
                        decimal travellingTime = 0;
                        var orderDetails = tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == item.OrderId).ToList();
                        foreach (var orderItem in orderDetails)
                        {
                            var orderDetailLoadStartTripTime = (from od in tMSDBContext.OrderDetails
                                                                join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                                join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                                where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                                osh.OrderStatusID == 4 && osh.IsLoad == true
                                                                select new
                                                                {
                                                                    Date = osh.StatusDate
                                                                }).FirstOrDefault();

                            var orderDetailLoadConfirmArriveTime = (from od in tMSDBContext.OrderDetails
                                                                    join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                                    join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                                    where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                                    osh.OrderStatusID == 5 && osh.IsLoad == true
                                                                    select new
                                                                    {
                                                                        Date = osh.StatusDate
                                                                    }).FirstOrDefault();

                            var orderDetailUnLoadStartTripTime = (from od in tMSDBContext.OrderDetails
                                                                  join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                                  join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                                  where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                                  osh.OrderStatusID == 4 && osh.IsLoad == false
                                                                  select new
                                                                  {
                                                                      Date = osh.StatusDate
                                                                  }).FirstOrDefault();

                            var orderDetailUnLoadConfirmArriveTime = (from od in tMSDBContext.OrderDetails
                                                                      join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                                      join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                                      where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                                      osh.OrderStatusID == 5 && osh.IsLoad == false
                                                                      select new
                                                                      {
                                                                          Date = osh.StatusDate
                                                                      }).FirstOrDefault();

                            var orderDetailStartLoadingTime = (from od in tMSDBContext.OrderDetails
                                                               join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                               join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                               where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId && osh.OrderStatusID == 6
                                                               select new
                                                               {
                                                                   Date = osh.StatusDate
                                                               }).FirstOrDefault();

                            var orderDetailEndLoaingTime = (from od in tMSDBContext.OrderDetails
                                                            join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                            join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                            where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId && osh.OrderStatusID == 7
                                                            select new
                                                            {
                                                                Date = osh.StatusDate
                                                            }).FirstOrDefault();

                            var orderDetailStartUnLoadingTime = (from od in tMSDBContext.OrderDetails
                                                                 join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                                 join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                                 where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId && osh.OrderStatusID == 9
                                                                 select new
                                                                 {
                                                                     Date = osh.StatusDate
                                                                 }).FirstOrDefault();

                            var orderDetailEndUnLoaingTime = (from od in tMSDBContext.OrderDetails
                                                              join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                              join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                              where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId && osh.OrderStatusID == 10
                                                              select new
                                                              {
                                                                  Date = osh.StatusDate
                                                              }).FirstOrDefault();

                            var orderCompletedTime = (from od in tMSDBContext.OrderDetails
                                                      join opd in tMSDBContext.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                                      join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                                      where od.ID == orderItem.ID && opd.PartnerID == orderReportRequest.Request.MainDealerId && osh.OrderStatusID == 12
                                                      select new
                                                      {
                                                          Date = osh.StatusDate
                                                      }).FirstOrDefault();



                            if (orderDetailEndLoaingTime != null && orderDetailStartLoadingTime != null)
                            {
                                loadingTime += Convert.ToDecimal((orderDetailEndLoaingTime.Date - orderDetailStartLoadingTime.Date).TotalHours);
                            }
                            if (orderDetailEndUnLoaingTime != null && orderDetailStartUnLoadingTime != null)
                            {
                                unLoadingTime += Convert.ToDecimal((orderDetailEndUnLoaingTime.Date - orderDetailStartUnLoadingTime.Date).TotalHours);
                            }
                            if (orderCompletedTime != null && orderDetailLoadStartTripTime != null)
                            {
                                item.ShippingTime = Math.Round(Convert.ToDecimal((orderCompletedTime.Date - orderDetailLoadStartTripTime.Date).TotalHours), 2).ToString();
                            }
                            decimal travelLoadTime = 0;
                            decimal travelUnLoadTime = 0;
                            if (orderDetailLoadConfirmArriveTime != null && orderDetailLoadStartTripTime != null)
                            {
                                travelLoadTime = Convert.ToDecimal((orderDetailLoadConfirmArriveTime.Date - orderDetailLoadStartTripTime.Date).TotalHours);
                            }
                            if (orderDetailUnLoadConfirmArriveTime != null && orderDetailUnLoadStartTripTime != null)
                            {
                                travelUnLoadTime = Convert.ToDecimal((orderDetailUnLoadConfirmArriveTime.Date - orderDetailUnLoadStartTripTime.Date).TotalHours);
                            }
                            travellingTime += travelLoadTime + travelUnLoadTime;
                        }
                        item.LoadingTime = Math.Round(loadingTime, 2).ToString();
                        item.UnloadingTime = Math.Round(unLoadingTime, 2).ToString();
                        item.TravellingTime = Math.Round(travellingTime, 2).ToString();
                    }

                    orderReportResponse.Data = new Domain.OrderReport()
                    {
                        OrderCompletedDates = comletedOrderprogresses,
                        Month = orderReportRequest.Request.Month,
                        Year = orderReportRequest.Request.Year,
                        OrderTypeId = orderReportRequest.Request.OrderTypeId,
                        MainDealerId = orderReportRequest.Request.MainDealerId
                    };
                    orderReportResponse.NumberOfRecords = comletedOrderprogresses.Count;
                    orderReportResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    orderReportResponse.StatusCode = (int)HttpStatusCode.OK;
                    orderReportResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderReportResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderReportResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderReportResponse.StatusMessage = ex.Message;
            }
            return orderReportResponse;
        }

        public OrderReportResponse OrdersLoadAndUnloadAvgDayWiseReport(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = new OrderReportResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    int startStatusCode, finishStatusCode, partnerTypeId;

                    if (orderReportRequest.Request.OrderAvgLoadingTypeId == 1)
                    {
                        // Loading
                        startStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "6").Select(i => i.ID).FirstOrDefault();
                        finishStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "7").Select(i => i.ID).FirstOrDefault();
                        partnerTypeId = 2;
                    }
                    else
                    {
                        // Unloading
                        startStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "9").Select(i => i.ID).FirstOrDefault();
                        finishStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "10").Select(i => i.ID).FirstOrDefault();
                        partnerTypeId = 3;
                    }

                    var avgLoadOrderReports = (from opd in tMSDBContext.OrderPartnerDetails
                                               join E in
                                               (
                                                    from B in
                                                    (
                                                        from osh in tMSDBContext.OrderStatusHistories
                                                        where osh.OrderStatusID == finishStatusCode
                                                        select new
                                                        {
                                                            osh.OrderDetailID,
                                                            osh.OrderStatusID,
                                                            osh.StatusDate
                                                        }
                                                    )
                                                    join C in
                                                    (
                                                        from osh in tMSDBContext.OrderStatusHistories
                                                        where osh.OrderStatusID == startStatusCode
                                                        select new
                                                        {
                                                            cd = osh.OrderDetailID,
                                                            csid = osh.OrderStatusID,
                                                            cdate = osh.StatusDate
                                                        }
                                                    ) on new { B.OrderDetailID } equals new { OrderDetailID = C.cd }
                                                    select new
                                                    {
                                                        B,
                                                        C
                                                    }
                                               ) on new { opd.OrderDetail.ID } equals new { ID = E.B.OrderDetailID }
                                               where
                                                 opd.OrderDetail.OrderHeader.OrderDate.Month == orderReportRequest.Request.Month &&
                                                 opd.OrderDetail.OrderHeader.OrderDate.Year == orderReportRequest.Request.Year &&
                                                 opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                 opd.PartnerTypeId == partnerTypeId
                                               group new { opd.OrderDetail.OrderHeader, E } by new
                                               {
                                                   Column1 = (int?)opd.OrderDetail.OrderHeader.OrderDate.Day
                                               } into g
                                               select new Domain.LoadUnloacOrdersByDate
                                               {
                                                   OrderCount = g.Count(),
                                                   TotlLoadingTime = (g.Sum(p => DbFunctions.DiffMinutes(p.E.C.cdate, p.E.B.StatusDate)) / 60.0).ToString(),
                                                   Day = g.Key.Column1.Value,
                                                   AvgTotlLoadingTime = g.Sum(p => (DbFunctions.DiffMinutes(p.E.C.cdate, p.E.B.StatusDate) / g.Count()) / 60.0).ToString()
                                               }).ToList();



                    List<Domain.LoadUnloacOrdersByDate> loadUnloacOrdersByDates = new List<Domain.LoadUnloacOrdersByDate>();

                    for (int dayValue = 1; dayValue <= DateTime.DaysInMonth(orderReportRequest.Request.Year, orderReportRequest.Request.Month); dayValue++)
                    {
                        Domain.LoadUnloacOrdersByDate loadUnloacOrdersByDate = new Domain.LoadUnloacOrdersByDate();
                        if (avgLoadOrderReports.Any(i => i.Day == dayValue))
                        {
                            loadUnloacOrdersByDate.Day = dayValue;
                            loadUnloacOrdersByDate.OrderCount = avgLoadOrderReports.Where(i => i.Day == dayValue).Select(d => d.OrderCount).FirstOrDefault();
                            loadUnloacOrdersByDate.TotlLoadingTime = Math.Round(decimal.Parse(avgLoadOrderReports.Where(i => i.Day == dayValue).Select(d => d.TotlLoadingTime).FirstOrDefault()), 2).ToString();
                            loadUnloacOrdersByDate.AvgTotlLoadingTime = Math.Round(decimal.Parse(avgLoadOrderReports.Where(i => i.Day == dayValue).Select(d => d.AvgTotlLoadingTime).FirstOrDefault()), 2).ToString();
                        }
                        else
                        {
                            loadUnloacOrdersByDate.Day = dayValue;
                            loadUnloacOrdersByDate.OrderCount = 0;
                            loadUnloacOrdersByDate.TotlLoadingTime = "0";
                            loadUnloacOrdersByDate.AvgTotlLoadingTime = "0";
                        }
                        loadUnloacOrdersByDates.Add(loadUnloacOrdersByDate);
                    }

                    orderReportResponse.Data = new Domain.OrderReport()
                    {
                        LoadUnloacOrdersByDates = loadUnloacOrdersByDates,
                        Month = orderReportRequest.Request.Month,
                        Year = orderReportRequest.Request.Year,
                        OrderTypeId = orderReportRequest.Request.OrderTypeId,
                        MainDealerId = orderReportRequest.Request.MainDealerId
                    };
                    orderReportResponse.NumberOfRecords = loadUnloacOrdersByDates.Sum(i => i.OrderCount);
                    orderReportResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    orderReportResponse.StatusCode = (int)HttpStatusCode.OK;
                    orderReportResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderReportResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderReportResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderReportResponse.StatusMessage = ex.Message;
            }
            return orderReportResponse;
        }

        // For generating either goods issue and goods receive reports based on request
        public GoodsReceiveOrIssueResponse GoodsReceiveOrGoodsIssueReport(GoodsReceiveOrIssueRequest goodsReceiveOrIssueRequest)
        {
            GoodsReceiveOrIssueResponse goodsReceiveOrIssueResponse = new GoodsReceiveOrIssueResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var goodsReceiveOrIssueData = (from A in (from opd in tMSDBContext.OrderPartnerDetails
                                                              where
                                                                opd.OrderDetail.OrderHeader.OrderType == goodsReceiveOrIssueRequest.Request.OrderTypeId &&
                                                                opd.PartnerID == goodsReceiveOrIssueRequest.Request.PartnerId &&
                                                                DbFunctions.TruncateTime(opd.OrderDetail.OrderHeader.OrderDate) >= DbFunctions.TruncateTime(goodsReceiveOrIssueRequest.Request.StartDate) &&
                                                                DbFunctions.TruncateTime(opd.OrderDetail.OrderHeader.OrderDate) <= DbFunctions.TruncateTime(goodsReceiveOrIssueRequest.Request.EndDate)
                                                              group new { opd.OrderDetail.OrderHeader, opd.OrderDetail } by new
                                                              {
                                                                  Column1 = DbFunctions.TruncateTime(opd.OrderDetail.OrderHeader.OrderDate)
                                                              } into g
                                                              select new
                                                              {
                                                                  Order_Qty = g.Sum(p => p.OrderDetail.TotalCollie),
                                                                  CreatedDate = g.Key.Column1
                                                              })
                                                   join B in (from opd in tMSDBContext.OrderPartnerDetails
                                                              join osh in tMSDBContext.OrderStatusHistories on opd.OrderDetail.ID equals osh.OrderDetailID
                                                              where
                                                                opd.OrderDetail.OrderHeader.OrderType == goodsReceiveOrIssueRequest.Request.OrderTypeId &&
                                                                opd.PartnerID == goodsReceiveOrIssueRequest.Request.PartnerId && osh.OrderStatusID == 12 &&
                                                                DbFunctions.TruncateTime(osh.StatusDate) >= DbFunctions.TruncateTime(goodsReceiveOrIssueRequest.Request.StartDate) &&
                                                                DbFunctions.TruncateTime(osh.StatusDate) <= DbFunctions.TruncateTime(goodsReceiveOrIssueRequest.Request.EndDate)
                                                              group new { osh, opd.OrderDetail } by new
                                                              {
                                                                  Column1 = DbFunctions.TruncateTime(osh.StatusDate)
                                                              } into g
                                                              select new
                                                              {
                                                                  Order_Qty = g.Sum(p => p.OrderDetail.TotalCollie),
                                                                  CreatedDate = g.Key.Column1
                                                              })
                                                               on new { CreatedDate = DbFunctions.TruncateTime(A.CreatedDate).Value } equals new { CreatedDate = DbFunctions.TruncateTime(B.CreatedDate).Value } into B_join
                                                   where A.Order_Qty > 0
                                                   from B in B_join.DefaultIfEmpty()
                                                   select new Domain.GoodsReceiveOrIssue
                                                   {
                                                       OrderQty = A.Order_Qty.ToString(),
                                                       GRQty = goodsReceiveOrIssueRequest.Request.OrderTypeId == 1 ? B.Order_Qty.ToString() : "0",
                                                       GIQty = goodsReceiveOrIssueRequest.Request.OrderTypeId == 2 ? B.Order_Qty.ToString() : "0",
                                                       Percentage = Math.Round(((B.Order_Qty * 100.00) / A.Order_Qty), 2).ToString(),
                                                       Date = (A.CreatedDate ?? B.CreatedDate.Value).ToString()
                                                   }).ToList();

                    foreach (Domain.GoodsReceiveOrIssue g in goodsReceiveOrIssueData)
                        g.Date = Convert.ToDateTime(g.Date).ToString("dd.MM.yyyy");

                    goodsReceiveOrIssueResponse.NumberOfRecords = goodsReceiveOrIssueData.Count;
                    goodsReceiveOrIssueResponse.Data = new Domain.GoodsReceiveOrIssueReport()
                    {
                        StartDate = goodsReceiveOrIssueRequest.Request.StartDate,
                        EndDate = goodsReceiveOrIssueRequest.Request.EndDate,
                        GoodsReceiveOrIssues = goodsReceiveOrIssueData,
                        OrderTypeId = goodsReceiveOrIssueRequest.Request.OrderTypeId,
                        PartnerId = goodsReceiveOrIssueRequest.Request.PartnerId
                    };
                    goodsReceiveOrIssueResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    goodsReceiveOrIssueResponse.StatusCode = (int)HttpStatusCode.OK;
                    goodsReceiveOrIssueResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                goodsReceiveOrIssueResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                goodsReceiveOrIssueResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                goodsReceiveOrIssueResponse.StatusMessage = ex.Message;
            }
            return goodsReceiveOrIssueResponse;
        }

        public AdminBoardReportResponse BoardAdminReport(int orderTypeId)
        {
            AdminBoardReportResponse adminBoardReportResponse = new AdminBoardReportResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var ordersInDay = (from oh in tMSDBContext.OrderHeaders
                                       join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                       where oh.OrderType == orderTypeId && DbFunctions.TruncateTime(oh.OrderDate) == DbFunctions.TruncateTime(DateTime.Now)
                                       group new { oh, od } by new { oh.ID } into ord
                                       select new Domain.OrdersInDay
                                       {
                                           Pallet = ord.Sum(p => p.od.TotalPallet).ToString(),
                                           OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                       }).ToList();

                    var assignmentInDay = (from oh in tMSDBContext.OrderHeaders
                                           join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                           join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                           where oh.OrderType == orderTypeId && osh.OrderStatusID == 3 && DbFunctions.TruncateTime(osh.StatusDate) == DbFunctions.TruncateTime(DateTime.Now)
                                           group new { oh, od } by new { oh.ID } into ord
                                           select new Domain.AssignmentInDay
                                           {
                                               Pallet = ord.Sum(p => p.od.TotalPallet).ToString(),
                                               VehicleNumber = ord.Select(or => or.oh.VehicleNo).FirstOrDefault(),
                                               OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                           }).ToList();

                    var finishInDay = (from oh in tMSDBContext.OrderHeaders
                                       join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                       join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                       where oh.OrderType == orderTypeId && osh.OrderStatusID == 12 && DbFunctions.TruncateTime(osh.StatusDate) == DbFunctions.TruncateTime(DateTime.Now)
                                       group new { oh, od } by new { oh.ID } into ord
                                       select new Domain.FinishInDay
                                       {
                                           Collie = ord.Sum(p => p.od.TotalCollie).ToString(),
                                           VehicleNumber = ord.Select(or => or.oh.VehicleNo).FirstOrDefault(),
                                           OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                       }).ToList();

                    var bongkarInDay = (from oh in tMSDBContext.OrderHeaders
                                        join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                        join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                        where oh.OrderType == orderTypeId && osh.OrderStatusID == 10 && DbFunctions.TruncateTime(osh.StatusDate) == DbFunctions.TruncateTime(DateTime.Now)
                                        group new { oh, od } by new { oh.ID } into ord
                                        select new Domain.BongkarInDay
                                        {
                                            Collie = ord.Sum(p => p.od.TotalCollie).ToString(),
                                            VehicleNumber = ord.Select(or => or.oh.VehicleNo).FirstOrDefault(),
                                            OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                        }).ToList();

                    var muatInDay = (from oh in tMSDBContext.OrderHeaders
                                     join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                     join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                     where oh.OrderType == orderTypeId && osh.OrderStatusID == 10 && DbFunctions.TruncateTime(osh.StatusDate) == DbFunctions.TruncateTime(DateTime.Now)
                                     group new { oh, od } by new { oh.ID } into ord
                                     select new Domain.MuatInDay
                                     {
                                         Collie = ord.Sum(p => p.od.TotalCollie).ToString(),
                                         VehicleNumber = ord.Select(or => or.oh.VehicleNo).FirstOrDefault(),
                                         OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                     }).ToList();

                    var jalanInDay = (from oh in tMSDBContext.OrderHeaders
                                      join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                      join osh in tMSDBContext.OrderStatusHistories on od.ID equals osh.OrderDetailID
                                      where oh.OrderType == orderTypeId && osh.OrderStatusID == 4 && DbFunctions.TruncateTime(osh.StatusDate) == DbFunctions.TruncateTime(DateTime.Now)
                                      group new { oh, od } by new { oh.ID } into ord
                                      select new Domain.JalanInDay
                                      {
                                          Collie = ord.Sum(p => p.od.TotalCollie).ToString(),
                                          VehicleNumber = ord.Select(or => or.oh.VehicleNo).FirstOrDefault(),
                                          OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                      }).ToList();

                    var gatinInDay = (from oh in tMSDBContext.OrderHeaders
                                      join g in tMSDBContext.GateInGateOuts on oh.ID equals g.OrderId
                                      join od in tMSDBContext.OrderDetails on oh.ID equals od.OrderHeaderID
                                      group new { oh, g, od } by new { oh.ID }
                                     into ord
                                      select new Domain.GatinInDay
                                      {
                                          Collie = ord.Sum(i => i.od.TotalCollie).ToString(),
                                          VehicleNumber = ord.Select(i => i.oh.VehicleNo).FirstOrDefault(),
                                          OrderNumber = ord.Select(or => or.oh.OrderNo).FirstOrDefault()
                                      }).ToList();

                    Domain.AdminBoardReport adminBoardReport = new Domain.AdminBoardReport()
                    {
                        BongkarInDays = orderTypeId == 1 ? bongkarInDay : null,
                        MuatInDays = orderTypeId == 2 ? muatInDay : null,
                        GatinInDays = gatinInDay,
                        FinishInDays = finishInDay,
                        AssignmentInDays = assignmentInDay,
                        JalanInDays = jalanInDay,
                        OrdersInDays = ordersInDay,
                        OrderType = tMSDBContext.OrderTypes.Where(o => o.ID == orderTypeId).Select(d => d.OrderTypeDescription).FirstOrDefault()
                    };
                    if (ordersInDay.Count > 0 && assignmentInDay.Count > 0 && finishInDay.Count > 0 && gatinInDay.Count > 0 && jalanInDay.Count > 0 && muatInDay.Count > 0 && bongkarInDay.Count > 0)
                    {
                        adminBoardReportResponse.NumberOfRecords = 1;
                    }
                    adminBoardReportResponse.Data = adminBoardReport;
                    adminBoardReportResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    adminBoardReportResponse.StatusCode = (int)HttpStatusCode.OK;
                    adminBoardReportResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                adminBoardReportResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                adminBoardReportResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                adminBoardReportResponse.StatusMessage = ex.Message;
            }
            return adminBoardReportResponse;
        }
    }
}
