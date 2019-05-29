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
                                      EstimationShipment = headers.EstimationShipmentDate,
                                      ActualShipment = headers.ActualShipmentDate,
                                      OrderDate = headers.OrderDate,
                                      IsActive = headers.IsActive,
                                      OrderShipmentStatus = headers.OrderStatusID,
                                      Sender = details.Sender,
                                      Receiver = details.Receiver,
                                      Dimension = details.Dimension,
                                      TotalPallet = details.TotalPallet,
                                      Instructions = details.Instruction,
                                      TotalCollie = details.TotalCollie,
                                      ShippingListNo = details.ShippingListNo

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
                                EstimationShipment = headers.EstimationShipmentDate,
                                ActualShipment = headers.ActualShipmentDate,
                                OrderDate = headers.OrderDate,
                                IsActive = headers.IsActive,
                                OrderShipmentStatus = headers.OrderStatusID,
                                Sender = details.Sender,
                                Receiver = details.Receiver,
                                Dimension = details.Dimension,
                                TotalPallet = details.TotalPallet,
                                Instructions = details.Instruction,
                                TotalCollie = details.TotalCollie,
                                ShippingListNo = details.ShippingListNo
                            }
                            )
                        .Where(p => p.IsActive)
                        .Where(p => filter.StatusId == 0 || p.OrderShipmentStatus == filter.StatusId)
                        .Where(p => String.IsNullOrEmpty(filter.OrderNumber) || p.OrderNo.Contains(filter.OrderNumber))
                        .Where(p => filter.FromDate == DateTime.MinValue || (DbFunctions.TruncateTime(p.OrderDate) >= filter.FromDate.Date))
                        .Where(p => filter.ToDate == DateTime.MinValue || (DbFunctions.TruncateTime(p.OrderDate) <= filter.ToDate.Date)).ToList();
                    }


                    foreach (var order in orders)
                    {
                        order.EstimationShipmentDate = order.EstimationShipment.ToString("dd.MM.yyyy");
                        order.EstimationShipmentTime = order.EstimationShipment.ToString("H:mm");
                        order.ActualShipmentDate = order.ActualShipment.ToString("dd.MM.yyyy");
                        order.ActualShipmentTime = order.ActualShipment.ToString("H:mm");

                        List<string> packingSheets = (from ps in context.PackingSheets
                                                      where ps.OrderDetailID == order.OrderDetailID
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
                        response.Status = DomainObjects.Resource.ResourceData.Failure;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusMessage = DomainObjects.Resource.ResourceData.NoOrders;
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
                            var orderData = context.OrderHeaders.FirstOrDefault(t => t.LegecyOrderNo == order.OrderNo);
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
                                orderData.EstimationShipmentDate = estimationShipmentDate;
                                orderData.ActualShipmentDate = actualShipmentDate;
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
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in OMS.";
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
                                    response.StatusMessage = "Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner1Id = partner1.ID;
                                    partner2Id = partner2.ID;
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
                                        var existingPackingSheet = context.PackingSheets.FirstOrDefault(t => t.OrderDetailID == orderDetailId && t.PackingSheetNo == packinSheet);
                                        if (existingPackingSheet == null)
                                        {
                                            Data.PackingSheet packingSheetRequest = new Data.PackingSheet()
                                            {
                                                OrderDetailID = orderDetailId,
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
                                    LegecyOrderNo = order.OrderNo,
                                    OrderNo = GetOrderNumber(businessAreaId, order.BusinessArea, "OMS", DateTime.Now.Year),
                                    OrderType = order.OrderType,
                                    FleetType = order.FleetType,
                                    VehicleShipment = order.VehicleShipmentType,
                                    DriverNo = order.DriverNo,
                                    DriverName = order.DriverName,
                                    VehicleNo = order.VehicleNo,
                                    OrderWeight = order.OrderWeight,
                                    OrderWeightUM = order.OrderWeightUM,
                                    EstimationShipmentDate = estimationShipmentDate,
                                    ActualShipmentDate = actualShipmentDate,
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
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in OMS.";
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
                                    response.StatusMessage = "Partner not found in OMS.";
                                    return response;
                                }
                                else
                                {
                                    partner1Id = partner1.ID;
                                    partner2Id = partner2.ID;
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
                                            OrderDetailID = orderDetail.ID,
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
                //response.Status = DomainObjects.Resource.ResourceData.Success;
                //response.StatusCode = (int)HttpStatusCode.OK;
                //response.StatusMessage = DomainObjects.Resource.ResourceData.OrderCreated;
            }
            return response;
        }

        public OrderStatusResponse GetAllOrderStatus()
        {
            OrderStatusResponse response = new OrderStatusResponse()
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

        private string GetOrderNumber(int businessAreaId, string businessArea, string applicationCode, int year)
        {
            string orderNo = businessArea + applicationCode;
            using (var context = new Data.OMSDBContext())
            {
                var order = context.OrderHeaders.Where(t => t.BusinessAreaId == businessAreaId).OrderByDescending(t => t.OrderNo).FirstOrDefault();
                if (order != null)
                {
                    int lastOrderYear = order.OrderDate.Year;
                    if (year != lastOrderYear)
                    {
                        orderNo += "000000000001";
                    }
                    else
                    {
                        string orderSequnceString = order.OrderNo.Substring(order.OrderNo.Length - 12);
                        int orderSequnceNumber = Convert.ToInt32(orderSequnceString) + 1;

                        orderNo += orderSequnceNumber.ToString().PadLeft(12, '0');
                    }
                }
                else
                {
                    orderNo += "000000000001";
                }
            }
            return orderNo;
        }
    }
}
