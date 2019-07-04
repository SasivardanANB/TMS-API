using NLog;
using Data = OMS.DataGateway.DataModels;
using OMS.DataGateway.Repositories.Iterfaces;
using OMS.DomainObjects.Objects;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain = OMS.DomainObjects.Objects;
using System.Net;
using System.Data.Entity;
using System.Globalization;

namespace OMS.DataGateway.Repositories
{
    public class Order : IOrder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderResponse GetOrders(DownloadOrderRequest orderRequest)
        {
            OrderResponse response = new OrderResponse()
            {
                Data = new List<Domain.Order>()
            };

            List<Domain.Order> orders = new List<Domain.Order>();
            try
            {
                using (var context = new Data.OMSDBContext())
                {
                    if (orderRequest.Requests.Count == 0)
                    {
                        orders = (from details in context.OrderDetails
                                  join headers in context.OrderHeaders on details.OrderHeaderID equals headers.ID
                                  where headers.IsActive
                                  select new Domain.Order()
                                  {
                                      ID = headers.ID,
                                      OrderDetailID = details.ID,
                                      BusinessArea = context.BusinessAreas.Where(t => t.ID == headers.BusinessAreaId).FirstOrDefault().BusinessAreaCode,
                                      OrderNo = headers.OrderNo,
                                      LegecyOrderNo = headers.LegecyOrderNo,
                                      SequenceNo = details.SequenceNo,
                                      PartnerNo1 = GetPartner(details.ID, "1").PartnerNo,
                                      PartnerType1 = 1,
                                      PartnerName1 = GetPartner(details.ID, "1").PartnerName,
                                      PartnerNo2 = GetPartner(details.ID, "2").PartnerNo,
                                      PartnerType2 = 2,
                                      PartnerName2 = GetPartner(details.ID, "2").PartnerName,
                                      PartnerNo3 = GetPartner(details.ID, "3").PartnerNo,
                                      PartnerType3 = 3,
                                      PartnerName3 = GetPartner(details.ID, "3").PartnerName,
                                      FleetType = headers.FleetType,
                                      OrderType = headers.OrderType,
                                      VehicleShipmentType = headers.VehicleShipment,
                                      DriverNo = headers.DriverNo,
                                      DriverName = headers.DriverName,
                                      VehicleNo = headers.VehicleNo,
                                      OrderWeight = headers.OrderWeight,
                                      OrderWeightUM = headers.OrderWeightUM,
                                      EstimationShipment = details.EstimationShipmentDate,
                                      ActualShipment = details.ActualShipmentDate,
                                      OrderDate = headers.OrderDate,
                                      IsActive = headers.IsActive,
                                      OrderShipmentStatus = headers.OrderStatusID,
                                      Sender = details.Sender,
                                      Receiver = details.Receiver,
                                      Dimension = details.Dimension,
                                      TotalPallet = details.TotalPallet,
                                      Instructions = details.Instruction,
                                      TotalCollie = details.TotalCollie,
                                      ShippingListNo = details.ShippingListNo,
                                      OrderCreatedTime=details.CreatedTime

                                  }).ToList();
                    }
                    else if (orderRequest.Requests.Count > 0)
                    {
                        var filter = orderRequest.Requests[0];

                        orders = context.OrderDetails
                            .Join(
                            context.OrderHeaders,
                            details => details.OrderHeaderID,
                            headers => headers.ID,
                            (details, headers) => new Domain.Order()
                            {
                                ID = headers.ID,
                                OrderDetailID = details.ID,
                                BusinessArea = context.BusinessAreas.Where(t => t.ID == headers.BusinessAreaId).FirstOrDefault().BusinessAreaCode,
                                OrderNo = headers.OrderNo,
                                LegecyOrderNo = headers.LegecyOrderNo,
                                SequenceNo = details.SequenceNo,
                                PartnerNo1 = context.Partners.FirstOrDefault(t => t.ID == (context.OrderPartnerDetails.FirstOrDefault(a => a.OrderDetailID == details.ID && a.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == "1").ID).PartnerID)).PartnerNo, //  GetPartner(details.ID, "1").PartnerNo,
                                PartnerType1 = 1,
                                PartnerName1 = context.Partners.FirstOrDefault(t => t.ID == (context.OrderPartnerDetails.FirstOrDefault(a => a.OrderDetailID == details.ID && a.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == "1").ID).PartnerID)).PartnerName,//GetPartner(details.ID, "1").PartnerName,
                                PartnerNo2 = context.Partners.FirstOrDefault(t => t.ID == (context.OrderPartnerDetails.FirstOrDefault(a => a.OrderDetailID == details.ID && a.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == "2").ID).PartnerID)).PartnerNo, //GetPartner(details.ID, "2").PartnerNo,
                                PartnerType2 = 2,
                                PartnerName2 = context.Partners.FirstOrDefault(t => t.ID == (context.OrderPartnerDetails.FirstOrDefault(a => a.OrderDetailID == details.ID && a.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == "2").ID).PartnerID)).PartnerName, //GetPartner(details.ID, "2").PartnerName,
                                PartnerNo3 = context.Partners.FirstOrDefault(t => t.ID == (context.OrderPartnerDetails.FirstOrDefault(a => a.OrderDetailID == details.ID && a.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == "3").ID).PartnerID)).PartnerNo,//GetPartner(details.ID, "3").PartnerNo,
                                PartnerType3 = 3,
                                PartnerName3 = context.Partners.FirstOrDefault(t => t.ID == (context.OrderPartnerDetails.FirstOrDefault(a => a.OrderDetailID == details.ID && a.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == "3").ID).PartnerID)).PartnerName, // GetPartner(details.ID, "3").PartnerName,
                                FleetType = headers.FleetType,
                                OrderType = headers.OrderType,
                                VehicleShipmentType = headers.VehicleShipment,
                                DriverNo = headers.DriverNo,
                                DriverName = headers.DriverName,
                                VehicleNo = headers.VehicleNo,
                                OrderWeight = headers.OrderWeight,
                                OrderWeightUM = headers.OrderWeightUM,
                                EstimationShipment = details.EstimationShipmentDate,
                                ActualShipment = details.ActualShipmentDate,
                                OrderDate = headers.OrderDate,
                                IsActive = headers.IsActive,
                                OrderShipmentStatus = headers.OrderStatusID,
                                Sender = details.Sender,
                                Receiver = details.Receiver,
                                Dimension = details.Dimension,
                                TotalPallet = details.TotalPallet,
                                Instructions = details.Instruction,
                                TotalCollie = details.TotalCollie,
                                ShippingListNo = details.ShippingListNo,
                                OrderCreatedTime = details.CreatedTime
                            }
                            )
                        .Where(p => p.IsActive)
                        .Where(p => /*filter.StatusId == 0 || p.OrderShipmentStatus == filter.StatusId*/filter.StatusIds.Count>0? filter.StatusIds.Contains(p.OrderShipmentStatus): p.OrderShipmentStatus>0)
                        .Where(p => String.IsNullOrEmpty(filter.OrderNumber) || p.OrderNo.Contains(filter.OrderNumber))
                        .Where(p => filter.FromDate == DateTime.MinValue || (DbFunctions.TruncateTime(p.OrderDate) >= filter.FromDate.Date))
                        .Where(p => filter.ToDate == DateTime.MinValue || (DbFunctions.TruncateTime(p.OrderDate) <= filter.ToDate.Date)).ToList();
                    }


                    foreach (var order in orders)
                    {
                        order.EstimationShipmentDate = order.EstimationShipment.ToString("dd MMM yyyy");
                        order.EstimationShipmentTime = order.EstimationShipment.ToString("H:mm");
                        order.ActualShipmentDate = order.ActualShipment.ToString("dd MMM yyyy");
                        order.ActualShipmentTime = order.ActualShipment.ToString("H:mm");
                        order.OrderCreatedDate = order.OrderCreatedTime.ToString("dd MMM yyyy");
                        order.OrderCreateTime=order.OrderCreatedTime.ToString("H:mm");

                        List<string> packingSheets = (from ps in context.PackingSheets
                                                      where ps.ShippingListNo == order.ShippingListNo
                                                      select ps.PackingSheetNo).ToList();
                        if (packingSheets.Count > 0)
                        {
                            order.PackingSheetNo = string.Join(",", packingSheets);
                        }

                        List<string> shipmentSAPs = (from ss in context.ShipmentSAPs
                                                     where ss.OrderDetailID == order.OrderDetailID
                                                     select ss.ShipmentSAPNo).ToList();
                        if (shipmentSAPs.Count > 0)
                        {
                            order.ShipmentSAPNo = string.Join(",", shipmentSAPs);
                        }
                    }

                    // Sorting
                    if (!String.IsNullOrEmpty(orderRequest.SortOrder))
                    {
                        switch (orderRequest.SortOrder.ToLower())
                        {
                            case "ordernumber":
                                orders = orders.OrderBy(s => s.OrderNo).ToList();
                                break;
                            case "ordernumber_desc":
                                orders = orders.OrderByDescending(s => s.OrderNo).ToList();
                                break;
                            case "fromdate":
                                orders = orders.OrderBy(s => s.OrderDate).ToList();
                                break;
                            case "fromdate_desc":
                                orders = orders.OrderByDescending(s => s.OrderDate).ToList();
                                break;
                            case "todate":
                                orders = orders.OrderBy(s => s.OrderDate).ToList();
                                break;
                            case "todate_desc":
                                orders = orders.OrderByDescending(s => s.OrderDate).ToList();
                                break;
                            case "statusid":
                                orders = orders.OrderBy(s => s.OrderShipmentStatus).ToList();
                                break;
                            case "statusid_desc":
                                orders = orders.OrderByDescending(s => s.OrderShipmentStatus).ToList();
                                break;
                            default:  // ID Descending 
                                orders = orders.OrderByDescending(s => s.ID).ToList();
                                break;
                        }
                    }

                    // Total NumberOfRecords
                    response.NumberOfRecords = orders.Count;

                    // Paging
                    int pageNumber = (orderRequest.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(orderRequest.PageSize);
                    if (pageSize > 0)
                    {
                        orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }
                    if (orders.Count > 0)
                    {
                        response.Data = orders;
                        response.Status = DomainObjects.Resource.ResourceData.Success;
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusMessage = DomainObjects.Resource.ResourceData.OrdersDownloaded;
                    }
                    else
                    {
                        response.Status = DomainObjects.Resource.ResourceData.Success;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                response.Status = DomainObjects.Resource.ResourceData.Failure;
                response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                response.StatusMessage = ex.Message;
            }
            return response;
        }

        private Data.Partner GetPartner(int orderDetailId, string partnerType)
        {
            Data.Partner partner = new Data.Partner();
            using (var context = new Data.OMSDBContext())
            {
                int partnerId = context.OrderPartnerDetails.FirstOrDefault(t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == context.PartnerTypes.FirstOrDefault(x => x.PartnerTypeCode == partnerType).ID).PartnerID;
                partner = context.Partners.FirstOrDefault(t => t.ID == partnerId);
            }
            return partner;
        }

        public OrderResponse CreateUpdateOrders(OrderRequest request)
        {
            OrderResponse response = new OrderResponse()
            {
                Data = new List<Domain.Order>()
            };

            using (var context = new Data.OMSDBContext())
            {
                foreach (var order in request.Requests)
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            DateTime estimationShipmentDate = DateTime.ParseExact(order.EstimationShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.EstimationShipmentTime);
                            DateTime actualShipmentDate = DateTime.ParseExact(order.ActualShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.ActualShipmentTime);

                            #region Step 1: Check if We have Business Area master data
                            int businessAreaId;
                            var businessArea = (from ba in context.BusinessAreas
                                                where ba.BusinessAreaCode == order.BusinessArea
                                                select new Domain.BusinessArea()
                                                {
                                                    ID = ba.ID
                                                }).FirstOrDefault();
                            if (businessArea != null)
                                businessAreaId = businessArea.ID;
                            else
                            {
                                //Return with Business Area not found
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = order.BusinessArea + " Business Area not found in OMS.";
                                return response;
                            }
                            #endregion

                            #region Step 1: Check if We have Order Status in Master data
                            int orderStatusId;
                            string orderStatusValue = "";
                            var orderStatus = (from os in context.OrderStatuses
                                               where os.OrderStatusCode == order.OrderShipmentStatus.ToString()
                                               select new Domain.BusinessArea()
                                               {
                                                   ID = os.ID
                                               }).FirstOrDefault();
                            if (orderStatus != null)
                                orderStatusId = orderStatus.ID;
                            else
                            {
                                //Return with Status Code not found 
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = order.OrderShipmentStatus.ToString() + " Status Code not found in OMS.";
                                return response;
                            }
                            #endregion

                            #region Step 2: Check if Order already existing then update/create accordingly
                            var orderData = context.OrderHeaders.FirstOrDefault(t => t.OrderNo == order.OrderNo);
                            if (orderData != null)
                            {
                                #region Update Order
                                orderData.BusinessAreaId = businessAreaId;
                                orderData.OrderType = order.OrderType;
                                orderData.FleetType = order.FleetType;
                                orderData.VehicleShipment = order.VehicleShipmentType;
                                orderData.DriverNo = order.DriverNo;
                                orderData.DriverName = order.DriverName;
                                orderData.VehicleNo = order.VehicleNo;
                                orderData.OrderWeight = order.OrderWeight;
                                orderData.OrderWeightUM = order.OrderWeightUM;
                                orderData.IsActive = true;
                                orderData.LastModifiedBy = request.LastModifiedBy;
                                orderData.LastModifiedTime = DateTime.Now;
                                orderData.OrderStatusID = orderStatusId;

                                context.Entry(orderData).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                order.ID = orderData.ID;
                                context.Entry(orderData).State = System.Data.Entity.EntityState.Detached;
                                int orderDetailId = 0;

                                #region Step 3 : Check if Order Detail Exists
                                var existingOrderDetail = context.OrderDetails.FirstOrDefault(t => t.OrderHeaderID == order.ID && t.SequenceNo == order.SequenceNo);

                                if (existingOrderDetail != null)
                                {
                                    #region Update Order Detail
                                    existingOrderDetail.SequenceNo = order.SequenceNo;
                                    existingOrderDetail.Sender = order.Sender;
                                    existingOrderDetail.Receiver = order.Receiver;
                                    existingOrderDetail.Dimension = order.Dimension;
                                    existingOrderDetail.TotalPallet = order.TotalPallet;
                                    existingOrderDetail.Instruction = order.Instructions;
                                    existingOrderDetail.ShippingListNo = order.ShippingListNo;
                                    existingOrderDetail.TotalCollie = order.TotalCollie;
                                    existingOrderDetail.LastModifiedBy = order.OrderLastModifiedBy;
                                    existingOrderDetail.LastModifiedTime = DateTime.Now;
                                    existingOrderDetail.EstimationShipmentDate = estimationShipmentDate;
                                    existingOrderDetail.ActualShipmentDate = actualShipmentDate;

                                    context.Entry(existingOrderDetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    orderDetailId = existingOrderDetail.ID;
                                    context.Entry(existingOrderDetail).State = System.Data.Entity.EntityState.Detached;
                                    #endregion
                                }
                                else
                                {
                                    #region Create Order Detail
                                    Data.OrderDetail orderDetail = new Data.OrderDetail()
                                    {
                                        OrderHeaderID = order.ID,
                                        SequenceNo = order.SequenceNo,
                                        Sender = order.Sender,
                                        Receiver = order.Receiver,
                                        Dimension = order.Dimension,
                                        TotalPallet = order.TotalPallet,
                                        Instruction = order.Instructions,
                                        ShippingListNo = order.ShippingListNo,
                                        TotalCollie = order.TotalCollie,
                                        EstimationShipmentDate = estimationShipmentDate,
                                        ActualShipmentDate = actualShipmentDate,
                                        CreatedBy = request.CreatedBy,
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderDetails.Add(orderDetail);
                                    context.SaveChanges();
                                    orderDetailId = orderDetail.ID;
                                    #endregion
                                }
                                #endregion

                                string partner1TypeId = order.PartnerType1.ToString();
                                string partner2TypeId = order.PartnerType2.ToString();
                                string partner3TypeId = order.PartnerType3.ToString();

                                #region Check if Partner Type Exists or not
                                var partnerType1 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner1TypeId);
                                var partnerType2 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner2TypeId);
                                var partnerType3 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner3TypeId);

                                if (partnerType1 == null || partnerType2 == null || partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                if (partnerType1 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                var partner1 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo1 && ppt.PartnerTypeId == partnerType1.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner2 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo2 && ppt.PartnerTypeId == partnerType2.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner3 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo3 && ppt.PartnerTypeId == partnerType3.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                if (partnerType1 == null || partnerType2 == null || partnerType3 == null)
                                {
                                    //Return with Partner not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }

                                if (partner1 == null)
                                {

                                    response.StatusMessage = order.PartnerNo1 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner1Id = partner1.ID;

                                }
                                if (partner2 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo2 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner2Id = partner2.ID;
                                }
                                if (partner3 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo3 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner3Id = partner3.ID;
                                }

                                #endregion

                                #region Check if Order Partner Expeditor Detail Exists or not
                                var existingOrderPartner1Detail = context.OrderPartnerDetails.FirstOrDefault
                                    (t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == partnerType1.ID && t.PartnerID == partner1Id);
                                if (existingOrderPartner1Detail == null)
                                {
                                    Data.OrderPartnerDetail orderPartner1Detail = new Data.OrderPartnerDetail()
                                    {
                                        OrderDetailID = orderDetailId,
                                        PartnerID = partner1Id,
                                        PartnerTypeId = partnerType1.ID,
                                        IsOriginal = true,
                                        IsParent = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderPartnerDetails.Add(orderPartner1Detail);
                                    context.SaveChanges();
                                }
                                #endregion

                                #region Check if Order Partner Source Detail Exists or not
                                var existingOrderPartner2Detail = context.OrderPartnerDetails.FirstOrDefault
                                    (t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == partnerType2.ID && t.PartnerID == partner2Id);
                                if (existingOrderPartner2Detail == null)
                                {
                                    Data.OrderPartnerDetail orderPartner2Detail = new Data.OrderPartnerDetail()
                                    {
                                        OrderDetailID = orderDetailId,
                                        PartnerID = partner2Id,
                                        PartnerTypeId = partnerType2.ID,
                                        IsOriginal = true,
                                        IsParent = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderPartnerDetails.Add(orderPartner2Detail);
                                    context.SaveChanges();
                                }
                                #endregion

                                #region Check if Order Partner Destination Detail Exists or not
                                var existingOrderPartner3Detail = context.OrderPartnerDetails.FirstOrDefault
                                    (t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == partnerType3.ID && t.PartnerID == partner3Id);
                                if (existingOrderPartner3Detail == null)
                                {
                                    Data.OrderPartnerDetail orderPartner3Detail = new Data.OrderPartnerDetail()
                                    {
                                        OrderDetailID = orderDetailId,
                                        PartnerID = partner3Id,
                                        PartnerTypeId = partnerType3.ID,
                                        IsOriginal = true,
                                        IsParent = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderPartnerDetails.Add(orderPartner3Detail);
                                    context.SaveChanges();
                                }
                                #endregion

                                #region Step 6: Insert Packing Sheet
                                if (!string.IsNullOrEmpty(order.PackingSheetNo))
                                {
                                    string[] packingSheets = order.PackingSheetNo.Split(',');
                                    foreach (string packinSheet in packingSheets)
                                    {
                                        var existingPackingSheet = context.PackingSheets.FirstOrDefault(t => t.ShippingListNo == order.ShippingListNo && t.PackingSheetNo == packinSheet);
                                        if (existingPackingSheet == null)
                                        {
                                            Data.PackingSheet packingSheetRequest = new Data.PackingSheet()
                                            {
                                                ShippingListNo = order.ShippingListNo,
                                                PackingSheetNo = packinSheet,
                                                CreatedBy = request.CreatedBy,
                                                CreatedTime = DateTime.Now,
                                                LastModifiedBy = "",
                                                LastModifiedTime = null
                                            };

                                            context.PackingSheets.Add(packingSheetRequest);
                                            context.SaveChanges();
                                        }

                                    }
                                }
                                #endregion

                                #region Step 7: Insert Shipment SAP
                                if (!string.IsNullOrEmpty(order.ShipmentSAPNo))
                                {
                                    string[] shipmentSAPs = order.ShipmentSAPNo.Split(',');
                                    foreach (string shipmentSAP in shipmentSAPs)
                                    {
                                        var existingShipmentSAP = context.ShipmentSAPs.FirstOrDefault(t => t.OrderDetailID == orderDetailId && t.ShipmentSAPNo == shipmentSAP);
                                        if (existingShipmentSAP == null)
                                        {
                                            Data.ShipmentSAP shipmentSAPRequest = new Data.ShipmentSAP()
                                            {
                                                OrderDetailID = orderDetailId,
                                                ShipmentSAPNo = shipmentSAP,
                                                CreatedBy = request.CreatedBy,
                                                CreatedTime = DateTime.Now,
                                                LastModifiedBy = "",
                                                LastModifiedTime = null
                                            };

                                            context.ShipmentSAPs.Add(shipmentSAPRequest);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                #endregion

                                #endregion
                            }
                            else
                            {
                                #region Create New Order Header
                                Data.OrderHeader orderHeader = new Data.OrderHeader()
                                {
                                    OrderNo = order.OrderNo,
                                    LegecyOrderNo = order.OrderNo,
                                    OrderType = order.OrderType,
                                    FleetType = order.FleetType,
                                    VehicleShipment = order.VehicleShipmentType,
                                    DriverNo = order.DriverNo,
                                    DriverName = order.DriverName,
                                    VehicleNo = order.VehicleNo,
                                    OrderWeight = order.OrderWeight,
                                    OrderWeightUM = order.OrderWeightUM,
                                    BusinessAreaId = businessAreaId,
                                    IsActive = true,
                                    OrderDate = DateTime.Now,
                                    OrderStatusID = orderStatusId,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderHeaders.Add(orderHeader);
                                context.SaveChanges();
                                order.ID = orderHeader.ID;

                                #region Step 3 : Create Order Detail
                                Data.OrderDetail orderDetail = new Data.OrderDetail()
                                {
                                    OrderHeaderID = order.ID,
                                    SequenceNo = order.SequenceNo,
                                    Sender = order.Sender,
                                    Receiver = order.Receiver,
                                    Dimension = order.Dimension,
                                    TotalPallet = order.TotalPallet,
                                    Instruction = order.Instructions,
                                    ShippingListNo = order.ShippingListNo,
                                    TotalCollie = order.TotalCollie,
                                    EstimationShipmentDate = estimationShipmentDate,
                                    ActualShipmentDate = actualShipmentDate,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderDetails.Add(orderDetail);
                                context.SaveChanges();
                                #endregion

                                string partner1TypeId = order.PartnerType1.ToString();
                                string partner2TypeId = order.PartnerType2.ToString();
                                string partner3TypeId = order.PartnerType3.ToString();

                                #region Check if Partner Type Exists or not
                                var partnerType1 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner1TypeId);
                                var partnerType2 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner2TypeId);
                                var partnerType3 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner3TypeId);

                                if (partnerType1 == null || partnerType2 == null || partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                if (partnerType1 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                var partner1 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo1 && ppt.PartnerTypeId == partnerType1.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner2 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo2 && ppt.PartnerTypeId == partnerType2.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner3 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo3 && ppt.PartnerTypeId == partnerType3.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                if (partner1 == null || partner2 == null || partner3 == null)
                                {
                                    //Return with Partner not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }

                                if (partner1 == null)
                                {

                                    response.StatusMessage = order.PartnerNo1 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner1Id = partner1.ID;

                                }
                                if (partner2 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo2 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner2Id = partner2.ID;
                                }
                                if (partner3 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo3 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner3Id = partner3.ID;
                                }

                                #endregion

                                #region Step 5: Insert Expedetor Partner Detail
                                Data.OrderPartnerDetail orderPartner1Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    PartnerID = partner1Id,
                                    PartnerTypeId = partnerType1.ID,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner1Detail);
                                context.SaveChanges();

                                #endregion

                                #region Insert Source Partner Detail
                                Data.OrderPartnerDetail orderPartner2Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    PartnerID = partner2Id,
                                    PartnerTypeId = partnerType2.ID,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner2Detail);
                                context.SaveChanges();
                                #endregion

                                #region Insert Destination Partner Detail
                                Data.OrderPartnerDetail orderPartner3Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    PartnerID = partner3Id,
                                    PartnerTypeId = partnerType3.ID,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner3Detail);
                                context.SaveChanges();
                                #endregion

                                #region Step 6: Insert Packing Sheet
                                if (!string.IsNullOrEmpty(order.PackingSheetNo))
                                {
                                    string[] packingSheets = order.PackingSheetNo.Split(',');
                                    foreach (string packinSheet in packingSheets)
                                    {
                                        Data.PackingSheet packingSheetRequest = new Data.PackingSheet()
                                        {
                                            ShippingListNo = order.ShippingListNo,
                                            PackingSheetNo = packinSheet,
                                            CreatedBy = request.CreatedBy,
                                            CreatedTime = DateTime.Now,
                                            LastModifiedBy = "",
                                            LastModifiedTime = null
                                        };

                                        context.PackingSheets.Add(packingSheetRequest);
                                        context.SaveChanges();
                                    }
                                }
                                #endregion

                                #region Step 7: Insert Shipment SAP
                                if (!string.IsNullOrEmpty(order.ShipmentSAPNo))
                                {
                                    string[] shipmentSAPs = order.ShipmentSAPNo.Split(',');
                                    foreach (string shipmentSAP in shipmentSAPs)
                                    {
                                        Data.ShipmentSAP shipmentSAPRequest = new Data.ShipmentSAP()
                                        {
                                            OrderDetailID = orderDetail.ID,
                                            ShipmentSAPNo = shipmentSAP,
                                            CreatedBy = request.CreatedBy,
                                            CreatedTime = DateTime.Now,
                                            LastModifiedBy = "",
                                            LastModifiedTime = null
                                        };

                                        context.ShipmentSAPs.Add(shipmentSAPRequest);
                                        context.SaveChanges();
                                    }
                                }
                                #endregion

                                #endregion
                            }
                            #endregion

                            transaction.Commit();
                            response.Status = DomainObjects.Resource.ResourceData.Success;
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusMessage = DomainObjects.Resource.ResourceData.OrderCreated;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.Log(LogLevel.Error, ex);
                            response.Status = DomainObjects.Resource.ResourceData.Failure;
                            response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                            response.StatusMessage = ex.Message;
                        }
                    }
                }
            }
            return response;
        }

        public OrderStatusCodesResponse GetAllOrderStatus()
        {
            OrderStatusCodesResponse response = new OrderStatusCodesResponse()
            {
                Data = new List<Domain.Common>()
            };

            using (var context = new Data.OMSDBContext())
            {
                try
                {
                    var orderStatus = context.OrderStatuses.ToList();
                    if (orderStatus != null)
                    {
                        foreach (var item in orderStatus)
                        {
                            Domain.Common common = new Common()
                            {
                                Id = Convert.ToInt32(item.OrderStatusCode),
                                Value = item.OrderStatusValue
                            };
                            response.Data.Add(common);
                        }
                    }
                    response.NumberOfRecords = response.Data.Count;
                    response.Data = response.Data.OrderBy(t => t.Id).ToList();
                    response.Status = DomainObjects.Resource.ResourceData.Success;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.StatusMessage = DomainObjects.Resource.ResourceData.OrderStatusRetrieved;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                    response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    response.StatusMessage = ex.Message;
                }

            }
            return response;
        }

        public OrderResponse SyncOrders(OrderRequest request)
        {
            OrderResponse response = new OrderResponse()
            {
                Data = new List<Domain.Order>()
            };

            using (var context = new Data.OMSDBContext())
            {
                foreach (var order in request.Requests)
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            DateTime estimationShipmentDate = DateTime.ParseExact(order.EstimationShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.EstimationShipmentTime);
                            DateTime actualShipmentDate = DateTime.ParseExact(order.ActualShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.ActualShipmentTime);

                            #region Step 1: Check if We have Business Area master data
                            Domain.BusinessArea businessArea = new BusinessArea();
                            int businessAreaId;
                            string businessAreaCode = string.Empty;
                            if (request.UploadType == 2) // Upload via UI
                            {
                                businessArea = (from ba in context.BusinessAreas
                                                where ba.BusinessAreaCode == order.BusinessArea
                                                select new Domain.BusinessArea()
                                                {
                                                    ID = ba.ID,
                                                    BusinessAreaCode = ba.BusinessAreaCode
                                                }).FirstOrDefault();
                            }
                            else
                            {

                                businessArea = (from ba in context.BusinessAreas
                                                where ba.BusinessAreaCode == order.BusinessArea
                                                select new Domain.BusinessArea()
                                                {
                                                    ID = ba.ID
                                                }).FirstOrDefault();
                            }

                            if (businessArea.ID > 0)
                            {
                                businessAreaId = businessArea.ID;
                                businessAreaCode = businessArea.BusinessAreaCode;
                            }
                            else
                            {
                                //Return with Business Area not found
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = order.BusinessArea + " Business Area not found in OMS.";
                                return response;
                            }
                            #endregion

                            #region Step 1: Check if We have Order Status in Master data
                            int orderStatusId;
                            string orderStatusValue = string.Empty;
                            var orderStatus = (from os in context.OrderStatuses
                                               where os.OrderStatusCode == order.OrderShipmentStatus.ToString()
                                               select new Domain.BusinessArea()
                                               {
                                                   ID = os.ID
                                               }).FirstOrDefault();
                            if (orderStatus != null)
                                orderStatusId = orderStatus.ID;
                            else
                            {
                                //Return with Status Code not found 
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = order.OrderShipmentStatus.ToString() + " Status Code not found in OMS.";
                                return response;
                            }
                            #endregion

                            #region Step 2: Check if Order already existing then update/create accordingly
                            var orderData = context.OrderHeaders.FirstOrDefault(t => t.OrderNo == order.OrderNo);

                            if (request.UploadType == 2)
                            {
                                if (orderData == null)
                                {
                                    string orderNumber = GetOrderNumber(businessAreaId, businessAreaCode, "OMS", DateTime.Now.Year);

                                    foreach (var ord in request.Requests)
                                    {
                                        ord.OrderNo = orderNumber;
                                    }
                                }
                            }

                            if (orderData != null)
                            {
                                #region Update Order
                                orderData.BusinessAreaId = businessAreaId;
                                orderData.OrderType = order.OrderType;
                                orderData.FleetType = order.FleetType;
                                orderData.VehicleShipment = order.VehicleShipmentType;
                                orderData.DriverNo = order.DriverNo;
                                orderData.DriverName = order.DriverName;
                                orderData.VehicleNo = order.VehicleNo;
                                orderData.OrderWeight = order.OrderWeight;
                                orderData.OrderWeightUM = order.OrderWeightUM;
                                orderData.IsActive = true;
                                orderData.LastModifiedBy = request.LastModifiedBy;
                                orderData.LastModifiedTime = DateTime.Now;
                                orderData.OrderStatusID = orderStatusId;

                                context.Entry(orderData).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                order.ID = orderData.ID;
                                context.Entry(orderData).State = System.Data.Entity.EntityState.Detached;
                                int orderDetailId = 0;

                                #region Step 3 : Check if Order Detail Exists
                                var existingOrderDetail = context.OrderDetails.FirstOrDefault(t => t.OrderHeaderID == order.ID && t.SequenceNo == order.SequenceNo);

                                if (existingOrderDetail != null)
                                {
                                    #region Update Order Detail
                                    existingOrderDetail.SequenceNo = order.SequenceNo;
                                    existingOrderDetail.Sender = order.Sender;
                                    existingOrderDetail.Receiver = order.Receiver;
                                    existingOrderDetail.Dimension = order.Dimension;
                                    existingOrderDetail.TotalPallet = order.TotalPallet;
                                    existingOrderDetail.Instruction = order.Instructions;
                                    existingOrderDetail.ShippingListNo = order.ShippingListNo;
                                    existingOrderDetail.TotalCollie = order.TotalCollie;
                                    existingOrderDetail.LastModifiedBy = order.OrderLastModifiedBy;
                                    existingOrderDetail.LastModifiedTime = DateTime.Now;
                                    existingOrderDetail.EstimationShipmentDate = estimationShipmentDate;
                                    existingOrderDetail.ActualShipmentDate = actualShipmentDate;

                                    context.Entry(existingOrderDetail).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    orderDetailId = existingOrderDetail.ID;
                                    context.Entry(existingOrderDetail).State = System.Data.Entity.EntityState.Detached;
                                    #endregion
                                }
                                else
                                {
                                    #region Create Order Detail
                                    Data.OrderDetail orderDetail = new Data.OrderDetail()
                                    {
                                        OrderHeaderID = order.ID,
                                        SequenceNo = order.SequenceNo,
                                        Sender = order.Sender,
                                        Receiver = order.Receiver,
                                        Dimension = order.Dimension,
                                        TotalPallet = order.TotalPallet,
                                        Instruction = order.Instructions,
                                        ShippingListNo = order.ShippingListNo,
                                        TotalCollie = order.TotalCollie,
                                        EstimationShipmentDate = estimationShipmentDate,
                                        ActualShipmentDate = actualShipmentDate,
                                        CreatedBy = request.CreatedBy,
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderDetails.Add(orderDetail);
                                    context.SaveChanges();
                                    orderDetailId = orderDetail.ID;
                                    #endregion
                                }
                                #endregion

                                string partner1TypeId = order.PartnerType1.ToString();
                                string partner2TypeId = order.PartnerType2.ToString();
                                string partner3TypeId = order.PartnerType3.ToString();

                                #region Check if Partner Type Exists or not
                                var partnerType1 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner1TypeId);
                                var partnerType2 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner2TypeId);
                                var partnerType3 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner3TypeId);

                                if (partnerType1 == null || partnerType2 == null || partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                if (partnerType1 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                var partner1 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo1 && ppt.PartnerTypeId == partnerType1.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner2 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo2 && ppt.PartnerTypeId == partnerType2.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner3 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo3 && ppt.PartnerTypeId == partnerType3.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                if (partnerType1 == null || partnerType2 == null || partnerType3 == null)
                                {
                                    //Return with Partner not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }

                                if (partner1 == null)
                                {

                                    response.StatusMessage = order.PartnerNo1 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner1Id = partner1.ID;

                                }
                                if (partner2 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo2 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner2Id = partner2.ID;
                                }
                                if (partner3 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo3 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner3Id = partner3.ID;
                                }

                                #endregion

                                #region Check if Order Partner Expeditor Detail Exists or not
                                var existingOrderPartner1Detail = context.OrderPartnerDetails.FirstOrDefault
                                    (t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == partnerType1.ID && t.PartnerID == partner1Id);
                                if (existingOrderPartner1Detail == null)
                                {
                                    Data.OrderPartnerDetail orderPartner1Detail = new Data.OrderPartnerDetail()
                                    {
                                        OrderDetailID = orderDetailId,
                                        PartnerID = partner1Id,
                                        PartnerTypeId = partnerType1.ID,
                                        IsOriginal = true,
                                        IsParent = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderPartnerDetails.Add(orderPartner1Detail);
                                    context.SaveChanges();
                                }
                                #endregion

                                #region Check if Order Partner Source Detail Exists or not
                                var existingOrderPartner2Detail = context.OrderPartnerDetails.FirstOrDefault
                                    (t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == partnerType2.ID && t.PartnerID == partner2Id);
                                if (existingOrderPartner2Detail == null)
                                {
                                    Data.OrderPartnerDetail orderPartner2Detail = new Data.OrderPartnerDetail()
                                    {
                                        OrderDetailID = orderDetailId,
                                        PartnerID = partner2Id,
                                        PartnerTypeId = partnerType2.ID,
                                        IsOriginal = true,
                                        IsParent = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderPartnerDetails.Add(orderPartner2Detail);
                                    context.SaveChanges();
                                }
                                #endregion

                                #region Check if Order Partner Destination Detail Exists or not
                                var existingOrderPartner3Detail = context.OrderPartnerDetails.FirstOrDefault
                                    (t => t.OrderDetailID == orderDetailId && t.PartnerTypeId == partnerType3.ID && t.PartnerID == partner3Id);
                                if (existingOrderPartner3Detail == null)
                                {
                                    Data.OrderPartnerDetail orderPartner3Detail = new Data.OrderPartnerDetail()
                                    {
                                        OrderDetailID = orderDetailId,
                                        PartnerID = partner3Id,
                                        PartnerTypeId = partnerType3.ID,
                                        IsOriginal = true,
                                        IsParent = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.OrderPartnerDetails.Add(orderPartner3Detail);
                                    context.SaveChanges();
                                }
                                #endregion

                                #region Step 6: Insert Packing Sheet
                                if (!string.IsNullOrEmpty(order.PackingSheetNo))
                                {
                                    string[] packingSheets = order.PackingSheetNo.Split(',');
                                    foreach (string packinSheet in packingSheets)
                                    {
                                        var existingPackingSheet = context.PackingSheets.FirstOrDefault(t => t.ShippingListNo == order.ShippingListNo && t.PackingSheetNo == packinSheet);
                                        if (existingPackingSheet == null)
                                        {
                                            Data.PackingSheet packingSheetRequest = new Data.PackingSheet()
                                            {
                                                ShippingListNo = order.ShippingListNo,
                                                PackingSheetNo = packinSheet,
                                                CreatedBy = request.CreatedBy,
                                                CreatedTime = DateTime.Now,
                                                LastModifiedBy = "",
                                                LastModifiedTime = null
                                            };

                                            context.PackingSheets.Add(packingSheetRequest);
                                            context.SaveChanges();
                                        }

                                    }
                                }
                                #endregion

                                #region Step 7: Insert Shipment SAP
                                if (!string.IsNullOrEmpty(order.ShipmentSAPNo))
                                {
                                    string[] shipmentSAPs = order.ShipmentSAPNo.Split(',');
                                    foreach (string shipmentSAP in shipmentSAPs)
                                    {
                                        var existingShipmentSAP = context.ShipmentSAPs.FirstOrDefault(t => t.OrderDetailID == orderDetailId && t.ShipmentSAPNo == shipmentSAP);
                                        if (existingShipmentSAP == null)
                                        {
                                            Data.ShipmentSAP shipmentSAPRequest = new Data.ShipmentSAP()
                                            {
                                                OrderDetailID = orderDetailId,
                                                ShipmentSAPNo = shipmentSAP,
                                                CreatedBy = request.CreatedBy,
                                                CreatedTime = DateTime.Now,
                                                LastModifiedBy = "",
                                                LastModifiedTime = null
                                            };

                                            context.ShipmentSAPs.Add(shipmentSAPRequest);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                #endregion

                                #endregion
                            }
                            else
                            {
                                #region Create New Order Header
                                Data.OrderHeader orderHeader = new Data.OrderHeader()
                                {
                                    OrderNo = order.OrderNo,
                                    LegecyOrderNo = order.OrderNo,
                                    OrderType = order.OrderType,
                                    FleetType = order.FleetType,
                                    VehicleShipment = order.VehicleShipmentType,
                                    DriverNo = order.DriverNo,
                                    DriverName = order.DriverName,
                                    VehicleNo = order.VehicleNo,
                                    OrderWeight = order.OrderWeight,
                                    OrderWeightUM = order.OrderWeightUM,
                                    BusinessAreaId = businessAreaId,
                                    IsActive = true,
                                    OrderDate = DateTime.Now,
                                    OrderStatusID = orderStatusId,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null,
                                    UploadType = request.UploadType
                                };
                                context.OrderHeaders.Add(orderHeader);
                                context.SaveChanges();
                                order.ID = orderHeader.ID;

                                #region Step 3 : Create Order Detail
                                Data.OrderDetail orderDetail = new Data.OrderDetail()
                                {
                                    OrderHeaderID = order.ID,
                                    SequenceNo = order.SequenceNo,
                                    Sender = order.Sender,
                                    Receiver = order.Receiver,
                                    Dimension = order.Dimension,
                                    TotalPallet = order.TotalPallet,
                                    Instruction = order.Instructions,
                                    ShippingListNo = order.ShippingListNo,
                                    TotalCollie = order.TotalCollie,
                                    EstimationShipmentDate = estimationShipmentDate,
                                    ActualShipmentDate = actualShipmentDate,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderDetails.Add(orderDetail);
                                context.SaveChanges();
                                #endregion

                                string partner1TypeId = order.PartnerType1.ToString();
                                string partner2TypeId = order.PartnerType2.ToString();
                                string partner3TypeId = order.PartnerType3.ToString();

                                #region Check if Partner Type Exists or not
                                var partnerType1 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner1TypeId);
                                var partnerType2 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner2TypeId);
                                var partnerType3 = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partner3TypeId);

                                if (partnerType1 == null || partnerType2 == null || partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                if (partnerType1 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in OMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                var partner1 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo1 && ppt.PartnerTypeId == partnerType1.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner2 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo2 && ppt.PartnerTypeId == partnerType2.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                var partner3 = (from p in context.Partners
                                                join ppt in context.PartnerPartnerTypes on p.ID equals ppt.PartnerId
                                                where p.PartnerNo == order.PartnerNo3 && ppt.PartnerTypeId == partnerType3.ID
                                                select new Domain.Partner()
                                                {
                                                    ID = p.ID
                                                }).FirstOrDefault();

                                if (partner1 == null || partner2 == null || partner3 == null)
                                {
                                    //Return with Partner not found.
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }

                                if (partner1 == null)
                                {

                                    response.StatusMessage = order.PartnerNo1 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner1Id = partner1.ID;

                                }
                                if (partner2 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo2 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner2Id = partner2.ID;
                                }
                                if (partner3 == null)
                                {
                                    //Return with Partner not found.
                                    response.StatusMessage = order.PartnerNo3 + " Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner3Id = partner3.ID;
                                }

                                #endregion

                                #region Step 5: Insert Expedetor Partner Detail
                                Data.OrderPartnerDetail orderPartner1Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    PartnerID = partner1Id,
                                    PartnerTypeId = partnerType1.ID,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner1Detail);
                                context.SaveChanges();

                                #endregion

                                #region Insert Source Partner Detail
                                Data.OrderPartnerDetail orderPartner2Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    PartnerID = partner2Id,
                                    PartnerTypeId = partnerType2.ID,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner2Detail);
                                context.SaveChanges();
                                #endregion

                                #region Insert Destination Partner Detail
                                Data.OrderPartnerDetail orderPartner3Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    PartnerID = partner3Id,
                                    PartnerTypeId = partnerType3.ID,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner3Detail);
                                context.SaveChanges();
                                #endregion

                                #region Step 6: Insert Packing Sheet
                                if (!string.IsNullOrEmpty(order.PackingSheetNo))
                                {
                                    string[] packingSheets = order.PackingSheetNo.Split(',');
                                    foreach (string packinSheet in packingSheets)
                                    {
                                        Data.PackingSheet packingSheetRequest = new Data.PackingSheet()
                                        {
                                            ShippingListNo = order.ShippingListNo,
                                            PackingSheetNo = packinSheet,
                                            CreatedBy = request.CreatedBy,
                                            CreatedTime = DateTime.Now,
                                            LastModifiedBy = "",
                                            LastModifiedTime = null
                                        };

                                        context.PackingSheets.Add(packingSheetRequest);
                                        context.SaveChanges();
                                    }
                                }
                                #endregion

                                #region Step 7: Insert Shipment SAP
                                if (!string.IsNullOrEmpty(order.ShipmentSAPNo))
                                {
                                    string[] shipmentSAPs = order.ShipmentSAPNo.Split(',');
                                    foreach (string shipmentSAP in shipmentSAPs)
                                    {
                                        Data.ShipmentSAP shipmentSAPRequest = new Data.ShipmentSAP()
                                        {
                                            OrderDetailID = orderDetail.ID,
                                            ShipmentSAPNo = shipmentSAP,
                                            CreatedBy = request.CreatedBy,
                                            CreatedTime = DateTime.Now,
                                            LastModifiedBy = "",
                                            LastModifiedTime = null
                                        };

                                        context.ShipmentSAPs.Add(shipmentSAPRequest);
                                        context.SaveChanges();
                                    }
                                }
                                #endregion

                                #endregion
                            }
                            #endregion

                            transaction.Commit();
                            response.Data.Add(order);
                            response.Status = DomainObjects.Resource.ResourceData.Success;
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusMessage = DomainObjects.Resource.ResourceData.OrderCreated;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.Log(LogLevel.Error, ex);
                            response.Status = DomainObjects.Resource.ResourceData.Failure;
                            response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                            response.StatusMessage = ex.Message;
                        }
                    }
                }
            }
            return response;
        }

        private string GetOrderNumber(int businessAreaId, string businessArea, string applicationCode, int year)
        {
            string orderNo = businessArea + applicationCode;
            using (var context = new Data.OMSDBContext())
            {
                var order = context.OrderHeaders.Where(t => t.BusinessAreaId == businessAreaId && t.UploadType == 2).OrderByDescending(t => t.OrderNo).FirstOrDefault();
                if (order != null)
                {
                    int lastOrderYear = order.OrderDate.Year;
                    if (year != lastOrderYear)
                    {
                        orderNo += "00000001";
                    }
                    else
                    {
                        string orderSequnceString = order.OrderNo.Substring(order.OrderNo.Length - 8);
                        int orderSequnceNumber = Convert.ToInt32(orderSequnceString) + 1;

                        orderNo += orderSequnceNumber.ToString().PadLeft(8, '0');
                    }
                }
                else
                {
                    orderNo += "00000001";
                }
            }
            return orderNo;
        }

        public PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            PackingSheetResponse packingSheetResponse = new PackingSheetResponse();
            try
            {
                using (var context = new Data.OMSDBContext())
                {
                    foreach (var packingSheet in packingSheetRequest.Requests)
                    {
                        var orderID = context.OrderHeaders.Where(oh => oh.OrderNo == packingSheet.OrderNumber).Select(oh => oh.ID).FirstOrDefault();
                        var partnerID = context.Partners.Where(p => p.PartnerNo == packingSheet.DealerNumber).Select(p => p.ID).FirstOrDefault();

                        var orderDetailID = (from ord in context.OrderDetails
                                             join opd in context.OrderPartnerDetails on ord.ID equals opd.OrderDetailID
                                             where ord.OrderHeaderID == orderID && opd.PartnerID == partnerID
                                             select ord.ID).FirstOrDefault();

                        var orderDetailsData = context.OrderDetails.Where(x => x.ID == orderDetailID).FirstOrDefault();
                        if (orderDetailsData != null)
                        {
                            orderDetailsData.ShippingListNo = packingSheet.ShippingListNo;
                            orderDetailsData.TotalCollie = packingSheet.Collie;
                            orderDetailsData.Katerangan = packingSheet.Katerangan;

                            if (packingSheet.PackingSheetNumbers.Count > 0)
                            {
                                foreach (var item in packingSheet.PackingSheetNumbers)
                                {
                                    Data.PackingSheet packingSheetData = new Data.PackingSheet()
                                    {
                                        PackingSheetNo = item.Value,
                                        CreatedBy = packingSheetRequest.CreatedBy,
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null,
                                        ShippingListNo = packingSheet.ShippingListNo

                                    };

                                    context.PackingSheets.Add(packingSheetData);
                                    context.SaveChanges();
                                }
                            }
                            packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            packingSheetResponse.StatusMessage = DomainObjects.Resource.ResourceData.PackingSheetUpdated;
                            packingSheetResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            packingSheetResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                            packingSheetResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                packingSheetResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                packingSheetResponse.StatusMessage = ex.Message;
            }
            return packingSheetResponse;
        }

        public OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request)
        {
            _logger.Log(LogLevel.Error, request);
            OrderStatusResponse response = new OrderStatusResponse()
            {
                Data = new List<OrderStatus>()
            };

            using (var context = new Data.OMSDBContext())
            {
                try
                {
                    foreach (var statusRequest in request.Requests)
                    {
                        int orderId = 0;

                        orderId = context.OrderHeaders.FirstOrDefault(t => t.LegecyOrderNo == statusRequest.OrderNumber).ID;

                        #region Update Order Header
                        var orderHeader = context.OrderHeaders.FirstOrDefault(t => t.ID == orderId);
                        orderHeader.OrderStatusID = context.OrderStatuses.FirstOrDefault(t => t.OrderStatusCode == statusRequest.OrderStatusCode).ID;

                        context.Entry(orderHeader).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        context.Entry(orderHeader).State = System.Data.Entity.EntityState.Detached;

                        #endregion

                        response.Data = request.Requests;
                        response.Status = DomainObjects.Resource.ResourceData.Success;
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                    response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    response.StatusMessage = ex.Message;
                }
            }

            return response;
        }

        public TripResponse ReAssignTrip(TripRequest request)
        {
            TripResponse response = new TripResponse();
            List<Trip> tripDetails = new List<Trip>();

            using (var context = new Data.OMSDBContext())
            {
                foreach (var trip in request.Requests)
                {
                    Trip tripDetail = new Trip();

                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var tripObj = context.OrderHeaders.Where(t => t.OrderNo == trip.OrderNumber).FirstOrDefault();
                            if (tripObj != null)
                            {
                                tripObj.DriverNo =  trip.DriverNo ;
                                tripObj.DriverName = trip.DriverName;
                                tripObj.VehicleShipment = trip.VehicleType;
                                tripObj.VehicleNo = trip.Vehicle;
                                tripObj.LastModifiedBy = request.LastModifiedBy;
                                tripObj.LastModifiedTime = request.LastModifiedTime;
                                context.SaveChanges();
                            }
                            else
                            {
                                response.Status = DomainObjects.Resource.ResourceData.Success;
                                response.StatusCode = (int)HttpStatusCode.NotFound;
                                response.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                                return response;
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.Log(LogLevel.Error, ex);
                            response.Status = DomainObjects.Resource.ResourceData.Failure;
                            response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                            response.StatusMessage = ex.Message;
                            return response;
                        }
                    }
                    tripDetails.Add(tripDetail);
                }
                #region Return Response with Success and Commit Changes
                response.Data = tripDetails;
                response.Status = DomainObjects.Resource.ResourceData.Success;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusMessage = DomainObjects.Resource.ResourceData.TripReAssigned;
                #endregion

            }
            return response;
        }
    }
}
