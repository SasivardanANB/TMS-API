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
using TMS.DataGateway.Repositories.Iterfaces;


namespace TMS.DataGateway.Repositories
{
    public class Trip : ITrip
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TripResponse GetTripList(TripRequest tripRequest)
        {
            //TripResponse tripResponse = new TripResponse();
            TripResponse tripResponse = new TripResponse()
            {
                Data = new List<Domain.Trip>()
            };

            List<Domain.Trip> tripList;

            try
            {
                using (var context = new DataModel.TMSDBContext())
                {
                    tripList = (from oh in context.OrderHeaders
                                 join od in context.OrderDetails on oh.ID equals od.OrderHeaderID
                                 join ps in context.PackingSheets on od.ShippingListNo equals ps.ShippingListNo into pks
                                 from pksh in pks.DefaultIfEmpty()
                                where( (((!String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.OrderNo == tripRequest.GlobalSearch)
                                       || (String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.OrderNo == oh.OrderNo))
                                       ||
                                       ((!String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.VehicleNo == tripRequest.GlobalSearch)
                                       || (String.IsNullOrEmpty(tripRequest.GlobalSearch) && oh.VehicleNo == oh.VehicleNo))
                                ||
                                ((tripRequest.GlobalSearch != string.Empty && pksh.PackingSheetNo == tripRequest.GlobalSearch)
                                || (tripRequest.GlobalSearch == string.Empty && pksh.PackingSheetNo == pksh.PackingSheetNo))) && ((oh.OrderStatusID == 1)) ) 

                                select new Domain.Trip
                                 {
                                     OrderId = oh.ID,
                                     OrderType = oh.OrderType,
                                     OrderNumber = oh.OrderNo,
                                     VehicleType = oh.VehicleShipment,
                                     Vehicle = oh.VehicleNo,
                                     EstimatedArrivalDate=oh.ActualShipmentDate,
                                     EstimatedShipmentDate=oh.EstimationShipmentDate,
                                     Dimensions=oh.OrderWeight+" "+oh.OrderWeightUM, 
                                     OrderStatus = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusValue
                                 }).Distinct().ToList();

                    if (tripList != null && tripList.Count > 0)
                    {
                        foreach (var order in tripList)
                        {
                            var orderData = (from od in context.OrderDetails
                                             where od.OrderHeaderID == order.OrderId
                                             group od by new { od.ID, od.SequenceNo } into gp
                                             select new
                                             {
                                                 OrderDetailId = gp.Key.ID,
                                                 SequenceNo = gp.Max(t => t.SequenceNo),
                                             }).FirstOrDefault();

                            if (orderData != null)
                            {
                                var partnerData = (from op in context.OrderPartnerDetails
                                                   where op.OrderDetailID == orderData.OrderDetailId
                                                   select new
                                                   {
                                                       PrtnerID = op.PartnerID,
                                                       PartnerName = context.Partners.Where(t => t.ID == op.PartnerID).FirstOrDefault().PartnerName,
                                                       partnerTypeID = context.Partners.Where(t => t.ID == op.PartnerID).FirstOrDefault().PartnerTypeID
                                                   }).ToList();

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
                                                    }).ToList();

                                    if (partners != null && partners.Count > 0)
                                    {
                                        foreach (var partner in partners)
                                        {
                                            //if (partner.PartnerTypeCode == "1")
                                            //    order.Transporter = partner.PartnerName;
                                            if (partner.PartnerTypeCode == "2")
                                                order.Source = partner.PartnerName;
                                            if (partner.PartnerTypeCode == "3")
                                                order.Destination = partner.PartnerName;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Filter
                    if (tripList != null && tripList.Count > 0 && tripRequest.Requests.Count > 0)
                    {
                        var orderFilter = tripRequest.Requests[0];

                        if (!string.IsNullOrEmpty(orderFilter.OrderNumber))
                        {
                            tripList = tripList.Where(o => o.OrderNumber.Contains(orderFilter.OrderNumber)).ToList();
                        }

                        if (!String.IsNullOrEmpty(orderFilter.PackingSheetNumber))
                        {
                            tripList = tripList.Where(o => o.PackingSheetNumber.Contains(orderFilter.PackingSheetNumber)).ToList();
                        }

                        if (!String.IsNullOrEmpty(orderFilter.PoliceNumber))
                        {
                            tripList = tripList.Where(o => o.PoliceNumber.Contains(orderFilter.PoliceNumber)).ToList();
                        }
                        //if (orderFilter.OrderType != 0)
                        //{
                        //    tripList = tripList.Where(o => o.OrderType == orderFilter.OrderType).ToList();
                        //}
                    }

                    // Sorting
                    if (tripList.Count > 0 && !string.IsNullOrEmpty(tripRequest.SortOrder))
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
                            case "vehicletype":
                                tripList = tripList.OrderBy(o => o.VehicleType).ToList();
                                break;
                            case "vehicletype_desc":
                                tripList = tripList.OrderByDescending(o => o.VehicleType).ToList();
                                break;
                            //case "expiditionname":
                            //    tripList = tripList.OrderBy(o => o.ExpeditionName).ToList();
                            //    break;
                            //case "expiditionname_desc":
                            //    tripList = tripList.OrderByDescending(o => o.ExpeditionName).ToList();
                            //    break;
                            case "policenumber":
                                tripList = tripList.OrderBy(o => o.PoliceNumber).ToList();
                                break;
                            case "policenumber_desc":
                                tripList = tripList.OrderByDescending(o => o.PoliceNumber).ToList();
                                break;
                            case "orderstatus":
                                tripList = tripList.OrderBy(o => o.OrderStatus).ToList();
                                break;
                            case "orderstatus_desc":
                                tripList = tripList.OrderByDescending(o => o.OrderStatus).ToList();
                                break;
                            default:  // ID Descending 
                                tripList = tripList.OrderByDescending(o => o.OrderId).ToList();
                                break;
                        }
                    }

                    // Total NumberOfRecords
                    tripResponse.NumberOfRecords = tripList.Count;

                    // Paging
                    int pageNumber = (tripRequest.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(tripRequest.PageSize);
                    if (pageSize > 0)
                    {
                        tripList = tripList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }

                    if (tripList.Count > 0)                           
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
            //return orderSearchResponse;
            return tripResponse;
        }
    }
}
