using System;
using System.Collections.Generic;
using System.Linq;
using DataModel = TMS.DataGateway.DataModels;
using Domain = TMS.DomainObjects.Objects;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System.Net;
using NLog;
using Data = TMS.DataGateway.DataModels;
using System.Data.Entity;
using System.Globalization;
using TMS.DomainObjects.Objects;

namespace TMS.DataGateway.Repositories
{
    public class Order : IOrder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderResponse CreateUpdateOrder(OrderRequest request)
        {
            OrderResponse response = new OrderResponse()
            {
                Data = new List<Domain.Order>()
            };

            using (var context = new Data.TMSDBContext())
            {
                foreach (var order in request.Requests)
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            string soPoNumber = String.Empty;
                            DateTime estimationShipmentDate = DateTime.ParseExact(order.EstimationShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.EstimationShipmentTime);
                            DateTime actualShipmentDate = DateTime.ParseExact(order.ActualShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.ActualShipmentTime);

                            #region Step 1: Check if We have Business Area master data
                            int businessAreaId;
                            if (request.UploadType == 2) // Upload via UI
                            {
                                var businessArea = (from ba in context.BusinessAreas
                                                    where ba.ID == order.BusinessAreaId
                                                    select new Domain.BusinessArea()
                                                    {
                                                        ID = ba.ID,
                                                        BusinessAreaCode = ba.BusinessAreaCode
                                                    }).FirstOrDefault();
                                if (businessArea != null)
                                {
                                    businessAreaId = businessArea.ID;
                                }
                                else
                                {
                                    //Return with Business Area not found
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    response.StatusMessage = order.BusinessAreaId + " Business Area ID not found in TMS.";
                                    return response;
                                }

                            }
                            else
                            {

                                var businessArea = (from ba in context.BusinessAreas
                                                    where ba.BusinessAreaCode == order.BusinessArea
                                                    select new Domain.BusinessArea()
                                                    {
                                                        ID = ba.ID,
                                                        BusinessAreaCode = ba.BusinessAreaCode
                                                    }).FirstOrDefault();
                                if (businessArea != null)
                                {
                                    businessAreaId = businessArea.ID;
                                }
                                else
                                {
                                    //Return with Business Area not found
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    response.StatusMessage = order.BusinessArea + " Business Area not found in TMS.";
                                    return response;
                                }
                            }
                            #endregion

                            #region Step 3: Check if We have Order Status in Master data
                            int orderStatusId;
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
                                response.StatusMessage = order.OrderShipmentStatus.ToString() + " Status Code not found in TMS.";
                                return response;
                            }
                            #endregion

                            #region Step 4: Check if Order already existing then update/create accordingly

                            var persistedOrderDataID = (from o in context.OrderHeaders
                                                        where o.LegecyOrderNo == order.OrderNo
                                                        select o.ID
                                                      ).FirstOrDefault();
                            if (request.UploadType == 2)
                            {
                                order.DriverNo = order.DriverNo;
                                order.DriverName = context.Drivers.FirstOrDefault(t => t.DriverNo == order.DriverNo).UserName;
                                soPoNumber = order.SOPONumber;
                            }

                            int orderDetailId = 0;
                            if (persistedOrderDataID > 0)
                            {
                                #region Update Order
                                var updatedOrderHeader = context.OrderHeaders.Find(persistedOrderDataID);
                                updatedOrderHeader.BusinessAreaId = businessAreaId;
                                updatedOrderHeader.OrderType = order.OrderType;
                                updatedOrderHeader.FleetTypeID = order.FleetType;
                                updatedOrderHeader.VehicleShipment = order.VehicleShipmentType;
                                updatedOrderHeader.DriverNo = order.DriverNo;
                                updatedOrderHeader.DriverName = order.DriverName;
                                updatedOrderHeader.VehicleNo = order.VehicleNo;
                                updatedOrderHeader.OrderWeight = order.OrderWeight;
                                updatedOrderHeader.OrderWeightUM = order.OrderWeightUM;
                                updatedOrderHeader.IsActive = true;
                                updatedOrderHeader.LastModifiedBy = request.LastModifiedBy;
                                updatedOrderHeader.LastModifiedTime = DateTime.Now;
                                updatedOrderHeader.OrderStatusID = orderStatusId;
                                updatedOrderHeader.Harga = order.Harga;
                                Data.ImageGuid existingImageGuidDetails = null;
                                string existingImageGUIDValue = string.Empty;
                                int? shipmentScheduleImageID = null;

                                // check if shipmentScheduleImage is changed
                                if (updatedOrderHeader.ShipmentScheduleImageID > 0)
                                {
                                    existingImageGuidDetails = context.ImageGuids.Where(i => i.ID == updatedOrderHeader.ShipmentScheduleImageID).FirstOrDefault();
                                    existingImageGUIDValue = existingImageGuidDetails.ImageGuIdValue;
                                }
                                if (existingImageGUIDValue != order.ShipmentScheduleImageGUID)
                                {
                                    // Making IsActive false for existed record 
                                    if (updatedOrderHeader.ShipmentScheduleImageID > 0 && existingImageGuidDetails != null)
                                    {
                                        existingImageGuidDetails.IsActive = false;
                                    }

                                    //Inserting new record with IsActive true
                                    Data.ImageGuid imageGuid = new Data.ImageGuid()
                                    {
                                        ImageGuIdValue = order.ShipmentScheduleImageGUID,
                                        IsActive = true,
                                        CreatedBy = order.OrderLastModifiedBy,
                                        CreatedTime = DateTime.Now
                                    };

                                    context.ImageGuids.Add(imageGuid);
                                    context.SaveChanges();
                                    shipmentScheduleImageID = imageGuid.ID;
                                }

                                updatedOrderHeader.ShipmentScheduleImageID = shipmentScheduleImageID;

                                context.Entry(updatedOrderHeader).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                order.ID = updatedOrderHeader.ID;
                                response.StatusMessage = DomainObjects.Resource.ResourceData.OrderUpdated;
                                context.Entry(updatedOrderHeader).State = System.Data.Entity.EntityState.Detached;

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
                                        CreatedBy = request.CreatedBy,
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null,
                                        EstimationShipmentDate = estimationShipmentDate,
                                        ActualShipmentDate = actualShipmentDate
                                    };
                                    context.OrderDetails.Add(orderDetail);
                                    context.SaveChanges();
                                    orderDetailId = orderDetail.ID;
                                    #endregion
                                    #region  Update Order Status
                                    string orderStatusCode = order.OrderShipmentStatus.ToString();
                                    Data.OrderStatusHistory orderStatusHistory = new Data.OrderStatusHistory()
                                    {
                                        OrderDetailID = orderDetailId,
                                        OrderStatusID = context.OrderStatuses.Where(t => t.OrderStatusCode == orderStatusCode).FirstOrDefault().ID,
                                        StatusDate = DateTime.Now,
                                        Remarks = "Order Creted"
                                    };

                                    context.OrderStatusHistories.Add(orderStatusHistory);
                                    context.SaveChanges();

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
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                if (request.UploadType == 2)
                                {
                                    partner1Id = Convert.ToInt32(order.PartnerNo1);
                                    partner2Id = Convert.ToInt32(order.PartnerNo2);
                                    partner3Id = Convert.ToInt32(order.PartnerNo3);
                                }
                                else
                                {
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

                                        response.StatusMessage = order.PartnerNo1 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner1Id = partner1.ID;

                                    }
                                    if (partner2 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo2 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner2Id = partner2.ID;
                                    }
                                    if (partner3 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo3 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner3Id = partner3.ID;
                                    }
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
                                int? shipmentScheduleImageID = null;

                                if (!String.IsNullOrEmpty(order.ShipmentScheduleImageGUID))
                                {
                                    //Inserting new record with IsActive true
                                    Data.ImageGuid imageGuid = new Data.ImageGuid()
                                    {
                                        ImageGuIdValue = order.ShipmentScheduleImageGUID,
                                        IsActive = true,
                                        CreatedBy = order.OrderLastModifiedBy,
                                        CreatedTime = DateTime.Now
                                    };

                                    context.ImageGuids.Add(imageGuid);
                                    context.SaveChanges();
                                    shipmentScheduleImageID = imageGuid.ID;
                                }

                                Data.OrderHeader orderHeader = new Data.OrderHeader()
                                {
                                    LegecyOrderNo = order.OrderNo,
                                    OrderNo = order.OrderNo,
                                    OrderType = order.OrderType,
                                    FleetTypeID = order.FleetType,
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
                                    Harga = order.Harga,
                                    ShipmentScheduleImageID = shipmentScheduleImageID,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null,
                                    SOPONumber = soPoNumber,
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
                                orderDetailId = orderDetail.ID;
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
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                if (request.UploadType == 2)
                                {
                                    partner1Id = Convert.ToInt32(order.PartnerNo1);
                                    partner2Id = Convert.ToInt32(order.PartnerNo2);
                                    partner3Id = Convert.ToInt32(order.PartnerNo3);
                                }
                                else
                                {
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

                                        response.StatusMessage = order.PartnerNo1 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner1Id = partner1.ID;

                                    }
                                    if (partner2 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo2 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner2Id = partner2.ID;
                                    }
                                    if (partner3 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo3 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner3Id = partner3.ID;
                                    }
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

                                #endregion

                                #region Step 8: Update Order Status
                                string orderStatusCode = order.OrderShipmentStatus.ToString();
                                Data.OrderStatusHistory orderStatusHistory = new Data.OrderStatusHistory()
                                {
                                    OrderDetailID = orderDetailId,
                                    OrderStatusID = context.OrderStatuses.Where(t => t.OrderStatusCode == orderStatusCode).FirstOrDefault().ID,
                                    StatusDate = DateTime.Now,
                                    Remarks = "Order Creted"
                                };

                                context.OrderStatusHistories.Add(orderStatusHistory);
                                context.SaveChanges();

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

        public OrderSearchResponse GetOrders(OrderSearchRequest orderSearchRequest)
        {
            OrderSearchResponse orderSearchResponse = new OrderSearchResponse()
            {
                Data = new List<Domain.OrderSearch>()
            };

            List<Domain.OrderSearch> orderList;

            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    var userDetails = context.Tokens.Where(t => t.TokenKey == orderSearchRequest.Token).FirstOrDefault();
                    var businessAreas = (from ur in context.UserRoles
                                         where ur.UserID == userDetails.UserID && ur.IsDelete == false
                                         select ur.BusinessAreaID).ToList();

                    orderList = (from oh in context.OrderHeaders
                                 join od in context.OrderDetails on oh.ID equals od.OrderHeaderID
                                 join ps in context.PackingSheets on od.ShippingListNo equals ps.ShippingListNo into pks
                                 from pksh in pks.DefaultIfEmpty()
                                 where businessAreas.Contains(oh.BusinessAreaId) &&
                                 (
                                        ((!String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.OrderNo == orderSearchRequest.GlobalSearch)
                                        || (String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.OrderNo == oh.OrderNo))
                                 ||
                                        ((!String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.VehicleNo == orderSearchRequest.GlobalSearch)
                                        || (String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.VehicleNo == oh.VehicleNo))
                                 ||
                                         ((orderSearchRequest.GlobalSearch != string.Empty && pksh.PackingSheetNo == orderSearchRequest.GlobalSearch)
                                         || (orderSearchRequest.GlobalSearch == string.Empty && pksh.PackingSheetNo == pksh.PackingSheetNo))
                                 )

                                 select new Domain.OrderSearch
                                 {
                                     OrderId = oh.ID,
                                     OrderType = oh.OrderType,
                                     OrderNumber = oh.OrderNo,
                                     VehicleType = context.VehicleTypes.Where(v => v.ID.ToString() == oh.VehicleShipment).Select(vt => vt.VehicleTypeDescription).FirstOrDefault(),
                                     PoliceNumber = oh.VehicleNo,
                                     OrderStatus = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusValue
                                    // PackingSheetNumber = pksh.PackingSheetNo == null ? "" : pksh.PackingSheetNo
                                 }).Distinct().ToList();

                    if (orderList != null && orderList.Count > 0)
                    {
                        foreach (var order in orderList)
                        {
                            var orderData = (from od in context.OrderDetails
                                             where od.OrderHeaderID == order.OrderId
                                             group od by new { od.ID, od.SequenceNo } into gp
                                             select new
                                             {
                                                 OrderDetailId = gp.Key.ID,
                                                 SequenceNo = gp.Max(t => t.SequenceNo),
                                             }).FirstOrDefault();

                            #region Check Order is editable or not
                            // Get all source orderdetailids - iist of int - orderdetail to orderpartnerdetail where partnertypeid = 2
                            List<int> sourceOrderIds = (from orderDetails in context.OrderDetails
                                                        join ordPartnerDetails in context.OrderPartnerDetails on orderDetails.ID equals ordPartnerDetails.OrderDetailID
                                                        where ordPartnerDetails.PartnerTypeId == 2 && orderDetails.OrderHeaderID== order.OrderId
                                                        select orderDetails.ID
                                                      ).ToList();
                            // For each orderdetailid in orderdetailids
                            bool isNotLoaded = true;
                            foreach(int ordDetailId in sourceOrderIds)
                            {
                                var orderStatusHistory = (from osh in context.OrderStatusHistories
                                                         where osh.OrderDetailID == ordDetailId && osh.OrderStatusID == 7
                                                         select osh).FirstOrDefault();
                                if(orderStatusHistory != null)
                                {
                                    isNotLoaded = false;
                                }
                                else
                                {
                                    isNotLoaded = true;
                                    break;
                                }
                            }
                            order.IsOrderEditable = isNotLoaded;
                            #endregion

                            if (orderData != null)
                            {
                                var partnerData = (from op in context.OrderPartnerDetails
                                                   where op.OrderDetailID == orderData.OrderDetailId
                                                   select new
                                                   {
                                                       PrtnerID = op.PartnerID,
                                                       PartnerName = context.Partners.Where(t => t.ID == op.PartnerID).FirstOrDefault().PartnerName,
                                                       partnerTypeID = op.PartnerTypeId
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
                                            if (partner.PartnerTypeCode == "1")
                                                order.ExpeditionName = partner.PartnerName;
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
                    if (orderList != null && orderList.Count > 0 && orderSearchRequest.Requests != null && orderSearchRequest.Requests.Count > 0)
                    {
                        var orderFilter = orderSearchRequest.Requests[0];

                        if (!string.IsNullOrEmpty(orderFilter.OrderNumber))
                        {
                            orderList = orderList.Where(o => o.OrderNumber.Contains(orderFilter.OrderNumber)).ToList();
                        }

                        if (!string.IsNullOrEmpty(orderFilter.PackingSheetNumber))
                        {
                            orderList = orderList.Where(o => o.PackingSheetNumber.Contains(orderFilter.PackingSheetNumber)).ToList();
                        }

                        if (!string.IsNullOrEmpty(orderFilter.PoliceNumber))
                        {
                            orderList = orderList.Where(o => o.PoliceNumber.Contains(orderFilter.PoliceNumber)).ToList();
                        }
                        if (orderFilter.OrderType != 0)
                        {
                            orderList = orderList.Where(o => o.OrderType == orderFilter.OrderType).ToList();
                        }
                    }

                    // Sorting
                    if (orderList != null && orderList.Count > 0)
                    {

                        if (!string.IsNullOrEmpty(orderSearchRequest.SortOrder))
                        {
                            switch (orderSearchRequest.SortOrder.ToLower())
                            {
                                case "ordernumber":
                                    orderList = orderList.OrderBy(o => o.OrderNumber).ToList();
                                    break;
                                case "ordernumber_desc":
                                    orderList = orderList.OrderByDescending(o => o.OrderNumber).ToList();
                                    break;
                                case "source":
                                    orderList = orderList.OrderBy(o => o.Source).ToList();
                                    break;
                                case "source_desc":
                                    orderList = orderList.OrderByDescending(o => o.Source).ToList();
                                    break;
                                case "destination":
                                    orderList = orderList.OrderBy(o => o.Destination).ToList();
                                    break;
                                case "destination_desc":
                                    orderList = orderList.OrderByDescending(o => o.Destination).ToList();
                                    break;
                                case "vehicletype":
                                    orderList = orderList.OrderBy(o => o.VehicleType).ToList();
                                    break;
                                case "vehicletype_desc":
                                    orderList = orderList.OrderByDescending(o => o.VehicleType).ToList();
                                    break;
                                case "expiditionname":
                                    orderList = orderList.OrderBy(o => o.ExpeditionName).ToList();
                                    break;
                                case "expiditionname_desc":
                                    orderList = orderList.OrderByDescending(o => o.ExpeditionName).ToList();
                                    break;
                                case "policenumber":
                                    orderList = orderList.OrderBy(o => o.PoliceNumber).ToList();
                                    break;
                                case "policenumber_desc":
                                    orderList = orderList.OrderByDescending(o => o.PoliceNumber).ToList();
                                    break;
                                case "orderstatus":
                                    orderList = orderList.OrderBy(o => o.OrderStatus).ToList();
                                    break;
                                case "orderstatus_desc":
                                    orderList = orderList.OrderByDescending(o => o.OrderStatus).ToList();
                                    break;
                                default:  // ID Descending 
                                    orderList = orderList.OrderByDescending(o => o.OrderId).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            orderList = orderList.OrderByDescending(o => o.OrderId).ToList();
                        }
                    }

                    // Total NumberOfRecords
                    if (orderList != null)
                        orderSearchResponse.NumberOfRecords = orderList.Count;

                    // Paging
                    int pageNumber = (orderSearchRequest.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(orderSearchRequest.PageSize);
                    if (pageSize > 0)
                    {
                        orderList = orderList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }

                    if (orderList != null && orderList.Count > 0)
                    {
                        orderSearchResponse.Data.AddRange(orderList);
                        orderSearchResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        orderSearchResponse.StatusCode = (int)HttpStatusCode.OK;
                        orderSearchResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        orderSearchResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        orderSearchResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        orderSearchResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderSearchResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderSearchResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderSearchResponse.StatusMessage = ex.Message;
            }
            return orderSearchResponse;
        }

        public OrderTrackResponse TrackOrder(int orderId)
        {
            OrderTrackResponse orderTrackResponse = new OrderTrackResponse()
            {
                Data = new TrackHeader()
                {
                    AcceptOrder = new TrackStep()
                    {
                        StepHeaderName = "ACCEPT ORDER",
                        StepHeaderDescription = "Driver/Transporter accepted order"
                    },
                    Loads = new List<TrackStepLoadUnload>(),
                    Unloads = new List<TrackStepLoadUnload>(),
                    POD = new TrackStep()
                    {
                        StepHeaderName = "POD",
                        StepHeaderDescription = "Driver uploaded shipment document from AHM"
                    },
                    Complete = new TrackStep()
                    {
                        StepHeaderName = "COMPLETE",
                        StepHeaderDescription = "Driver finished the trip"
                    }
                }
            };
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    var orderHeader = (from oh in context.OrderHeaders
                                       where oh.ID == orderId
                                       select new
                                       {
                                           OrderId = oh.ID,
                                           OrderType = oh.OrderType
                                       }).FirstOrDefault();

                    if (orderHeader != null)
                    {
                        //Get Status for the Order
                        var statusData = (from status in context.OrderStatusHistories
                                          join od in context.OrderDetails on status.OrderDetailID equals od.ID
                                          join oh in context.OrderHeaders on od.OrderHeaderID equals oh.ID
                                          where oh.ID == orderHeader.OrderId
                                          orderby status.StatusDate ascending
                                          select new
                                          {
                                              OrderHistoryId = status.ID,
                                              OrderDetailId = status.OrderDetailID,
                                              OrderStatusId = status.OrderStatusID,
                                              StatusCode = context.OrderStatuses.Where(t => t.ID == status.OrderStatusID).FirstOrDefault().OrderStatusCode,
                                              StatusValue = context.OrderStatuses.Where(t => t.ID == status.OrderStatusID).FirstOrDefault().OrderStatusValue,
                                              StatusDate = status.StatusDate,
                                              IsLoad = status.IsLoad,
                                              Remarks = status.Remarks
                                          }).ToList();

                        if (statusData != null && statusData.Count > 0)
                        {
                            #region Accept Order Section
                            foreach (var status in statusData)
                            {
                                if (status.StatusCode == "15")
                                {
                                    orderTrackResponse.Data.AcceptOrder.StepHeaderDateTime = status.StatusDate.ToString("dd MMM yyyy HH:mm");
                                }
                            }
                            #endregion

                            var orderDetails = (from od in context.OrderDetails
                                                where od.OrderHeaderID == orderHeader.OrderId
                                                orderby od.SequenceNo ascending
                                                select new
                                                {
                                                    OrderID = od.OrderHeaderID,
                                                    OrderDetailId = od.ID,
                                                }).ToList();


                            if (orderHeader.OrderType == 1) //For Inbound, There can be multiple loads for every Order Detail
                            {
                                #region Create Load response for In-Bound Orders
                                if (orderDetails != null && orderDetails.Count > 0)
                                {
                                    int sourceNumber = 0;
                                    int totalSources = orderDetails.Count;
                                    foreach (var orderDetail in orderDetails)
                                    {
                                        //Get Source Name for Loading
                                        var partnerData = (from p in context.OrderPartnerDetails
                                                           where p.OrderDetailID == orderDetail.OrderDetailId
                                                           select new
                                                           {
                                                               PrtnerID = p.PartnerID,
                                                               PartnerName = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerName,
                                                               partnerTypeID = p.PartnerTypeId
                                                           }).ToList();
                                        if (partnerData != null && partnerData.Count > 0)
                                        {
                                            var partners = (from pd in partnerData
                                                            join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                                                            where pt.PartnerTypeCode == "2"
                                                            select new
                                                            {
                                                                OrderDetailID = orderDetail.OrderDetailId,
                                                                PrtnerID = pd.PrtnerID,
                                                                PartnerName = pd.PartnerName,
                                                                partnerTypeID = pd.partnerTypeID,
                                                                PartnerTypeCode = pt.PartnerTypeCode
                                                            }).ToList();
                                            if (partners != null && partners.Count > 0)
                                            {
                                                foreach (var partner in partners)
                                                {
                                                    var loadStatusData = statusData.Where(t => t.IsLoad == true).ToList();
                                                    if (loadStatusData != null && loadStatusData.Count > 0)
                                                    {
                                                        TrackStepLoadUnload loadData = new TrackStepLoadUnload()
                                                        {
                                                            TrackLoadUnloadName = "LOAD",
                                                            StepHeaderNotification = String.Format("{0} from {1} AHM", ++sourceNumber, totalSources),
                                                            StartTrip = new TrackStep(),
                                                            ConfirmArrive = new TrackStep(),
                                                            StartLoad = new TrackStep(),
                                                            FinishLoad = new TrackStep()
                                                        };

                                                        foreach (var loadStatus in loadStatusData)
                                                        {
                                                            if (loadStatus.StatusCode == "4" && loadStatus.OrderDetailId == orderDetail.OrderDetailId)
                                                            {
                                                                loadData.StartTrip.StepHeaderName = "START TRIP";
                                                                loadData.StartTrip.StepHeaderDescription = "On the way to " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    loadData.StartTrip.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }

                                                            }
                                                            else if (loadStatus.StatusCode == "5" && loadStatus.OrderDetailId == orderDetail.OrderDetailId)
                                                            {
                                                                loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                                loadData.ConfirmArrive.StepHeaderDescription = "Arrived at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    loadData.ConfirmArrive.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                            else if (loadStatus.StatusCode == "6" && loadStatus.OrderDetailId == orderDetail.OrderDetailId)
                                                            {
                                                                loadData.StartLoad.StepHeaderName = "START LOAD";
                                                                loadData.StartLoad.StepHeaderDescription = "Loading parts at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    loadData.StartLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                            else if (loadStatus.StatusCode == "7" && loadStatus.OrderDetailId == orderDetail.OrderDetailId)
                                                            {
                                                                loadData.FinishLoad.StepHeaderName = "FINISH LOAD";
                                                                loadData.FinishLoad.StepHeaderDescription = "Parts loaded at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    loadData.FinishLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                        }
                                                        orderTrackResponse.Data.Loads.Add(loadData);
                                                    }
                                                    else
                                                    {
                                                        TrackStepLoadUnload loadData = new TrackStepLoadUnload()
                                                        {
                                                            TrackLoadUnloadName = "LOAD",
                                                            StepHeaderNotification = String.Format("{0} from {1} AHM", ++sourceNumber, totalSources),
                                                            StartTrip = new TrackStep(),
                                                            ConfirmArrive = new TrackStep(),
                                                            StartLoad = new TrackStep(),
                                                            FinishLoad = new TrackStep()
                                                        };

                                                        loadData.StartTrip.StepHeaderName = "START TRIP";
                                                        loadData.StartTrip.StepHeaderDescription = "On the way to AHM";

                                                        loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                        loadData.ConfirmArrive.StepHeaderDescription = "Arrived at AHM";

                                                        loadData.StartLoad.StepHeaderName = "START LOAD";
                                                        loadData.StartLoad.StepHeaderDescription = "Loading parts at AHM";

                                                        loadData.FinishLoad.StepHeaderName = "FINISH LOAD";
                                                        loadData.FinishLoad.StepHeaderDescription = "Parts loaded at AHM";

                                                        orderTrackResponse.Data.Loads.Add(loadData);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Create UnLoad response for In-bound Orders
                                var orderDetailUnload = (from od in context.OrderDetails
                                                         where od.OrderHeaderID == orderHeader.OrderId
                                                         group od by new { od.ID, od.SequenceNo } into gp
                                                         select new
                                                         {
                                                             OrderDetailId = gp.Key.ID,
                                                             SequenceNo = gp.Max(t => t.SequenceNo),
                                                         }).FirstOrDefault();

                                if (orderDetailUnload != null)
                                {
                                    //Get Source Name for Loading
                                    var partnerData = (from p in context.OrderPartnerDetails
                                                       where p.OrderDetailID == orderDetailUnload.OrderDetailId
                                                       select new
                                                       {
                                                           PrtnerID = p.PartnerID,
                                                           PartnerName = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerName,
                                                           partnerTypeID = p.PartnerTypeId
                                                       }).ToList();
                                    if (partnerData != null && partnerData.Count > 0)
                                    {
                                        var partners = (from pd in partnerData
                                                        join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                                                        where pt.PartnerTypeCode == "3"
                                                        select new
                                                        {
                                                            OrderDetailID = orderDetailUnload.OrderDetailId,
                                                            PrtnerID = pd.PrtnerID,
                                                            PartnerName = pd.PartnerName,
                                                            partnerTypeID = pd.partnerTypeID,
                                                            PartnerTypeCode = pt.PartnerTypeCode
                                                        }).FirstOrDefault();
                                        if (partners != null)
                                        {
                                            var unLoadStatusData = statusData.Where(t => t.IsLoad == false).ToList();
                                            if (unLoadStatusData != null && unLoadStatusData.Count > 0)
                                            {
                                                TrackStepLoadUnload unLoadData = new TrackStepLoadUnload()
                                                {
                                                    TrackLoadUnloadName = "UNLOAD",
                                                    StartTrip = new TrackStep(),
                                                    ConfirmArrive = new TrackStep(),
                                                    StartLoad = new TrackStep(),
                                                    FinishLoad = new TrackStep(),
                                                    StepHeaderNotification = "Unloaded at Main Dealer"
                                                };
                                                foreach (var unLoadStatus in unLoadStatusData)
                                                {
                                                    if (unLoadStatus.StatusCode == "4")
                                                    {
                                                        unLoadData.StartTrip.StepHeaderName = "START TRIP";
                                                        unLoadData.StartTrip.StepHeaderDescription = "On the way to " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        unLoadData.StartTrip.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "5")
                                                    {
                                                        unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                        unLoadData.ConfirmArrive.StepHeaderDescription = "Arrived at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        unLoadData.ConfirmArrive.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "9")
                                                    {
                                                        unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                        unLoadData.StartLoad.StepHeaderDescription = "Unloading parts at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        unLoadData.StartLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "10")
                                                    {
                                                        unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                        unLoadData.FinishLoad.StepHeaderDescription = "Parts unloaded at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        unLoadData.FinishLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                }
                                                orderTrackResponse.Data.Unloads.Add(unLoadData);
                                            }
                                            else
                                            {
                                                TrackStepLoadUnload unLoadData = new TrackStepLoadUnload()
                                                {
                                                    TrackLoadUnloadName = "UNLOAD",
                                                    StartTrip = new TrackStep(),
                                                    ConfirmArrive = new TrackStep(),
                                                    StartLoad = new TrackStep(),
                                                    FinishLoad = new TrackStep()
                                                };
                                                unLoadData.StartTrip.StepHeaderName = "START TRIP";
                                                unLoadData.StartTrip.StepHeaderDescription = "On the way to Main Dealer";

                                                unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                unLoadData.ConfirmArrive.StepHeaderDescription = "Arrived at Main Dealer";

                                                unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                unLoadData.StartLoad.StepHeaderDescription = "Unloading parts at Main Dealer";

                                                unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                unLoadData.FinishLoad.StepHeaderDescription = "Parts unloaded at Main Dealer";
                                                orderTrackResponse.Data.Unloads.Add(unLoadData);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (orderHeader.OrderType == 2) //For Outbound, There can be multiple unloads for every Order Detail
                            {
                                #region Create Load response for Out-bound Orders
                                var orderDetailsOutbound = (from od in context.OrderDetails
                                                            where od.OrderHeaderID == orderHeader.OrderId
                                                            group od by new { od.ID, od.SequenceNo } into gp
                                                            select new
                                                            {
                                                                OrderDetailId = gp.Key.ID,
                                                                SequenceNo = gp.Max(t => t.SequenceNo),
                                                            }).FirstOrDefault();

                                if (orderDetailsOutbound != null)
                                {
                                    //Get Source Name for Loading
                                    var partnerDataOutbound = (from p in context.OrderPartnerDetails
                                                               where p.OrderDetailID == orderDetailsOutbound.OrderDetailId
                                                               select new
                                                               {
                                                                   PrtnerID = p.PartnerID,
                                                                   PartnerName = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerName,
                                                                   partnerTypeID = p.PartnerTypeId
                                                               }).ToList();
                                    if (partnerDataOutbound != null && partnerDataOutbound.Count > 0)
                                    {
                                        var partners = (from pd in partnerDataOutbound
                                                        join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                                                        where pt.PartnerTypeCode == "2"
                                                        select new
                                                        {
                                                            OrderDetailID = orderDetailsOutbound.OrderDetailId,
                                                            PrtnerID = pd.PrtnerID,
                                                            PartnerName = pd.PartnerName,
                                                            partnerTypeID = pd.partnerTypeID,
                                                            PartnerTypeCode = pt.PartnerTypeCode
                                                        }).FirstOrDefault();
                                        if (partners != null)
                                        {
                                            var unLoadStatusData = statusData.Where(t => t.IsLoad == true).ToList();
                                            if (unLoadStatusData != null && unLoadStatusData.Count > 0)
                                            {
                                                TrackStepLoadUnload loadData = new TrackStepLoadUnload()
                                                {
                                                    TrackLoadUnloadName = "LOAD",
                                                    StartTrip = new TrackStep(),
                                                    ConfirmArrive = new TrackStep(),
                                                    StartLoad = new TrackStep(),
                                                    FinishLoad = new TrackStep(),
                                                    StepHeaderNotification = "Loaded at Main Dealer"
                                                };
                                                foreach (var unLoadStatus in unLoadStatusData)
                                                {
                                                    if (unLoadStatus.StatusCode == "4")
                                                    {
                                                        loadData.StartTrip.StepHeaderName = "START TRIP";
                                                        loadData.StartTrip.StepHeaderDescription = "On the way to " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        loadData.StartTrip.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "5")
                                                    {
                                                        loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                        loadData.ConfirmArrive.StepHeaderDescription = "Arrived at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        loadData.ConfirmArrive.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "6")
                                                    {
                                                        loadData.StartLoad.StepHeaderName = "START LOAD";
                                                        loadData.StartLoad.StepHeaderDescription = "Loading parts at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        loadData.StartLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "7")
                                                    {
                                                        loadData.FinishLoad.StepHeaderName = "FINISH LOAD";
                                                        loadData.FinishLoad.StepHeaderDescription = "Parts loaded at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == unLoadStatus.OrderDetailId && o.PartnerTypeId == 2).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                        loadData.FinishLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                }
                                                orderTrackResponse.Data.Loads.Add(loadData);
                                            }
                                            else
                                            {
                                                TrackStepLoadUnload loadData = new TrackStepLoadUnload()
                                                {
                                                    TrackLoadUnloadName = "LOAD",
                                                    StartTrip = new TrackStep(),
                                                    ConfirmArrive = new TrackStep(),
                                                    StartLoad = new TrackStep(),
                                                    FinishLoad = new TrackStep()
                                                };
                                                loadData.StartTrip.StepHeaderName = "START TRIP";
                                                loadData.StartTrip.StepHeaderDescription = "On the way to Main Dealer";

                                                loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                loadData.ConfirmArrive.StepHeaderDescription = "Arrived at Main Dealer";

                                                loadData.StartLoad.StepHeaderName = "START LOAD";
                                                loadData.StartLoad.StepHeaderDescription = "Loading parts at Main Dealer";

                                                loadData.FinishLoad.StepHeaderDescription = "Parts loaded at Main Dealer";
                                                orderTrackResponse.Data.Loads.Add(loadData);
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Create Unload response for Out-Bound Orders
                                var orderDetailOutBound = (from od in context.OrderDetails
                                                           where od.OrderHeaderID == orderHeader.OrderId
                                                           orderby od.SequenceNo ascending
                                                           select new
                                                           {
                                                               OrderID = od.OrderHeaderID,
                                                               OrderDetailId = od.ID,
                                                           }).ToList();

                                if (orderDetailOutBound != null && orderDetailOutBound.Count > 0)
                                {
                                    int sourceNumber = 0;
                                    int totalSources = orderDetails.Count;
                                    foreach (var orderDetail in orderDetails)
                                    {
                                        //Get Source Name for UnLoading
                                        var partnerDataUnload = (from p in context.OrderPartnerDetails
                                                                 where p.OrderDetailID == orderDetail.OrderDetailId
                                                                 select new
                                                                 {
                                                                     PrtnerID = p.PartnerID,
                                                                     PartnerName = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerName,
                                                                     partnerTypeID = p.PartnerTypeId
                                                                 }).ToList();
                                        if (partnerDataUnload != null && partnerDataUnload.Count > 0)
                                        {
                                            var partners = (from pd in partnerDataUnload
                                                            join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                                                            where pt.PartnerTypeCode == "3"
                                                            select new
                                                            {
                                                                OrderDetailID = orderDetail.OrderDetailId,
                                                                PrtnerID = pd.PrtnerID,
                                                                PartnerName = pd.PartnerName,
                                                                partnerTypeID = pd.partnerTypeID,
                                                                PartnerTypeCode = pt.PartnerTypeCode
                                                            }).ToList();
                                            if (partners != null && partnerDataUnload.Count > 0)
                                            {
                                                foreach (var partner in partners)
                                                {
                                                    var loadStatusData = statusData.Where(t => t.IsLoad == false && t.OrderDetailId == orderDetail.OrderDetailId).ToList();
                                                    if (loadStatusData != null && loadStatusData.Count > 0)
                                                    {
                                                        TrackStepLoadUnload unLoadData = new TrackStepLoadUnload()
                                                        {
                                                            TrackLoadUnloadName = "UNLOAD",
                                                            StepHeaderNotification = String.Format("{0} from {1} Dealers", ++sourceNumber, totalSources),
                                                            StartTrip = new TrackStep(),
                                                            ConfirmArrive = new TrackStep(),
                                                            StartLoad = new TrackStep(),
                                                            FinishLoad = new TrackStep()
                                                        };

                                                        foreach (var loadStatus in loadStatusData)
                                                        {
                                                            if (loadStatus.StatusCode == "4")
                                                            {
                                                                unLoadData.StartTrip.StepHeaderName = "START TRIP";
                                                                unLoadData.StartTrip.StepHeaderDescription = "On the way to " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    unLoadData.StartTrip.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                            else if (loadStatus.StatusCode == "5")
                                                            {
                                                                unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                                unLoadData.ConfirmArrive.StepHeaderDescription = "Arrived at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    unLoadData.ConfirmArrive.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                            else if (loadStatus.StatusCode == "9")
                                                            {
                                                                unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                                unLoadData.StartLoad.StepHeaderDescription = "Unloading parts at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    unLoadData.StartLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                            else if (loadStatus.StatusCode == "10")
                                                            {
                                                                unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                                unLoadData.FinishLoad.StepHeaderDescription = "Parts unloaded at " + context.OrderPartnerDetails.Where(o => o.OrderDetailID == loadStatus.OrderDetailId && o.PartnerTypeId == 3).Select(n => n.Partner.PartnerName).FirstOrDefault();
                                                                if (orderDetail.OrderDetailId == loadStatus.OrderDetailId)
                                                                {
                                                                    unLoadData.FinishLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                }
                                                            }
                                                        }
                                                        orderTrackResponse.Data.Unloads.Add(unLoadData);
                                                    }
                                                    else
                                                    {
                                                        TrackStepLoadUnload unLoadData = new TrackStepLoadUnload()
                                                        {
                                                            TrackLoadUnloadName = "UNLOAD",
                                                            StepHeaderNotification = String.Format("{0} from {1} Dealers", ++sourceNumber, totalSources),
                                                            StartTrip = new TrackStep(),
                                                            ConfirmArrive = new TrackStep(),
                                                            StartLoad = new TrackStep(),
                                                            FinishLoad = new TrackStep()
                                                        };

                                                        unLoadData.StartTrip.StepHeaderName = "START TRIP";
                                                        unLoadData.StartTrip.StepHeaderDescription = "On the way to Dealer";

                                                        unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                        unLoadData.ConfirmArrive.StepHeaderDescription = "Arrived at Dealer";

                                                        unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                        unLoadData.StartLoad.StepHeaderDescription = "UnlLoading parts at Dealer";

                                                        unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                        unLoadData.FinishLoad.StepHeaderDescription = "Parts unloaded at Dealer";

                                                        orderTrackResponse.Data.Unloads.Add(unLoadData);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            #region POD Section
                            foreach (var status in statusData)
                            {
                                if (status.StatusCode == "11")
                                {
                                    orderTrackResponse.Data.POD.StepHeaderDateTime = status.StatusDate.ToString("dd MMM yyyy HH:mm");
                                }
                            }
                            #endregion

                            #region Complete Order Section
                            foreach (var status in statusData)
                            {
                                if (status.StatusCode == "12")
                                {
                                    orderTrackResponse.Data.Complete.StepHeaderDateTime = status.StatusDate.ToString("dd MMM yyyy HH:mm");
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderTrackResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderTrackResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderTrackResponse.StatusMessage = ex.Message;
            }

            return orderTrackResponse;
        }

        public PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            PackingSheetResponse packingSheetResponse = new PackingSheetResponse();
            packingSheetResponse.Data = new List<PackingSheet>();
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    foreach (var packingSheet in packingSheetRequest.Requests)
                    {
                        var orderDetailsData = context.OrderDetails.Where(x => x.ID == packingSheet.OrderDetailId).FirstOrDefault();

                        if (orderDetailsData != null)
                        {
                            var partnerdetails = context.OrderPartnerDetails.Where(x => x.OrderDetailID == packingSheet.OrderDetailId && x.PartnerID == packingSheet.DealerId).FirstOrDefault();

                            PackingSheet ps = new PackingSheet();
                            ps.OrderNumber = context.OrderHeaders.Where(o => o.ID == orderDetailsData.OrderHeaderID).Select(oh => oh.OrderNo).FirstOrDefault();
                            ps.DealerNumber = context.Partners.Where(o => o.ID == partnerdetails.PartnerID).Select(oh => oh.PartnerNo).FirstOrDefault();
                            ps.DealerId = packingSheet.DealerId;

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
                            packingSheetResponse.Data.Add(ps);
                            packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            packingSheetResponse.StatusMessage = DomainObjects.Resource.ResourceData.PackingSheetUpdated;
                            packingSheetResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Failure;
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

        public PackingSheetResponse GetPackingSheetDetails(int orderId)
        {
            PackingSheetResponse packingSheetResponse = new PackingSheetResponse();
            List<PackingSheet> packingSheets = new List<PackingSheet>();
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    var orderDetailsData = context.OrderDetails.Where(x => x.OrderHeaderID == orderId).ToList();
                    if (orderDetailsData != null)
                    {
                        foreach (var item in orderDetailsData)
                        {
                            PackingSheet pack = new PackingSheet();
                            pack.OrderNumber = context.OrderHeaders.Where(o => o.ID == item.OrderHeaderID).Select(p => p.OrderNo).FirstOrDefault();
                            pack.Collie = item.TotalCollie;
                            pack.Katerangan = item.Katerangan;
                            pack.Notes = item.Instruction;
                            pack.OrderDetailId = item.ID;
                            pack.ShippingListNo = item.ShippingListNo;
                            pack.Katerangan = item.Katerangan;
                            pack.DealerId = context.OrderPartnerDetails.Where(p => p.OrderDetailID == item.ID).Select(p => p.PartnerID).FirstOrDefault();
                            pack.DealerNumber = context.Partners.Where(p => p.ID == pack.DealerId).Select(p => p.PartnerNo).FirstOrDefault();
                            pack.DealerName = context.Partners.Where(p => p.ID == pack.DealerId).Select(p => p.PartnerName).FirstOrDefault();
                            var packingSheetNos = context.PackingSheets.Where(ps => ps.ShippingListNo == item.ShippingListNo).Select(i => new Common { Id = i.ID, Value = i.PackingSheetNo }).ToList();
                            if (packingSheetNos.Count > 0)
                            {
                                pack.PackingSheetNumbers = packingSheetNos;
                            }
                            packingSheets.Add(pack);
                        }
                        packingSheetResponse.Data = packingSheets;
                        packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        packingSheetResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                        packingSheetResponse.StatusCode = (int)HttpStatusCode.OK;
                        packingSheetResponse.NumberOfRecords = packingSheets.Count;
                    }
                    else
                    {
                        packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        packingSheetResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                        packingSheetResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        packingSheetResponse.NumberOfRecords = 0;
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

        public CommonResponse GetOrderIds(string tokenValue)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    var userDetails = context.Tokens.Where(t => t.TokenKey == tokenValue).FirstOrDefault();
                    var businessAreas = (from ur in context.UserRoles
                                         where ur.UserID == userDetails.UserID && !ur.IsDelete
                                         select ur.BusinessAreaID).ToList();

                    var orderData = (from orderHeader in context.OrderHeaders
                                     where businessAreas.Contains(orderHeader.BusinessAreaId) &&
                                     orderHeader.OrderType == (context.OrderTypes.Where(ot => ot.OrderTypeCode == "INBD").Select(p => p.ID).FirstOrDefault()

                                     )
                                     select new Domain.Common
                                     {
                                         Id = orderHeader.ID,
                                         Value = orderHeader.OrderNo
                                     }
                                     ).ToList();
                    if (orderData.Count > 0)
                    {
                        commonResponse.NumberOfRecords = orderData.Count;
                        commonResponse.Data = orderData;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        commonResponse.NumberOfRecords = 0;
                        commonResponse.Data = null;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                commonResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonResponse.StatusMessage = ex.Message;
            }
            return commonResponse;
        }

        public DealerDetailsResponse GetDealers(int orderId, string searchText)
        {
            DealerDetailsResponse dealerDetailsResponse = new DealerDetailsResponse();
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    var delearData = new List<Domain.DealerDetails>();
                    if (searchText != "" && searchText != null)
                    {

                        delearData = (from orderHeader in context.OrderHeaders
                                      join orderDetails in context.OrderDetails on orderHeader.ID equals orderDetails.OrderHeaderID
                                      join opd in context.OrderPartnerDetails on orderDetails.ID equals opd.OrderDetailID
                                      where orderHeader.ID == orderId && opd.PartnerTypeId == context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == "2").ID
                                      && opd.Partner.PartnerName.Contains(searchText)
                                      select new Domain.DealerDetails
                                      {
                                          DealerId = opd.PartnerID,
                                          DealerName = opd.Partner.PartnerName,
                                          DealerNumber = opd.Partner.PartnerNo,
                                          OrderDeatialId = orderDetails.ID
                                      }
                                      ).ToList();
                    }
                    else
                    {
                        delearData = (from orderHeader in context.OrderHeaders
                                      join orderDetails in context.OrderDetails on orderHeader.ID equals orderDetails.OrderHeaderID
                                      join opd in context.OrderPartnerDetails on orderDetails.ID equals opd.OrderDetailID
                                      where orderHeader.ID == orderId && opd.PartnerTypeId == context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == "2").ID
                                      select new Domain.DealerDetails
                                      {
                                          DealerId = opd.PartnerID,
                                          DealerName = opd.Partner.PartnerName,
                                          DealerNumber = opd.Partner.PartnerNo,
                                          OrderDeatialId = orderDetails.ID
                                      }
                                    ).ToList();
                    }
                    if (delearData.Count > 0)
                    {
                        dealerDetailsResponse.NumberOfRecords = delearData.Count;
                        dealerDetailsResponse.Data = delearData;
                        dealerDetailsResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        dealerDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                        dealerDetailsResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        dealerDetailsResponse.NumberOfRecords = 0;
                        dealerDetailsResponse.Data = null;
                        dealerDetailsResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        dealerDetailsResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        dealerDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }


                }

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                dealerDetailsResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                dealerDetailsResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                dealerDetailsResponse.StatusMessage = ex.Message;
            }
            return dealerDetailsResponse;

        }

        public OrderDetailsResponse GetOrderDetails(int orderId)
        {
            OrderDetailsResponse orderDetailsResponse = new OrderDetailsResponse();
            try
            {
                using (var context = new Data.TMSDBContext())
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
                        var orderPackingSheetData = (from od in context.OrderDetails
                                               join ps in context.PackingSheets on od.ShippingListNo equals ps.ShippingListNo
                                               where od.OrderHeaderID == orderData.ID
                                               select ps
                                               ).FirstOrDefault();

                        if(orderPackingSheetData != null){
                            orderData.PackingSheetNo = orderPackingSheetData.PackingSheetNo;
                            orderData.IsPackingSheetAvailable = true;
                        }
                        else
                        {
                            orderData.PackingSheetNo = "";
                            orderData.IsPackingSheetAvailable = false;
                        }

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
                        orderDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
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

        public Domain.Partner GetPartnerDetail(string partnerNo, int uploadType)
        {
            Domain.Partner response = new Domain.Partner();

            using (var context = new DataModel.TMSDBContext())
            {
                try
                {
                    if (uploadType == 2)
                    {
                        int partnerId = Convert.ToInt32(partnerNo);
                        response = (from partner in context.Partners
                                        //join postalcode in context.PostalCodes on partner.PostalCodeID equals postalcode.ID
                                    join subDistrict in context.SubDistricts on partner.SubDistrictID equals subDistrict.ID
                                    where partner.ID == partnerId
                                    select new Domain.Partner
                                    {
                                        PartnerAddress = partner.PartnerAddress,
                                        CityCode = subDistrict.City.CityCode,
                                        ProvinceCode = subDistrict.City.Province.ProvinceCode,
                                        PartnerName = partner.PartnerName,
                                        PartnerNo = partner.PartnerNo,
                                        PartnerEmail = partner.PartnerEmail
                                    }).FirstOrDefault();
                    }
                    else
                    {
                        response = (from partner in context.Partners
                                        //join postalcode in context.PostalCodes on partner.PostalCodeID equals postalcode.ID
                                    join subDistrict in context.SubDistricts on partner.SubDistrictID equals subDistrict.ID
                                    where partner.PartnerNo == partnerNo
                                    select new Domain.Partner
                                    {
                                        PartnerAddress = partner.PartnerAddress,
                                        CityCode = subDistrict.City.CityCode,
                                        ProvinceCode = subDistrict.City.Province.ProvinceCode,
                                        PartnerName = partner.PartnerName,
                                        PartnerNo = partner.PartnerNo
                                    }).FirstOrDefault();
                    }

                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                }
            }
            return response;
        }

        public string GetBusinessAreaCode(int businessAreaId)
        {
            string businessAreaCode = "";
            using (var context = new DataModel.TMSDBContext())
            {
                try
                {
                    businessAreaCode = (from ba in context.BusinessAreas
                                        where ba.ID == businessAreaId
                                        select new
                                        {
                                            BusinessAreaCode = ba.BusinessAreaCode
                                        }).FirstOrDefault().BusinessAreaCode;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                }
            }
            return businessAreaCode;
        }

        public OrderStatusResponse UpdateOrderStatus(OrderStatusRequest request)
        {
            _logger.Log(LogLevel.Error, request);
            OrderStatusResponse response = new OrderStatusResponse()
            {
                Data = new List<OrderStatus>()
            };

            using (var context = new DataModel.TMSDBContext())
            {
                try
                {
                    foreach (var statusRequest in request.Requests)
                    {
                        int orderId = 0;
                        int orderDetailId = 0;
                        orderId = context.OrderHeaders.FirstOrDefault(t => t.OrderNo == statusRequest.OrderNumber).ID;

                        if (statusRequest.SequenceNumber > 0)
                        {
                            orderDetailId = context.OrderDetails.FirstOrDefault(t => t.SequenceNo == statusRequest.SequenceNumber && t.OrderHeaderID == orderId).ID;
                        }
                        else
                        {
                            orderDetailId = context.OrderDetails.FirstOrDefault(t => t.OrderHeaderID == orderId).ID;
                        }

                        DataModel.OrderStatusHistory statusHistory = new DataModel.OrderStatusHistory()
                        {
                            OrderDetailID = orderDetailId,
                            OrderStatusID = context.OrderStatuses.FirstOrDefault(t => t.OrderStatusCode == statusRequest.OrderStatusCode).ID,
                            IsLoad = statusRequest.IsLoad,
                            Remarks = statusRequest.Remarks,
                            StatusDate = DateTime.Now

                        };
                        context.OrderStatusHistories.Add(statusHistory);
                        context.SaveChanges();

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

        public OrderStatusResponse CancelOrder(OrderStatusRequest request)
        {
            OrderStatusResponse response = new OrderStatusResponse()
            {
                Data = new List<OrderStatus>()
            };

            using (var context = new DataModel.TMSDBContext())
            {
                try
                {
                    foreach (var statusRequest in request.Requests)
                    {
                        var orderHeader = context.OrderHeaders.Where(oh => oh.OrderNo == statusRequest.OrderNumber).FirstOrDefault();
                        var orderDetailData = context.OrderDetails.Where(od => od.OrderHeaderID == orderHeader.ID).ToList();

                        if (orderDetailData.Count > 0)
                        {
                            foreach (var orderDetail in orderDetailData)
                            {
                                DataModel.OrderStatusHistory statusHistory = new DataModel.OrderStatusHistory()
                                {
                                    OrderDetailID = orderDetail.ID,
                                    OrderStatusID = context.OrderStatuses.FirstOrDefault(t => t.OrderStatusCode == "13").ID,
                                    Remarks = statusRequest.Remarks,
                                    StatusDate = DateTime.Now

                                };
                                context.OrderStatusHistories.Add(statusHistory);
                                context.SaveChanges();
                            }
                            #region Update Order Header
                            orderHeader.OrderStatusID = context.OrderStatuses.FirstOrDefault(t => t.OrderStatusCode == "13").ID;

                            context.Entry(orderHeader).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            context.Entry(orderHeader).State = System.Data.Entity.EntityState.Detached;
                            #endregion
                            response.Status = DomainObjects.Resource.ResourceData.Success;
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusMessage = DomainObjects.Resource.ResourceData.OrderCanceled;
                        }
                        else
                        {
                            response.Status = DomainObjects.Resource.ResourceData.Success;
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            response.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                        }
                        response.Data = request.Requests;
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

        public HargaResponse GetHarga(HargaRequest request)
        {
            HargaResponse response = new HargaResponse()
            {
                Data = new List<Harga>()
            };

            Harga hargaRequest = request.Requests[0];

            using (var context = new DataModel.TMSDBContext())
            {
                DataModel.Harga result = context.Hargas.Where(h => h.TransporterID == hargaRequest.TransporterID && h.VechicleTypeID == hargaRequest.VechicleTypeID).FirstOrDefault();
                if (result != null)
                {
                    Harga harga = new Harga()
                    {
                        ID = result.ID,
                        TransporterID = result.TransporterID,
                        VechicleTypeID = result.VechicleTypeID,
                        Price = result.Price
                    };
                    response.Data.Add(harga);
                }
                else
                {
                    Harga harga = new Harga()
                    {
                        TransporterID = hargaRequest.TransporterID,
                        VechicleTypeID = hargaRequest.VechicleTypeID
                    };
                    response.Data.Add(harga);
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Status = DomainObjects.Resource.ResourceData.Success;
                response.StatusMessage = DomainObjects.Resource.ResourceData.Success;
            }

            return response;
        }

        public ShipmentScheduleOcrResponse CreateOrderFromShipmentScheduleOcr(ShipmentScheduleOcrRequest request)
        {
            ShipmentScheduleOcrResponse response = new ShipmentScheduleOcrResponse()
            {
                Data = new List<Domain.ShipmentScheduleOcr>()
            };

            using (var context = new Data.TMSDBContext())
            {
                foreach (var shipment in request.Requests)
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusMessage = DomainObjects.Resource.ResourceData.OrderCreated;

                            string soPoNumber = String.Empty;
                            if (shipment.Data.ShipmentScheduleNo.Split('-').Length > 0)
                            {
                                soPoNumber = shipment.Data.ShipmentScheduleNo.Split('-')[4];
                            }

                            #region Step 1: Check if We have Business Area master data
                            int businessAreaId;
                            string businessAreaCode = string.Empty;

                            var mdBusinessAreaMappings = (from bam in context.MDBusinessAreaMappings
                                                          where bam.MainDealerCode == shipment.Data.MainDealerCode
                                                          select new Domain.BusinessArea()
                                                          {
                                                              BusinessAreaCode = bam.BusinessAreaCode
                                                          }).FirstOrDefault();
                            if (mdBusinessAreaMappings != null)
                            {
                                var businessArea = (from ba in context.BusinessAreas
                                                    where ba.BusinessAreaCode == mdBusinessAreaMappings.BusinessAreaCode
                                                    select new Domain.BusinessArea()
                                                    {
                                                        BusinessAreaCode = ba.BusinessAreaCode,
                                                        ID = ba.ID
                                                    }).FirstOrDefault();
                                if (businessArea != null)
                                {
                                    businessAreaCode = businessArea.BusinessAreaCode;
                                    businessAreaId = businessArea.ID;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.Status = DomainObjects.Resource.ResourceData.Failure;
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    response.StatusMessage = mdBusinessAreaMappings.BusinessAreaCode + " Business Area Code not found in TMS.";
                                    return response;
                                }

                            }
                            else
                            {
                                //Return with Business Area not found
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = shipment.Data.MainDealerCode + " Main Dealer Code found in TMS.";
                                return response;
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

        public OrderResponse OcrOrderResponse(ShipmentScheduleOcrRequest shipmentScheduleOcrRequest)
        {
            OrderResponse orderResponse = new OrderResponse();
            orderResponse.Data = new List<Domain.Order>();


            using (var context = new Data.TMSDBContext())
            {
                foreach (var shipment in shipmentScheduleOcrRequest.Requests)
                {
                    try
                    {
                        // save ShipmentScheduleOCRDetails into ShipmentScheduleOCRDetails log
                        DataModel.ShipmentScheduleOCRDetails shipmentScheduleOCRDetails = new DataModel.ShipmentScheduleOCRDetails()
                        {
                            EmailFrom = shipment.EmailFrom,
                            EmailDateTime = shipment.EmailDateTime,

                            ShipmentScheduleNo = shipment.Data.ShipmentScheduleNo,
                            DayShipment = shipment.Data.DayShipment,
                            ShipmentTime = shipment.Data.ShipmentTime,
                            VehicleType = shipment.Data.VehicleType,
                            MainDealerCode = shipment.Data.MainDealerCode,
                            MainDealerName = shipment.Data.MainDealerName,
                            ShipToParty = shipment.Data.ShipToParty,
                            MultiDropShipment = shipment.Data.MultiDropShipment,
                            EstimatedTotalPallet = shipment.Data.EstimatedTotalPallet,
                            Weight = shipment.Data.Weight,
                            IsProcessed = true,
                            IsOrderCreated = false,
                            ImageGuid = shipment.ImageGUID,
                            ProcessedDateTime = DateTime.Now,
                            ProcessedBy = "System",
                        };
                        context.ShipmentScheduleOCRDetails.Add(shipmentScheduleOCRDetails);
                        context.SaveChanges();
                        /////


                        string soPoNumber = String.Empty;
                        string actualShipmentDate = String.Empty;
                        string actualShipmentTime = String.Empty;
                        if (shipment.Data.ShipmentScheduleNo.Split('-').Length > 0)
                        {
                            soPoNumber = shipment.Data.ShipmentScheduleNo.Split('-')[4];
                        }
                        if (shipment.Data.DayShipment.Split(',').Length > 0)
                        {
                            actualShipmentDate = shipment.Data.DayShipment.Split(',')[1];
                        }
                        if (shipment.Data.ShipmentTime.Split(' ').Length > 0)
                        {
                            actualShipmentTime = shipment.Data.ShipmentTime.Split(' ')[0];
                        }
                        if (!string.IsNullOrEmpty(actualShipmentDate))
                        {
                            actualShipmentDate = actualShipmentDate + " " + actualShipmentTime;
                        }

                        #region Step 1: Check if We have Business Area master data
                        int businessAreaId;
                        string businessAreaCode = string.Empty;

                        // Source partner: partner with initial "AHM"
                        var sourceDetails = (from partner in context.Partners
                                             join partnerType in context.PartnerPartnerTypes on partner.ID equals partnerType.PartnerId
                                             where partner.PartnerInitial == "AHM" && partnerType.PartnerTypeId == context.PartnerTypes.Where(p => p.PartnerTypeCode == "2").Select(p => p.ID).FirstOrDefault()
                                             select new Domain.Partner
                                             {
                                                 PartnerNo = partner.PartnerNo,
                                                 PartnerName = partner.PartnerName,
                                             }
                                           ).FirstOrDefault();
                        // Destination Partner: Partner number with initial as maindealer code
                        var destinationDetails = (from partner in context.Partners
                                                  join partnerType in context.PartnerPartnerTypes on partner.ID equals partnerType.PartnerId
                                                  where partner.PartnerInitial == shipment.Data.MainDealerCode &&
                                                  partnerType.PartnerTypeId == context.PartnerTypes.Where(p => p.PartnerTypeCode == "3").Select(p => p.ID).FirstOrDefault()
                                                  select new Domain.Partner
                                                  {
                                                      PartnerNo = partner.PartnerNo,
                                                      PartnerName = partner.PartnerName,
                                                      ID = partner.ID
                                                  }
                                           ).FirstOrDefault();

                        // Transporter : DestinationPartnerID to TransporterID mapping in MDTransporterMapping table
                        var transporterDetails = (from partner in context.Partners
                                                  join partnerType in context.PartnerPartnerTypes on partner.ID equals partnerType.PartnerId
                                                  join mdTransporter in context.MDTransporterMappings on partner.ID equals mdTransporter.TransporterID
                                                  where mdTransporter.DestinationPartnerID == destinationDetails.ID && partnerType.PartnerTypeId == (context.PartnerTypes.Where(p => p.PartnerTypeCode == "1").Select(p => p.ID).FirstOrDefault())
                                                  //&& mdTransporter.Priority == context.MDTransporterMappings.Where(x => x.DestinationPartnerID == destinationDetails.ID).Min(p => p.Priority)
                                                  select new Domain.Partner
                                                  {
                                                      PartnerNo = partner.PartnerNo,
                                                      PartnerName = partner.PartnerName,
                                                      ID = partner.ID
                                                  }
                                          ).FirstOrDefault();


                        // Business Area:
                        var mdBusinessAreaMappings = (from bam in context.MDBusinessAreaMappings
                                                      where bam.MainDealerCode == shipment.Data.MainDealerCode
                                                      select new Domain.BusinessArea()
                                                      {
                                                          BusinessAreaCode = bam.BusinessAreaCode
                                                      }).FirstOrDefault();

                        if (mdBusinessAreaMappings != null)
                        {
                            var businessArea = (from ba in context.BusinessAreas
                                                where ba.BusinessAreaCode == mdBusinessAreaMappings.BusinessAreaCode
                                                select new Domain.BusinessArea()
                                                {
                                                    BusinessAreaCode = ba.BusinessAreaCode,
                                                    ID = ba.ID
                                                }).FirstOrDefault();
                            if (businessArea != null)
                            {
                                businessAreaCode = businessArea.BusinessAreaCode;
                                businessAreaId = businessArea.ID;
                            }
                            else
                            {
                                orderResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                                orderResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                                orderResponse.StatusMessage = mdBusinessAreaMappings.BusinessAreaCode + " Business Area Code not found in TMS.";
                                return orderResponse;
                            }

                        }
                        else
                        {
                            //Return with Business Area not found
                            orderResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                            orderResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                            orderResponse.StatusMessage = shipment.Data.MainDealerCode + " Main Dealer Code found in TMS.";
                            return orderResponse;
                        }
                        #endregion

                        Domain.Order order = new Domain.Order();
                        order.ActualShipment = Convert.ToDateTime(actualShipmentDate);
                        order.SOPONumber = soPoNumber;
                        order.EstimationShipment = Convert.ToDateTime(actualShipmentDate);
                        order.FleetType = 1;
                        order.PartnerType1 = 1;
                        order.PartnerNo1 = transporterDetails.PartnerNo;
                        order.PartnerName1 = transporterDetails.PartnerName;
                        order.PartnerType2 = 2;
                        order.PartnerNo2 = sourceDetails.PartnerNo;
                        order.PartnerName2 = sourceDetails.PartnerName;
                        order.PartnerType3 = 3;
                        order.PartnerNo3 = destinationDetails.PartnerNo;
                        order.PartnerName3 = destinationDetails.PartnerName;
                        order.BusinessArea = businessAreaCode;
                        order.BusinessAreaId = businessAreaId;
                        order.TotalPallet = shipment.Data.EstimatedTotalPallet.Split(' ')[0] == "" ? 0 : Convert.ToInt32(shipment.Data.EstimatedTotalPallet.Split(' ')[0]);
                        order.Dimension = shipment.Data.EstimatedTotalPallet.Split(' ')[1] == "" ? "" : shipment.Data.EstimatedTotalPallet.Split(' ')[1];
                        order.SequenceNo = 10;
                        order.ShipmentScheduleImageGUID = shipment.ImageGUID;
                        order.OrderWeight = 100;
                        order.OrderWeightUM = "KG";
                        order.OrderType = 1;
                        orderResponse.Data.Add(order);
                        orderResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, ex);
                        orderResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        orderResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                        orderResponse.StatusMessage = ex.Message;
                    }
                }
            }
            return orderResponse;
        }

        public OrderResponse CreateOrdersFromShipmentListOCR(OrderRequest request)
        {
            OrderResponse response = new OrderResponse()
            {
                Data = new List<Domain.Order>()
            };

            using (var context = new Data.TMSDBContext())
            {
                foreach (var order in request.Requests)
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            string soPoNumber = String.Empty;
                            DateTime estimationShipmentDate = order.ActualShipment;
                            DateTime actualShipmentDate = order.EstimationShipment; 

                            #region Step 1: Check if We have Business Area master data
                            int businessAreaId;
                            string businessAreaCode = string.Empty;

                            var businessArea = (from ba in context.BusinessAreas
                                                where ba.BusinessAreaCode == order.BusinessArea
                                                select new Domain.BusinessArea()
                                                {
                                                    ID = ba.ID,
                                                    BusinessAreaCode = ba.BusinessAreaCode
                                                }).FirstOrDefault();
                            if (businessArea != null)
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
                            order.OrderShipmentStatus = 1;
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

                            #region Step 4: Check if Order already existing then update/create accordingly

                            var persistedOrderDataID = (from o in context.OrderHeaders
                                                        where o.OrderNo == order.OrderNo
                                                        select o.ID
                                                      ).FirstOrDefault();

                            int orderDetailId = 0;
                            if (persistedOrderDataID > 0)
                            {
                                #region Update Order
                                var updatedOrderHeader = context.OrderHeaders.Find(persistedOrderDataID);
                                updatedOrderHeader.BusinessAreaId = businessAreaId;
                                updatedOrderHeader.OrderType = order.OrderType;
                                updatedOrderHeader.FleetTypeID = order.FleetType;
                                updatedOrderHeader.VehicleShipment = order.VehicleShipmentType;
                                updatedOrderHeader.DriverNo = order.DriverNo;
                                updatedOrderHeader.DriverName = order.DriverName;
                                updatedOrderHeader.VehicleNo = order.VehicleNo;
                                updatedOrderHeader.OrderWeight = order.OrderWeight;
                                updatedOrderHeader.OrderWeightUM = order.OrderWeightUM;
                                updatedOrderHeader.IsActive = true;
                                updatedOrderHeader.LastModifiedBy = request.LastModifiedBy;
                                updatedOrderHeader.LastModifiedTime = DateTime.Now;
                                updatedOrderHeader.OrderStatusID = orderStatusId;
                                updatedOrderHeader.Harga = order.Harga;
                                Data.ImageGuid existingImageGuidDetails = null;
                                string existingImageGUIDValue = string.Empty;
                                int? shipmentScheduleImageID = null;

                                // check if shipmentScheduleImage is changed
                                if (updatedOrderHeader.ShipmentScheduleImageID > 0)
                                {
                                    existingImageGuidDetails = context.ImageGuids.Where(i => i.ID == updatedOrderHeader.ShipmentScheduleImageID).FirstOrDefault();
                                    existingImageGUIDValue = existingImageGuidDetails.ImageGuIdValue;
                                }
                                if (existingImageGUIDValue != order.ShipmentScheduleImageGUID)
                                {
                                    // Making IsActive false for existed record 
                                    if (updatedOrderHeader.ShipmentScheduleImageID > 0 && existingImageGuidDetails != null)
                                    {
                                        existingImageGuidDetails.IsActive = false;
                                    }

                                    //Inserting new record with IsActive true
                                    Data.ImageGuid imageGuid = new Data.ImageGuid()
                                    {
                                        ImageGuIdValue = order.ShipmentScheduleImageGUID,
                                        IsActive = true,
                                        CreatedBy = order.OrderLastModifiedBy,
                                        CreatedTime = DateTime.Now
                                    };

                                    context.ImageGuids.Add(imageGuid);
                                    context.SaveChanges();
                                    shipmentScheduleImageID = imageGuid.ID;
                                }

                                updatedOrderHeader.ShipmentScheduleImageID = shipmentScheduleImageID;

                                context.Entry(updatedOrderHeader).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                order.ID = updatedOrderHeader.ID;
                                response.StatusMessage = DomainObjects.Resource.ResourceData.OrderUpdated;
                                context.Entry(updatedOrderHeader).State = System.Data.Entity.EntityState.Detached;

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
                                        CreatedBy = request.CreatedBy,
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null,
                                        EstimationShipmentDate = estimationShipmentDate,
                                        ActualShipmentDate = actualShipmentDate
                                    };
                                    context.OrderDetails.Add(orderDetail);
                                    context.SaveChanges();
                                    orderDetailId = orderDetail.ID;
                                    #endregion
                                    #region  Update Order Status
                                    string orderStatusCode = order.OrderShipmentStatus.ToString();
                                    Data.OrderStatusHistory orderStatusHistory = new Data.OrderStatusHistory()
                                    {
                                        OrderDetailID = orderDetailId,
                                        OrderStatusID = context.OrderStatuses.Where(t => t.OrderStatusCode == orderStatusCode).FirstOrDefault().ID,
                                        StatusDate = DateTime.Now,
                                        Remarks = "Order Creted"
                                    };

                                    context.OrderStatusHistories.Add(orderStatusHistory);
                                    context.SaveChanges();

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
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                if (request.UploadType == 2)
                                {
                                    partner1Id = Convert.ToInt32(order.PartnerNo1);
                                    partner2Id = Convert.ToInt32(order.PartnerNo2);
                                    partner3Id = Convert.ToInt32(order.PartnerNo3);
                                }
                                else
                                {
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

                                        response.StatusMessage = order.PartnerNo1 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner1Id = partner1.ID;

                                    }
                                    if (partner2 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo2 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner2Id = partner2.ID;
                                    }
                                    if (partner3 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo3 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner3Id = partner3.ID;
                                    }
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
                                int? shipmentScheduleImageID = null;

                                if (!String.IsNullOrEmpty(order.ShipmentScheduleImageGUID))
                                {
                                    //Inserting new record with IsActive true
                                    Data.ImageGuid imageGuid = new Data.ImageGuid()
                                    {
                                        ImageGuIdValue = order.ShipmentScheduleImageGUID,
                                        IsActive = true,
                                        CreatedBy = order.OrderLastModifiedBy,
                                        CreatedTime = DateTime.Now
                                    };

                                    context.ImageGuids.Add(imageGuid);
                                    context.SaveChanges();
                                    shipmentScheduleImageID = imageGuid.ID;
                                }

                                Data.OrderHeader orderHeader = new Data.OrderHeader()
                                {
                                    LegecyOrderNo = order.OrderNo,
                                    OrderNo = order.OrderNo,
                                    OrderType = order.OrderType,
                                    FleetTypeID = order.FleetType,
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
                                    Harga = order.Harga,
                                    ShipmentScheduleImageID = shipmentScheduleImageID,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null,
                                    SOPONumber = order.SOPONumber,
                                    UploadType = request.UploadType,
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
                                orderDetailId = orderDetail.ID;
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
                                    response.StatusMessage = order.PartnerType1.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType2 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType2.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }
                                if (partnerType3 == null)
                                {
                                    //Return with Partner Type not found.
                                    response.StatusMessage = order.PartnerType3.ToString() + " Partner Type not found in TMS.";
                                    return response;
                                }

                                #endregion

                                int partner1Id;
                                int partner2Id;
                                int partner3Id;

                                #region Check if Partner Exists or not
                                if (request.UploadType == 2)
                                {
                                    partner1Id = Convert.ToInt32(order.PartnerNo1);
                                    partner2Id = Convert.ToInt32(order.PartnerNo2);
                                    partner3Id = Convert.ToInt32(order.PartnerNo3);
                                }
                                else
                                {
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

                                        response.StatusMessage = order.PartnerNo1 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner1Id = partner1.ID;

                                    }
                                    if (partner2 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo2 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner2Id = partner2.ID;
                                    }
                                    if (partner3 == null)
                                    {
                                        //Return with Partner not found.
                                        response.StatusMessage = order.PartnerNo3 + " Partner not found in TMS.";
                                        return response;
                                    }
                                    else
                                    {
                                        partner3Id = partner3.ID;
                                    }
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

                                #endregion

                                #region Step 8: Update Order Status
                                string orderStatusCode = order.OrderShipmentStatus.ToString();
                                Data.OrderStatusHistory orderStatusHistory = new Data.OrderStatusHistory()
                                {
                                    OrderDetailID = orderDetailId,
                                    OrderStatusID = context.OrderStatuses.Where(t => t.OrderStatusCode == orderStatusCode).FirstOrDefault().ID,
                                    StatusDate = DateTime.Now,
                                    Remarks = "Order Creted"
                                };

                                context.OrderStatusHistories.Add(orderStatusHistory);
                                context.SaveChanges();

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

        public InvoiceResponse GetInvoiceRequest(OrderStatusRequest request)

        {
            InvoiceResponse invoiceResponse = new InvoiceResponse()
            {
                Data = new List<Domain.Invoice>()
            };

            using (var context = new Data.TMSDBContext())
            {
                foreach (var reqData in request.Requests)
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var orderHeader = (from oh in context.OrderHeaders
                                               where oh.OrderNo == reqData.OrderNumber
                                               select oh).FirstOrDefault();

                            var orderDetails = (from od in context.OrderDetails
                                                where od.OrderHeaderID == orderHeader.ID
                                                select od).FirstOrDefault();

                            Domain.Invoice invoice = new Domain.Invoice
                            {
                                GeneralPOHeader = new GeneralPOHeader
                                {
                                    GeneralPOHeaderId = "",
                                    DepartementId = "",
                                    OrderDate = orderHeader.OrderDate.ToShortDateString(),
                                    OrderNo = orderHeader.OrderNo,
                                    LocationId = "",
                                    VendorCode = "",
                                    ReviewerDepartementId = "",
                                    TotalPrice = orderHeader.Harga.ToString(),
                                    Currency = "",
                                    Reference = "",
                                    Status = context.OrderStatuses.Where(s => s.OrderStatusCode == reqData.OrderStatusCode).Select(x => x.OrderStatusValue).FirstOrDefault(),
                                    StatusSAP = "",
                                    IsDeleted = "",
                                    CreatedDate = DateTime.Now.ToShortDateString(),
                                    CreatedBy = "SYSTEM",
                                    ModifiedDate = "",
                                    ModifiedBy = "",
                                    Note = "",
                                    IsFromSAP = "",
                                    BusinessArea = context.BusinessAreas.Where(ba => ba.ID == orderHeader.BusinessAreaId).Select(x => x.BusinessAreaCode).FirstOrDefault(),
                                    CompanyCode = "",
                                    GRNumber = "",
                                    GRDate = "",
                                    GRTime = ""

                                },
                                GeneralPODetails = new GeneralPODetails()
                                {
                                    GRNumber = "",
                                    MaterialNumber = orderDetails.ShippingListNo,
                                    GeneralPODetailId = "",
                                    GeneralPOHeaderId = "",
                                    OrderDescription = "",
                                    Qty = orderDetails.TotalCollie.ToString(),
                                    DeliveryDate = orderDetails.ActualShipmentDate.ToShortDateString(),
                                    UnitPrice = "",
                                    PPN = "",
                                    TotalPrice = orderHeader.Harga.ToString(),
                                    Jenis = "",
                                    DepartementId = "",
                                    Currency = "",
                                    ItemNo = "",
                                    IsDeleted = "",
                                    CreatedDate = "",
                                    CreatedBy = "",
                                    ModifiedDate = "",
                                    ModifiedBy = "",
                                    MaterialDesc = ""
                                }

                            };

                            invoiceResponse.Data.Add(invoice);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.Log(LogLevel.Error, ex);
                            invoiceResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                            invoiceResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                            invoiceResponse.StatusMessage = ex.Message;
                        }
                    }
                }
            }
            return invoiceResponse;
        }

        public OrderStatusResponse SwapeStopPoints(OrderStatusRequest orderStatusRequest)
        {
            _logger.Log(LogLevel.Error, orderStatusRequest);
            OrderStatusResponse response = new OrderStatusResponse()
            {
                Data = new List<OrderStatus>()
            };

            using (var context = new DataModel.TMSDBContext())
            {
                try
                {
                    foreach (var statusRequest in orderStatusRequest.Requests)
                    {
                        int orderId = 0;
                        orderId = context.OrderHeaders.FirstOrDefault(t => t.OrderNo == statusRequest.OrderNumber).ID;

                        if (statusRequest.SequenceNumber > 0)
                        {
                            DataModels.OrderDetail orderDetailData = context.OrderDetails.Where(t => t.SequenceNo == statusRequest.SequenceNumber && t.OrderHeaderID == orderId).FirstOrDefault();

                            if (orderDetailData.SequenceNo != statusRequest.NewSequenceNumber && orderDetailData.SequenceNo > 0)
                            {
                                int originalSequenceNo = orderDetailData.SequenceNo;

                                orderDetailData.SequenceNo = statusRequest.NewSequenceNumber;
                                context.Entry(orderDetailData).State = System.Data.Entity.EntityState.Modified;

                                DataModels.OrderDetail swappingDetailData = context.OrderDetails.Where(t => t.OrderHeaderID == orderDetailData.OrderHeaderID && t.SequenceNo == statusRequest.NewSequenceNumber).FirstOrDefault();
                                swappingDetailData.SequenceNo = originalSequenceNo;
                                context.Entry(swappingDetailData).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }


                        }

                        response.Data = orderStatusRequest.Requests;
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

    }
}
