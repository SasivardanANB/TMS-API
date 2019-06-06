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
                                       join opd in tMSDBContext.OrderPartnerDetails on tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                                       join p in tMSDBContext.Partners on opd.PartnerID equals p.ID
                                       where opd.PartnerID== orderReportRequest.Request.MainDealerId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
                                       group oh by new { oh.OrderDate.Day } into ord
                                       select new Domain.OrdersByDate
                                       {
                                           Day = ord.Key.Day,
                                           OrderCount = ord.Count()
                                       }).ToList();
                    orderReportResponse.Data = new Domain.OrderReport()
                    {
                        OrdersByDates = orderReports,
                        Month= orderReportRequest.Request.Month,
                        Year= orderReportRequest.Request.Year,
                        OrderTypeId= orderReportRequest.Request.OrderTypeId,
                        MainDealerId= orderReportRequest.Request.MainDealerId
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
                                        where opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderType==orderReportRequest.Request.OrderTypeId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
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

        public OrderReportResponse CargoFleetOrdersAvgDayWiseReport(OrderReportRequest orderReportRequest)
        {
            OrderReportResponse orderReportResponse = new OrderReportResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var avgLoadOrderReports = (from oh in tMSDBContext.OrderHeaders
                                        join opd in tMSDBContext.OrderPartnerDetails on tMSDBContext.OrderDetails.Where(i => i.OrderHeaderID == oh.ID).Select(od => od.ID).Take(1).FirstOrDefault() equals opd.OrderDetailID
                                        join p in tMSDBContext.Partners on opd.PartnerID equals p.ID
                                        where opd.PartnerID == orderReportRequest.Request.MainDealerId && oh.OrderDate.Month == orderReportRequest.Request.Month && oh.OrderDate.Year == orderReportRequest.Request.Year
                                        group oh by new { oh.OrderDate.Day } into ord
                                        select new Domain.LoadUnloacOrdersByDate
                                        {
                                            Day = ord.Key.Day,
                                            OrderCount = ord.Count(),
                                            //TotlLoadingTime=,
                                            //AvgTotlLoadingTime=
                                        }).ToList();
                    orderReportResponse.Data = new Domain.OrderReport()
                    {
                        LoadUnloacOrdersByDates = avgLoadOrderReports,
                        Month = orderReportRequest.Request.Month,
                        Year = orderReportRequest.Request.Year,
                        OrderTypeId = orderReportRequest.Request.OrderTypeId,
                        MainDealerId = orderReportRequest.Request.MainDealerId
                    };
                    orderReportResponse.NumberOfRecords = avgLoadOrderReports.Sum(i => i.OrderCount);
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
