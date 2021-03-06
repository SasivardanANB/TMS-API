﻿using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMS.DataGateway.DataModels;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using Domain = TMS.DomainObjects.Objects;
using DataModel = TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Iterfaces;


namespace TMS.DataGateway.Repositories
{
    public class Trip : ITrip
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TripResponse GetTripList(TripRequest tripRequest)
        {
            TripResponse tripResponse = new TripResponse()
            {
                Data = new List<Domain.Trip>()
            };

            List<Domain.Trip> tripList;
            List<Domain.Trip> tripListGrouped = new List<Domain.Trip>();
            try
            {
                using (var context = new DataModel.TMSDBContext())
                {
                    var userDetails = context.Tokens.Where(t => t.TokenKey == tripRequest.Token).FirstOrDefault();
                    var userData = context.Users.Where(t => t.ID == userDetails.UserID).FirstOrDefault();
                    var picID = context.Pics.Where(p => p.PICEmail == userData.Email).Select(x => x.ID).FirstOrDefault();

                    List<int> partnerDataCk = null;
                    if (picID > 0)
                    {
                        partnerDataCk = (from partner in context.Partners
                                         join ptype in context.PartnerPartnerTypes on partner.ID equals ptype.PartnerId
                                         where ptype.PartnerTypeId == 1 && partner.PICID.Value == picID
                                         select partner.ID
                                          ).ToList();
                    }

                    var businessAreas = (from ur in context.UserRoles
                                         where ur.UserID == userDetails.UserID && !ur.IsDelete
                                         select ur.BusinessAreaID).ToList();

                    if (partnerDataCk != null && partnerDataCk.Count > 0)
                    {
                        tripList = (from oh in context.OrderHeaders
                                    join od in context.OrderDetails on oh.ID equals od.OrderHeaderID
                                    join opd in context.OrderPartnerDetails on od.ID equals opd.OrderDetailID
                                    join ps in context.PackingSheets on od.ShippingListNo equals ps.ShippingListNo into pks
                                    from pksh in pks.DefaultIfEmpty()
                                    where businessAreas.Contains(oh.BusinessAreaId) && ((((!String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.OrderNo == tripRequest.GlobalSearch)
                                           || (String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.OrderNo == oh.OrderNo))
                                           ||
                                           ((!String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.VehicleNo == tripRequest.GlobalSearch)
                                           || (String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.VehicleNo == oh.VehicleNo))
                                    ||
                                    ((tripRequest.GlobalSearch != string.Empty && pksh.PackingSheetNo == tripRequest.GlobalSearch)
                                    || (tripRequest.GlobalSearch == string.Empty && pksh.PackingSheetNo == pksh.PackingSheetNo))) && (oh.DriverName != null) && (oh.VehicleNo != null))
                                     &&
                                    (partnerDataCk.Contains(opd.PartnerID) && opd.PartnerTypeId == 1)
                                    select new Domain.Trip
                                    {
                                        OrderId = oh.ID,
                                        OrderType = oh.OrderType,
                                        OrderNumber = oh.OrderNo,
                                        VehicleType = oh.VehicleShipment,
                                        Vehicle = oh.VehicleNo,
                                        EstimatedArrivalDate = od.ActualShipmentDate,
                                        EstimatedShipmentDate = od.EstimationShipmentDate,
                                        Dimensions = oh.OrderWeight + " " + oh.OrderWeightUM,
                                        OrderStatusId = oh.OrderStatusID,
                                        OrderStatus = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusValue,
                                        OrderStatusCode = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusCode
                                    }).Distinct().ToList();
                    }
                    else
                    {
                        tripList = (from oh in context.OrderHeaders
                                    join od in context.OrderDetails on oh.ID equals od.OrderHeaderID
                                    join ps in context.PackingSheets on od.ShippingListNo equals ps.ShippingListNo into pks
                                    from pksh in pks.DefaultIfEmpty()
                                    where businessAreas.Contains(oh.BusinessAreaId) && ((((!String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.OrderNo == tripRequest.GlobalSearch)
                                           || (String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.OrderNo == oh.OrderNo))
                                           ||
                                           ((!String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.VehicleNo == tripRequest.GlobalSearch)
                                           || (String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.VehicleNo == oh.VehicleNo))
                                    ||
                                    ((tripRequest.GlobalSearch != string.Empty && pksh.PackingSheetNo == tripRequest.GlobalSearch)
                                    || (tripRequest.GlobalSearch == string.Empty && pksh.PackingSheetNo == pksh.PackingSheetNo))) && (oh.DriverName != null) && (oh.VehicleNo != null))
                                    select new Domain.Trip
                                    {
                                        OrderId = oh.ID,
                                        OrderType = oh.OrderType,
                                        OrderNumber = oh.OrderNo,
                                        VehicleType = oh.VehicleShipment,
                                        Vehicle = oh.VehicleNo,
                                        EstimatedArrivalDate = od.ActualShipmentDate,
                                        EstimatedShipmentDate = od.EstimationShipmentDate,
                                        Dimensions = oh.OrderWeight + " " + oh.OrderWeightUM,
                                        OrderStatusId = oh.OrderStatusID,
                                        OrderStatus = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusValue,
                                        OrderStatusCode = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusCode
                                    }).Distinct().ToList();
                    }

                  

                    if (tripList != null && tripList.Count > 0)
                    {
                        List<int> distinctOrderIds = tripList.Select(t => t.OrderId).Distinct().ToList();
                      
                        foreach (int orderId in distinctOrderIds)
                        {
                            Domain.Trip order = new Domain.Trip();
                            order = tripList.Where(t => t.OrderId == orderId).FirstOrDefault();
                            //}
                            //tripList = (from element in tripList
                            //          group element by element.ID
                            //          into groups
                            //          select groups.OrderBy(p => p.EstimatedArrivalDate).First()).ToList();
                            //foreach (var order in tripList)
                            //{
                            int orderDetailId = context.OrderDetails.Where(x => x.OrderHeaderID == order.OrderId).Select(y => new { y.ID, y.SequenceNo }).OrderBy(od => od.SequenceNo).FirstOrDefault().ID;

                            var orderResponse = context.OrderStatusHistories.Where(x => x.OrderDetailID == orderDetailId).Select(osh => osh.OrderStatusID).ToList();
                            //         var omsresponse = JsonConvert.DeserializeObject<OrderStatusResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                            //+ "/v1/trip/getlasttripstatus", Method.POST, omsRequest, token));

                            // if (order.OrderStatusCode == "3" || order.OrderStatusCode == "13" || order.OrderStatusCode == "15" || order.OrderStatusCode == "16")
                            if (orderResponse.Contains(7))
                            {
                                order.IsChangeAllowed = false;
                            }
                            else
                            {
                                order.IsChangeAllowed = true;
                            }
                            //var orderData = context.OrderDetails.Where(x => x.OrderHeaderID == order.OrderId).Select(od => new { OrderDetailId = od.ID, SequenceNo = od.SequenceNo }).OrderByDescending(s=>s.SequenceNo).FirstOrDefault();
                            var orderData = (from od in context.OrderDetails
                                             where od.OrderHeaderID == order.OrderId
                                             group od by new { od.ID, od.SequenceNo } into gp
                                             select new
                                             {
                                                 OrderDetailId = gp.Key.ID,
                                                 SequenceNo = (from t2 in gp select t2.SequenceNo).Max(),
                                             }).ToList();

                            if (orderData != null)
                            {
                                var OrderDetailList = orderData.Select(x => x.OrderDetailId).ToList();
                                var partnerData = context.OrderPartnerDetails.
                                                          Where(op => OrderDetailList.Contains(op.OrderDetailID)).

                                                           Select(p => new
                                                           {
                                                               PrtnerID = p.PartnerID,
                                                               PartnerName = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerName,
                                                               partnerTypeID = p.PartnerTypeId
                                                            
                                                              
                                                           }).ToList();
                                //var partnerData = (from op in context.OrderPartnerDetails
                                //                       where (op => OrderDetailList.Contains(op.OrderDetailID) )
                                //                       select new
                                //                       {
                                //                           PrtnerID = op.PartnerID,
                                //                           PartnerName = context.Partners.Where(t => t.ID == op.PartnerID).FirstOrDefault().PartnerName,
                                //                           partnerTypeID = op.PartnerTypeId
                                //                       }).ToList();




                                if (partnerData != null && partnerData.Count > 0)
                                {
                                    var partners = (from pd in partnerData
                                                    join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                                                    select new
                                                    {
                                                        PrtnerID = pd.PrtnerID,
                                                        PartnerName = pd.PartnerName,
                                                        partnerTypeID = pd.partnerTypeID,
                                                        PartnerTypeCode = pt.PartnerTypeCode
                                                    }).Distinct().ToList();

                                    if (partners != null && partners.Count > 0)
                                    {
                                        foreach (var partner in partners)
                                        {
                                            if (partner.PartnerTypeCode == "2")
                                            {
                                                if (order.Source == null)
                                                {
                                                    order.Source = partner.PartnerName;
                                                    order.EstimatedArrivalDate = context.OrderDetails.Where(x => x.OrderHeaderID == order.OrderId && x.SequenceNo == 10).Select(od => od.ActualShipmentDate).First();

                                                }
                                            }
                                            if (partner.PartnerTypeCode == "3")
                                            {
                                                order.Destination = partner.PartnerName;
                                                order.EstimatedShipmentDate = context.OrderDetails.Where(x => x.OrderHeaderID == order.OrderId).Select(od => new { od.EstimationShipmentDate, od.SequenceNo }).OrderByDescending(x=>x.SequenceNo).FirstOrDefault().EstimationShipmentDate;
                                            }
                                        }
                                    }
                                }
                            }
                            tripListGrouped.Add(order);
                            order = null;
                        }
                    }

                    // Filter
                    if (tripList != null && tripList.Count > 0 && tripRequest.Requests.Count > 0)
                    {
                        tripList.Clear();
                        tripList = tripListGrouped;
                        var orderFilter = tripRequest.Requests[0];

                        if (!string.IsNullOrEmpty(orderFilter.OrderNumber))
                        {
                            tripList = tripList.Where(o => o.OrderNumber.Contains(orderFilter.OrderNumber)).ToList();
                        }

                        if (!String.IsNullOrEmpty(orderFilter.PackingSheetNumber))
                        {
                            tripList = tripList.Where(o => o.PackingSheetNumber.Contains(orderFilter.PackingSheetNumber)).ToList();
                        }

                        if (!String.IsNullOrEmpty(orderFilter.Vehicle))
                        {
                            tripList = tripList.Where(o => o.Vehicle.Contains(orderFilter.Vehicle)).ToList();
                        }

                        if (orderFilter.OrderStatusId > 0)
                        {
                            tripList = tripList.Where(o => o.OrderStatusId == orderFilter.OrderStatusId).ToList();
                        }
                    }

                    // Sorting
                    if (tripList != null && tripList.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(tripRequest.SortOrder))
                        {
                            switch (tripRequest.SortOrder.ToLower())
                            {
                                case "ordernumber":
                                    tripList = tripList.OrderBy(o => o.OrderNumber).ToList();
                                    break;
                                case "ordernumber_desc":
                                    tripList = tripList.OrderByDescending(o => o.OrderNumber).ToList();
                                    break;
                                case "source":
                                    tripList = tripList.OrderBy(o => o.Source).ToList();
                                    break;
                                case "source_desc":
                                    tripList = tripList.OrderByDescending(o => o.Source).ToList();
                                    break;
                                case "destination":
                                    tripList = tripList.OrderBy(o => o.Destination).ToList();
                                    break;
                                case "destination_desc":
                                    tripList = tripList.OrderByDescending(o => o.Destination).ToList();
                                    break;
                                case "estimatedarrivaldate":
                                    tripList = tripList.OrderBy(o => o.EstimatedArrivalDate).ToList();
                                    break;
                                case "estimatedarrivaldate_desc":
                                    tripList = tripList.OrderByDescending(o => o.EstimatedArrivalDate).ToList();
                                    break;
                                case "estimatedshipmentdate":
                                    tripList = tripList.OrderBy(o => o.EstimatedShipmentDate).ToList();
                                    break;
                                case "estimatedshipmentdate_desc":
                                    tripList = tripList.OrderByDescending(o => o.EstimatedShipmentDate).ToList();
                                    break;
                                case "policenumber":
                                    tripList = tripList.OrderBy(o => o.Vehicle).ToList();
                                    break;
                                case "policenumber_desc":
                                    tripList = tripList.OrderByDescending(o => o.Vehicle).ToList();
                                    break;
                                case "orderstatus":
                                    tripList = tripList.OrderBy(o => o.OrderStatus).ToList();
                                    break;
                                case "orderstatus_desc":
                                    tripList = tripList.OrderByDescending(o => o.OrderStatus).ToList();
                                    break;
                                case "dimensions":
                                    tripList = tripList.OrderBy(o => o.Dimensions).ToList();
                                    break;
                                case "dimensions_desc":
                                    tripList = tripList.OrderByDescending(o => o.Dimensions).ToList();
                                    break;
                                default:  // ID Descending 
                                    tripList = tripList.OrderByDescending(o => o.OrderId).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            tripList = tripList.OrderByDescending(o => o.OrderId).ToList();
                        }
                    }


                    // Total NumberOfRecords
                    if (tripList != null)
                        tripResponse.NumberOfRecords = tripList.Count;

                    // Paging
                    int pageNumber = (tripRequest.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(tripRequest.PageSize);
                    if (pageSize > 0)
                    {
                        tripList = tripList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }

                    if (tripList != null && tripList.Count > 0)
                    {
                        tripResponse.Data.AddRange(tripList);
                        tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        tripResponse.StatusCode = (int)HttpStatusCode.OK;
                        tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        tripResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                tripResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                tripResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                tripResponse.StatusMessage = ex.Message;
            }
            return tripResponse;
        }

        public OrderDetailsResponse GetTripDetails(int orderId)
        {
            OrderDetailsResponse orderDetailsResponse = new OrderDetailsResponse();
            try
            {
                using (var context = new DataModel.TMSDBContext())
                {
                    var orderData = (from oH in context.OrderHeaders
                                     where oH.ID == orderId
                                     select new OrderDetailsResponse
                                     {
                                         ID = oH.ID,
                                         BusinessArea = oH.BusinessArea.BusinessAreaDescription,
                                         BusinessAreaId = oH.BusinessAreaId,
                                         DriverName = oH.DriverName,
                                         DriverNo = oH.DriverNo,
                                         FleetType = oH.FleetType.ID,
                                         Harga = oH.Harga,
                                         IsActive = oH.IsActive,
                                         LegecyOrderNo = oH.LegecyOrderNo,
                                         VehicleNo = oH.VehicleNo,
                                         VehicleShipmentType = oH.VehicleShipment,
                                         SOPONumber = oH.SOPONumber,
                                         OrderNo = oH.OrderNo,
                                         OrderShipmentStatus = oH.OrderStatusID,
                                         OrderType = oH.OrderType,
                                         OrderWeight = oH.OrderWeight,
                                         OrderWeightUM = oH.OrderWeightUM,
                                         ShipmentScheduleImageGUID = context.ImageGuids.FirstOrDefault(t => t.ID == oH.ShipmentScheduleImageID).ImageGuIdValue,
                                         ShipmentScheduleImageID = oH.ShipmentScheduleImageID

                                     }).FirstOrDefault();


                    if (orderData != null)
                    {


                        var orderPartnerData = (from orderPartnerDetails in context.OrderPartnerDetails
                                                join orderDetailsData in context.OrderDetails on orderPartnerDetails.OrderDetailID equals orderDetailsData.ID
                                                where orderDetailsData.OrderHeaderID == orderId
                                                select new Domain.StopPoints
                                                {
                                                    ID = orderPartnerDetails.ID,
                                                    Address = orderPartnerDetails.Partner.PartnerAddress,
                                                    CityName = orderPartnerDetails.Partner.SubDistrict.City.CityDescription,
                                                    ProvinceName = orderPartnerDetails.Partner.SubDistrict.City.Province.ProvinceDescription,
                                                    SubDistrictName = orderPartnerDetails.Partner.SubDistrict.SubdistrictName,
                                                    ActualShipmentDate = orderDetailsData.ActualShipmentDate.ToString(),
                                                    EstimationShipmentDate = orderDetailsData.EstimationShipmentDate.ToString(),
                                                    PartnerCode = orderPartnerDetails.Partner.PartnerNo,
                                                    PartnerId = orderPartnerDetails.PartnerID,
                                                    PartnerName = orderPartnerDetails.Partner.PartnerName,
                                                    PeartnerType = orderPartnerDetails.PartnerTypeId,
                                                    SequenceNo = orderDetailsData.SequenceNo,
                                                    Instruction = orderDetailsData.Instruction,
                                                    TotalPallet = orderDetailsData.TotalPallet

                                                }
                                                ).ToList();
                        if (orderPartnerData.Count > 0)
                        {
                            if (orderData.OrderType == 2) //Out Bound
                            {

                                int maxSeqNo = orderPartnerData.Max(x => x.SequenceNo);
                                var transporter = (from data in orderPartnerData
                                                   where data.PeartnerType == 1 && data.SequenceNo == maxSeqNo
                                                   select data
                                                   ).FirstOrDefault();
                                var source = (from data in orderPartnerData
                                              where data.PeartnerType == 2 && data.SequenceNo == maxSeqNo
                                              select data
                                                  ).FirstOrDefault();
                                var destinations = (from data in orderPartnerData
                                                    where data.PeartnerType == 3
                                                    select data
                                                  ).ToList();

                                List<Domain.StopPoints> stopPoints = new List<Domain.StopPoints>();
                                stopPoints.Add(source);
                                if (destinations.Count > 0)
                                {
                                    foreach (var item in destinations)
                                    {
                                        stopPoints.Add(item);
                                    }
                                }
                                orderDetailsResponse = orderData;
                                orderDetailsResponse.Transporter = transporter;
                                orderDetailsResponse.Instructions = transporter.Instruction;
                                orderDetailsResponse.TotalPallet = transporter.TotalPallet;
                                orderDetailsResponse.SourceOrDestinations = stopPoints;
                            }
                            else //In Bound
                            {
                                int maxSeqNo = orderPartnerData.Max(x => x.SequenceNo);
                                var transporter = (from data in orderPartnerData
                                                   where data.PeartnerType == 1 && data.SequenceNo == maxSeqNo
                                                   select data
                                                   ).FirstOrDefault();
                                var source = (from data in orderPartnerData
                                              where data.PeartnerType == 2
                                              select data
                                                  ).ToList();
                                var destinations = (from data in orderPartnerData
                                                    where data.PeartnerType == 3 && data.SequenceNo == maxSeqNo
                                                    select data
                                                  ).FirstOrDefault();

                                List<Domain.StopPoints> stopPoints = new List<Domain.StopPoints>();
                                if (source.Count > 0)
                                {
                                    foreach (var item in source)
                                    {
                                        stopPoints.Add(item);
                                    }
                                }
                                stopPoints.Add(destinations);
                                orderDetailsResponse = orderData;
                                orderDetailsResponse.Transporter = transporter;
                                orderDetailsResponse.Instructions = transporter.Instruction;
                                orderDetailsResponse.TotalPallet = transporter.TotalPallet;
                                orderDetailsResponse.SourceOrDestinations = stopPoints;
                            }
                        }
                        orderDetailsResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        orderDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                        orderDetailsResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        orderDetailsResponse.NumberOfRecords = 0;
                        orderDetailsResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        orderDetailsResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        orderDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderDetailsResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderDetailsResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderDetailsResponse.StatusMessage = ex.Message;
            }
            return orderDetailsResponse;
        }

        public TripResponse UpdateTripDetails(TripRequest tripRequest)
        {
            TripResponse tripResponse = new TripResponse();
            List<Domain.Trip> trips = new List<Domain.Trip>();

            try
            {
                using (var context = new DataModel.TMSDBContext())
                {
                    foreach (var request in tripRequest.Requests)
                    {
                        Domain.Trip trip = new Domain.Trip();

                        var orderHeadeData = (from data in context.OrderHeaders
                                              where data.ID == request.OrderId
                                              select data).FirstOrDefault();
                        if (orderHeadeData != null)
                        {
                            orderHeadeData.DriverName = context.Drivers.Where(o => o.DriverNo == request.DriverName && o.IsActive).Select(d => d.UserName).FirstOrDefault();
                            orderHeadeData.DriverNo = context.Drivers.Where(o => o.DriverNo == request.DriverName && o.IsActive).Select(d => d.DriverNo).FirstOrDefault();
                            orderHeadeData.VehicleShipment = request.VehicleType;
                            orderHeadeData.VehicleNo = request.Vehicle;
                            orderHeadeData.LastModifiedTime = tripRequest.LastModifiedTime;
                            orderHeadeData.LastModifiedBy = tripRequest.LastModifiedBy;
                            orderHeadeData.OrderStatusID = context.OrderStatuses.Where(s => s.OrderStatusCode == "3").Select(t => t.ID).FirstOrDefault();
                            context.SaveChanges();

                            var OrderDetailsIds = context.OrderDetails.Where(o => o.OrderHeaderID == orderHeadeData.ID).ToList();
                            if (OrderDetailsIds.Count > 0)
                            {
                                foreach (var orderDetail in OrderDetailsIds)
                                {
                                    var statusHistory = context.OrderStatusHistories.Where(osh => osh.OrderDetailID == orderDetail.ID).ToList();
                                    context.OrderStatusHistories.RemoveRange(statusHistory);
                                    OrderStatusHistory tshObj = new OrderStatusHistory()
                                    {
                                        OrderDetailID = orderDetail.ID,
                                        StatusDate = DateTime.Now,
                                        Remarks = "Trip re-assigned",
                                        OrderStatusID = context.OrderStatuses.Where(t => t.OrderStatusCode == "3").Select(t => t.ID).FirstOrDefault()
                                    };
                                    context.OrderStatusHistories.Add(tshObj);
                                    context.SaveChanges();
                                }
                            }
                            tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            tripResponse.StatusCode = (int)HttpStatusCode.OK;
                            tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.TripReAssigned;
                            trip.OrderNumber = orderHeadeData.OrderNo;
                            trip.DriverNo = orderHeadeData.DriverNo;
                            trip.DriverName = orderHeadeData.DriverName;
                            trip.VehicleType = orderHeadeData.VehicleShipment;
                            trip.Vehicle = orderHeadeData.VehicleNo;
                            trips.Add(trip);
                            tripResponse.Data = trips;
                        }
                        else
                        {
                            tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            tripResponse.StatusCode = (int)HttpStatusCode.NotFound;
                            tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                tripResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                tripResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                tripResponse.StatusMessage = ex.Message;
            }
            return tripResponse;
        }
    }
}
