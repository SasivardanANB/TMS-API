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
                                        join opd in tMSDBContext.OrderPartnerDetails on orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                                        join p in tMSDBContext.Partners on opd.PartnerID equals p.ID
                                        where opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
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
                                        join opd in tMSDBContext.OrderPartnerDetails on orderReportRequest.Request.OrderTypeId==1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID)./*OrderByDescending(d=>d.ID).*/Select(od => od.ID).Take(1).FirstOrDefault(): tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                                        join p in tMSDBContext.Partners on opd.PartnerID equals p.ID
                                        where oh.OrderStatusID!=12 && opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderType==orderReportRequest.Request.OrderTypeId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
                                        select new Domain.OrderProgress
                                        {
                                            PartnerId= opd.PartnerID,
                                            OrderId =oh.ID,
                                            OrderNo=oh.OrderNo,
                                            OrderCreatedDate=oh.OrderDate.ToString(),
                                            Vehicle=tMSDBContext.VehicleTypes.Where(v=>v.ID.ToString()==oh.VehicleShipment).Select(d=>d.VehicleTypeDescription).FirstOrDefault(),
                                            Drivername=oh.DriverName,

                                            Transporter=tMSDBContext.Partners.Where(pa=> pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 1).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                            Source = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId==2).Select(pt=>pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                            Destination = tMSDBContext.Partners.Where(pa => pa.ID == tMSDBContext.OrderPartnerDetails.Where(i => i.OrderDetailID == tMSDBContext.OrderDetails.Where(o => o.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && i.PartnerTypeId == 3).Select(pt => pt.PartnerID).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                            
                                            //Transporter=tMSDBContext.Partners.Where(pa=>opd.PartnerTypeId==1 && pa.ID==opd.PartnerID && opd.OrderDetailID==tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault()).Select(pn=>pn.PartnerName).FirstOrDefault(),
                                            //Source= tMSDBContext.Partners.Where(pa => opd.OrderDetailID == tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() && opd.PartnerTypeId == 2 /*&& pa.ID==opd.PartnerID*/).Select(pn => pn.PartnerName).FirstOrDefault(),
                                            //Destination = tMSDBContext.Partners.Where(pa => opd.PartnerTypeId == 3 && pa.ID == opd.PartnerID && opd.OrderDetailID == tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault()).Select(pn => pn.PartnerName).FirstOrDefault(),
                                            OrderStatusId = oh.OrderStatusID,
                                            OrderStatus=tMSDBContext.OrderStatuses.Where(i=>i.ID==oh.OrderStatusID).Select(s=>s.OrderStatusValue).FirstOrDefault()
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
                                           join opd in tMSDBContext.OrderPartnerDetails on orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID)./*OrderByDescending(d=>d.ID).*/Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                                           join osh in tMSDBContext.OrderStatusHistories on orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID)./*OrderByDescending(d=>d.ID).*/Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals osh.OrderDetailID
                                           join p in tMSDBContext.Partners on opd.PartnerID equals p.ID
                                           where oh.OrderStatusID==12 && opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderType == orderReportRequest.Request.OrderTypeId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
                                           select new Domain.OrderCompletedDates
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
                                               
                                               OrderStatusId = oh.OrderStatusID,
                                               OrderStatus = tMSDBContext.OrderStatuses.Where(i => i.ID == oh.OrderStatusID).Select(s => s.OrderStatusValue).FirstOrDefault(),

                                               ServiceRate=oh.Harga.ToString(),
                                               LoadingTime=tMSDBContext.OrderStatusHistories.Where(o=> orderReportRequest.Request.OrderTypeId == 1 ? o.OrderStatusID==6:o.OrderStatusID==9 && o.OrderDetailID== (orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault())).Select(i=>i.StatusDate).FirstOrDefault(),
                                               UnloadingTime = tMSDBContext.OrderStatusHistories.Where(o => orderReportRequest.Request.OrderTypeId == 1 ? o.OrderStatusID == 7 : o.OrderStatusID == 10 && o.OrderDetailID == (orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault())).Select(i => i.StatusDate).FirstOrDefault(),
                                               ShippingTime = tMSDBContext.OrderStatusHistories.Where(o => o.OrderStatusID == 4 && o.OrderDetailID == (orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault())).Select(i => i.StatusDate).FirstOrDefault(),
                                               TravellingTime = tMSDBContext.OrderDetails.Where(o => o.ID == (orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault())).Select(i => i.ActualShipmentDate).FirstOrDefault(),
                                               ETA = tMSDBContext.OrderDetails.Where(o => o.ID == (orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault())).Select(i => i.EstimationShipmentDate).FirstOrDefault(),
                                               FinishDelivery = tMSDBContext.OrderStatusHistories.Where(o => o.OrderStatusID == 12 && o.OrderDetailID == (orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).OrderByDescending(d => d.ID).Select(od => od.ID).Take(1).FirstOrDefault())).Select(i => i.StatusDate).FirstOrDefault(),
                                           }).Distinct().ToList();
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
                    //var avgLoadOrderReports = (from oh in tMSDBContext.OrderHeaders
                    //                    join opd in tMSDBContext.OrderPartnerDetails on tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                    //                    join p in tMSDBContext.Partners on opd.PartnerID equals p.ID
                    //                    where opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
                    //                    group oh by new { oh.OrderDate.Day } into ord
                    //                    select new Domain.LoadUnloacOrdersByDate
                    //                    {
                    //                        Day = ord.Key.Day,
                    //                        OrderCount = ord.Count(),
                    //                        //TotlLoadingTime=,
                    //                        //AvgTotlLoadingTime=
                    //                    }).ToList();


                    int loadStatusCode, unloadStatusCode;
                    if (orderReportRequest.Request.OrderAvgLoadingTypeId == 1)
                    {

                         loadStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "6").Select(i => i.ID).FirstOrDefault();
                         unloadStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "7").Select(i => i.ID).FirstOrDefault();
                    }
                    else
                    {
                         loadStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "9").Select(i => i.ID).FirstOrDefault();
                         unloadStatusCode = tMSDBContext.OrderStatuses.Where(o => o.OrderStatusCode == "10").Select(i => i.ID).FirstOrDefault();
                    }

                    var avgLoadOrderReports = (from od in tMSDBContext.OrderDetails
                                               join opd in tMSDBContext.OrderPartnerDetails on orderReportRequest.Request.OrderTypeId == 1 ? tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == od.OrderHeaderID).Select(odetails => odetails.ID).Take(1).FirstOrDefault() : tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == od.OrderHeaderID).OrderByDescending(d => d.ID).Select(odetails => odetails.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                                               join E in (
                                                   (from B in (
                                                       (from osh in tMSDBContext.OrderStatusHistories
                                                        where
                                      osh.OrderStatusID == unloadStatusCode
                                                        select new
                                                        {
                                                            osh.OrderDetailID,
                                                            osh.OrderStatusID,
                                                            osh.StatusDate
                                                        }))
                                                    join C in (
                                    (from osh in tMSDBContext.OrderStatusHistories
                                     where
                          osh.OrderStatusID == loadStatusCode
                                     select new
                                     {
                                         cd = osh.OrderDetailID,
                                         csid = osh.OrderStatusID,
                                         cdate = osh.StatusDate
                                     })) on new { OrderDetailID = B.OrderDetailID } equals new { OrderDetailID = C.cd }
                                                    select new
                                                    {
                                                        B,
                                                        C
                                                    })) on new { ID = od.ID } equals new { ID = E.B.OrderDetailID }
                                               where
                                               opd.PartnerID == orderReportRequest.Request.MainDealerId &&
                                                 od.OrderHeader.OrderDate.Month == orderReportRequest.Request.Month &&
                                                 od.OrderHeader.OrderDate.Year == orderReportRequest.Request.Year
                                               group new { od.OrderHeader, E } by new
                                               {
                                                   Column1 = (int?)od.OrderHeader.OrderDate.Day
                                               } into g
                                               select new Domain.LoadUnloacOrdersByDate
                                               {
                                                   OrderCount = g.Count(),
                                                   TotlLoadingTime = g.Sum(p => DbFunctions.DiffHours(p.E.C.cdate, p.E.B.StatusDate)).ToString(),
                                                   Day = g.Key.Column1.Value,
                                                   AvgTotlLoadingTime = g.Sum(p => (DbFunctions.DiffHours(p.E.C.cdate, p.E.B.StatusDate)) / g.Count()).ToString()
                                               }).Distinct().ToList();

                    List<Domain.LoadUnloacOrdersByDate> loadUnloacOrdersByDates = new List<Domain.LoadUnloacOrdersByDate>();

                    for (int dayValue = 1; dayValue <= DateTime.DaysInMonth(orderReportRequest.Request.Year, orderReportRequest.Request.Month); dayValue++)
                    {
                        Domain.LoadUnloacOrdersByDate loadUnloacOrdersByDate = new Domain.LoadUnloacOrdersByDate();
                        if (avgLoadOrderReports.Any(i => i.Day == dayValue))
                        {
                            loadUnloacOrdersByDate.Day = dayValue;
                            loadUnloacOrdersByDate.OrderCount = avgLoadOrderReports.Where(i => i.Day == dayValue).Select(d => d.OrderCount).FirstOrDefault();
                            loadUnloacOrdersByDate.TotlLoadingTime= avgLoadOrderReports.Where(i => i.Day == dayValue).Select(d => d.TotlLoadingTime).FirstOrDefault();
                            loadUnloacOrdersByDate.AvgTotlLoadingTime= avgLoadOrderReports.Where(i => i.Day == dayValue).Select(d => d.AvgTotlLoadingTime).FirstOrDefault();
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
    }
}
