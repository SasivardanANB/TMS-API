using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                            DateTime estimationShipmentDate = DateTime.ParseExact(order.EstimationShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.EstimationShipmentTime);
                            DateTime actualShipmentDate = DateTime.ParseExact(order.ActualShipmentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.ActualShipmentTime);

                            #region Step 1: Business Area

                            int businessAreaId = 0;
                            string businessAreaCode = "";
                            if (request.UploadType == 1) // Upload via Excel
                            {
                                // Check if We have Business Area in Master data
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
                                    Data.BusinessArea businessAreaRequest = new Data.BusinessArea()
                                    {
                                        BusinessAreaCode = order.BusinessArea,
                                        BusinessAreaDescription = order.BusinessArea,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now
                                    };

                                    context.BusinessAreas.Add(businessAreaRequest);
                                    context.SaveChanges();
                                    businessAreaId = businessAreaRequest.ID;
                                    businessAreaCode = businessAreaRequest.BusinessAreaCode;
                                }
                            }
                            else if (request.UploadType == 2) // Upload via UI
                            {
                                var businessArea = (from ba in context.BusinessAreas
                                                    where ba.ID == order.BusinessAreaId
                                                    select new Domain.BusinessArea()
                                                    {
                                                        ID = ba.ID,
                                                        BusinessAreaCode = ba.BusinessAreaCode
                                                    }).FirstOrDefault();

                                businessAreaId = businessArea.ID;
                                businessAreaCode = businessArea.BusinessAreaCode;
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
                                switch (order.OrderShipmentStatus)
                                {
                                    case 1:
                                        orderStatusValue = "Booked";
                                        break;
                                    case 2:
                                        orderStatusValue = "Confirmed";
                                        break;
                                    case 3:
                                        orderStatusValue = "Assigned";
                                        break;
                                    case 4:
                                        orderStatusValue = "Start Trip";
                                        break;
                                    case 5:
                                        orderStatusValue = "Confirm Arrived";
                                        break;
                                    case 6:
                                        orderStatusValue = "Start Load";
                                        break;
                                    case 7:
                                        orderStatusValue = "Finish Load";
                                        break;
                                    case 8:
                                        orderStatusValue = "Confirm Pickup";
                                        break;
                                    case 9:
                                        orderStatusValue = "Start Unload";
                                        break;
                                    case 10:
                                        orderStatusValue = "Finish Unload";
                                        break;
                                    case 11:
                                        orderStatusValue = "POD";
                                        break;
                                    case 12:
                                        orderStatusValue = "Complete";
                                        break;
                                    case 13:
                                        orderStatusValue = "Cancel";
                                        break;
                                    case 14:
                                        orderStatusValue = "Billed";
                                        break;
                                    case 15:
                                        orderStatusValue = "Driver Accepted";
                                        break;
                                    case 16:
                                        orderStatusValue = "Driver Rejected";
                                        break;
                                    default:
                                        break;
                                }
                                Data.OrderStatus orderStatusRequest = new Data.OrderStatus()
                                {
                                    OrderStatusCode = order.OrderShipmentStatus.ToString(),
                                    OrderStatusValue = orderStatusValue,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now
                                };

                                context.OrderStatuses.Add(orderStatusRequest);
                                context.SaveChanges();
                                orderStatusId = orderStatusRequest.ID;
                            }

                            #endregion

                            #region Step 2: Check if Order already existing then update/create accordingly

                            var persistedOrderDataID = (from o in context.OrderHeaders
                                                        where o.LegecyOrderNo == order.OrderNo
                                                        select o.ID
                                                      ).FirstOrDefault();
                            if (request.UploadType == 2)
                            {
                                int driverId = Convert.ToInt32(order.DriverNo);
                                order.DriverNo = context.Drivers.FirstOrDefault(t => t.ID == driverId).DriverNo;
                                order.DriverName = context.Drivers.FirstOrDefault(t => t.ID == driverId).UserName;
                            }

                            int orderDetailId= 0;
                            if (persistedOrderDataID > 0) // Update Order
                            {
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
                                updatedOrderHeader.EstimationShipmentDate = estimationShipmentDate;
                                updatedOrderHeader.ActualShipmentDate = actualShipmentDate;
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

                                #region TODO: Check if order detail is existing or not else create it
                                //TODO:
                                #endregion
                            }
                            else // Create New Order Header
                            {
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
                                    OrderNo = GetOrderNumber(businessAreaId, businessAreaCode, "TMS", DateTime.Now.Year),
                                    OrderType = order.OrderType,
                                    FleetTypeID = order.FleetType,
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
                                    Harga = order.Harga,
                                    ShipmentScheduleImageID = shipmentScheduleImageID,
                                    CreatedBy = request.CreatedBy,
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null,
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
                                orderDetailId = orderDetail.ID;
                                #endregion

                                #region Step 4: Check if Partners exists or not

                                int partner1Id = 0;
                                int partner2Id = 0;
                                int partner3Id = 0;
                                if (request.UploadType == 1)
                                {
                                    var partner1 = (from p in context.Partners
                                                    where p.PartnerNo == order.PartnerNo1
                                                    select new Domain.Partner()
                                                    {
                                                        ID = p.ID
                                                    }).FirstOrDefault();
                                    if (partner1 != null)
                                        partner1Id = partner1.ID;
                                }
                                else if (request.UploadType == 2)
                                {
                                    partner1Id = Convert.ToInt32(order.PartnerNo1);
                                }

                                if (partner1Id == 0)
                                {
                                    Data.Partner partner1Request = new Data.Partner()
                                    {
                                        PartnerNo = order.PartnerNo1,
                                        PartnerName = order.PartnerName1,
                                        PartnerTypeID = context.PartnerTypes.Where(t => t.PartnerTypeCode == order.PartnerType1.ToString()).FirstOrDefault().ID,
                                        IsActive = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.Partners.Add(partner1Request);
                                    context.SaveChanges();
                                    partner1Id = partner1Request.ID;
                                }

                                if (request.UploadType == 1)
                                {
                                    var partner2 = (from p in context.Partners
                                                    where p.PartnerNo == order.PartnerNo2
                                                    select new Domain.Partner()
                                                    {
                                                        ID = p.ID
                                                    }).FirstOrDefault();
                                    if (partner2 != null)
                                        partner2Id = partner2.ID;
                                }
                                else if (request.UploadType == 2)
                                {
                                    partner2Id = Convert.ToInt32(order.PartnerNo2);
                                }

                                if (partner2Id == 0)
                                {
                                    Data.Partner partner2Request = new Data.Partner()
                                    {
                                        PartnerNo = order.PartnerNo2,
                                        PartnerName = order.PartnerName2,
                                        PartnerTypeID = context.PartnerTypes.Where(t => t.PartnerTypeCode == order.PartnerType2.ToString()).FirstOrDefault().ID,
                                        IsActive = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.Partners.Add(partner2Request);
                                    context.SaveChanges();
                                    partner2Id = partner2Request.ID;
                                }

                                if (request.UploadType == 1)
                                {
                                    var partner3 = (from p in context.Partners
                                                    where p.PartnerNo == order.PartnerNo3
                                                    select new Domain.Partner()
                                                    {
                                                        ID = p.ID
                                                    }).FirstOrDefault();
                                    if (partner3 != null)
                                        partner3Id = partner3.ID;
                                }
                                else if (request.UploadType == 2)
                                {
                                    partner3Id = Convert.ToInt32(order.PartnerNo3);
                                }

                                if (partner3Id == 0)
                                {
                                    Data.Partner partner3Request = new Data.Partner()
                                    {
                                        PartnerNo = order.PartnerNo3,
                                        PartnerName = order.PartnerName3,
                                        PartnerTypeID = context.PartnerTypes.Where(t => t.PartnerTypeCode == order.PartnerType3.ToString()).FirstOrDefault().ID,
                                        IsActive = true,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now,
                                        LastModifiedBy = "",
                                        LastModifiedTime = null
                                    };
                                    context.Partners.Add(partner3Request);
                                    context.SaveChanges();
                                    partner3Id = partner3Request.ID;
                                }

                                #endregion

                                #region Step 5: Insert Order Partner Detail

                                Data.OrderPartnerDetail orderPartner1Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetailId,
                                    PartnerID = partner1Id,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner1Detail);
                                context.SaveChanges();

                                Data.OrderPartnerDetail orderPartner2Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetailId,
                                    PartnerID = partner2Id,
                                    IsOriginal = true,
                                    IsParent = true,
                                    CreatedBy = "SYSTEM",
                                    CreatedTime = DateTime.Now,
                                    LastModifiedBy = "",
                                    LastModifiedTime = null
                                };
                                context.OrderPartnerDetails.Add(orderPartner2Detail);
                                context.SaveChanges();

                                Data.OrderPartnerDetail orderPartner3Detail = new Data.OrderPartnerDetail()
                                {
                                    OrderDetailID = orderDetailId,
                                    PartnerID = partner3Id,
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
                                            // OrderDetailID = orderDetail.ID,
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
                    orderList = (from oh in context.OrderHeaders
                                 join od in context.OrderDetails on oh.ID equals od.OrderHeaderID
                                 join ps in context.PackingSheets on od.ShippingListNo equals ps.ShippingListNo into pks
                                 from pksh in pks.DefaultIfEmpty()
                                 where ((!String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.OrderNo == orderSearchRequest.GlobalSearch)
                                        || (String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.OrderNo == oh.OrderNo))
                                        ||
                                        ((!String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.VehicleNo == orderSearchRequest.GlobalSearch)
                                        || (String.IsNullOrEmpty(orderSearchRequest.GlobalSearch) && oh.VehicleNo == oh.VehicleNo))
                                 ||
                                 ((orderSearchRequest.GlobalSearch != string.Empty && pksh.PackingSheetNo == orderSearchRequest.GlobalSearch)
                                 || (orderSearchRequest.GlobalSearch == string.Empty && pksh.PackingSheetNo == pksh.PackingSheetNo))

                                 select new Domain.OrderSearch
                                 {
                                     OrderId = oh.ID,
                                     OrderType = oh.OrderType,
                                     OrderNumber = oh.OrderNo,
                                     VehicleType = context.VehicleTypes.Where(v => v.ID.ToString() == oh.VehicleShipment).Select(vt => vt.VehicleTypeDescription).FirstOrDefault(),
                                     PoliceNumber = oh.VehicleNo,
                                     OrderStatus = context.OrderStatuses.Where(t => t.ID == oh.OrderStatusID).FirstOrDefault().OrderStatusValue
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

                    //if (orderList != null && orderList.Count > 0)
                    //{
                    //    foreach (var order in orderList)
                    //    {
                    //        //Get Packing Sheet No
                    //        var packingSheets = (from ps in context.PackingSheets
                    //                             join od in context.OrderDetails on ps.ShippingListNo equals od.ShippingListNo
                    //                             join oh in context.OrderHeaders on od.OrderHeaderID equals oh.ID
                    //                             where od.OrderHeaderID == order.OrderId
                    //                             select new
                    //                             {
                    //                                 packingSheetNo = ps.PackingSheetNo
                    //                             }).ToList();
                    //        if (packingSheets != null && packingSheets.Count > 0)
                    //        {
                    //            foreach (var packingSheet in packingSheets)
                    //            {
                    //                if (string.IsNullOrEmpty(order.PackingSheetNumber))
                    //                    order.PackingSheetNumber = packingSheet.packingSheetNo;
                    //                else
                    //                    order.PackingSheetNumber += ", " + packingSheet.packingSheetNo;
                    //            }
                    //        }

                    //        var orderData = (from od in context.OrderDetails
                    //                         where od.OrderHeaderID == order.OrderId
                    //                         group od by new { od.ID, od.SequenceNo } into gp
                    //                         select new
                    //                         {
                    //                             OrderDetailId = gp.Key.ID,
                    //                             SequenceNo = gp.Max(t => t.SequenceNo),
                    //                         }).FirstOrDefault();
                    //        if (orderData != null)
                    //        {
                    //            var partnerData = (from op in context.OrderPartnerDetails
                    //                               where op.OrderDetailID == orderData.OrderDetailId
                    //                               select new
                    //                               {
                    //                                   PrtnerID = op.PartnerID,
                    //                                   PartnerName = context.Partners.Where(t => t.ID == op.PartnerID).FirstOrDefault().PartnerName,
                    //                                   partnerTypeID = context.Partners.Where(t => t.ID == op.PartnerID).FirstOrDefault().PartnerTypeID
                    //                               }).ToList();

                    //            if (partnerData != null && partnerData.Count > 0)
                    //            {
                    //                var partners = (from pd in partnerData
                    //                                join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                    //                                select new
                    //                                {
                    //                                    PrtnerID = pd.PrtnerID,
                    //                                    PartnerName = pd.PartnerName,
                    //                                    partnerTypeID = pd.partnerTypeID,
                    //                                    PartnerTypeCode = pt.PartnerTypeCode
                    //                                }).ToList();
                    //                if (partners != null && partners.Count > 0)
                    //                {
                    //                    foreach (var partner in partners)
                    //                    {
                    //                        if (partner.PartnerTypeCode == "1")
                    //                            order.ExpeditionName = partner.PartnerName;
                    //                        if (partner.PartnerTypeCode == "2")
                    //                            order.Source = partner.PartnerName;
                    //                        if (partner.PartnerTypeCode == "3")
                    //                            order.Destination = partner.PartnerName;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        //Add to List
                    //        orderSearchResponse.Data.Add(order);
                    //    }
                    //}

                    // Filter
                    if (orderList != null && orderList.Count > 0 && orderSearchRequest.Requests.Count > 0)
                    {
                        var orderFilter = orderSearchRequest.Requests[0];

                        if (!string.IsNullOrEmpty(orderFilter.OrderNumber))
                        {
                            orderList = orderList.Where(o => o.OrderNumber.Contains(orderFilter.OrderNumber)).ToList();
                        }

                        if (!String.IsNullOrEmpty(orderFilter.PackingSheetNumber))
                        {
                            orderList = orderList.Where(o => o.PackingSheetNumber.Contains(orderFilter.PackingSheetNumber)).ToList();
                        }

                        if (!String.IsNullOrEmpty(orderFilter.PoliceNumber))
                        {
                            orderList = orderList.Where(o => o.PoliceNumber.Contains(orderFilter.PoliceNumber)).ToList();
                        }
                        if (orderFilter.OrderType != 0)
                        {
                            orderList = orderList.Where(o => o.OrderType == orderFilter.OrderType).ToList();
                        }
                    }

                    // Sorting
                    if (orderList.Count > 0 && !string.IsNullOrEmpty(orderSearchRequest.SortOrder))
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

                    // Total NumberOfRecords
                    orderSearchResponse.NumberOfRecords = orderList.Count;

                    // Paging
                    int pageNumber = (orderSearchRequest.PageNumber ?? 1);
                    int pageSize = Convert.ToInt32(orderSearchRequest.PageSize);
                    if (pageSize > 0)
                    {
                        orderList = orderList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }

                    if (orderList.Count > 0)
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
                        StepHeaderDescription = "Driver/Transporter accept order"
                    },
                    Loads = new List<TrackStepLoadUnload>(),
                    Unloads = new List<TrackStepLoadUnload>(),
                    POD = new TrackStep()
                    {
                        StepHeaderName = "POD",
                        StepHeaderDescription = "Driver upload shipment document from AHM"
                    },
                    Complete = new TrackStep()
                    {
                        StepHeaderName = "COMPLETE",
                        StepHeaderDescription = "Driver complete the trip"
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
                            if (orderHeader.OrderType == 1) //For Inbound, There can be multiple loads for every Order Detail
                            {
                                #region Create Load response for In-Bound Orders
                                var orderDetails = (from od in context.OrderDetails
                                                    where od.OrderHeaderID == orderHeader.OrderId
                                                    orderby od.SequenceNo ascending
                                                    select new
                                                    {
                                                        OrderID = od.OrderHeaderID,
                                                        OrderDetailId = od.ID,
                                                    }).ToList();

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
                                                               partnerTypeID = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerTypeID
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
                                                            if (loadStatus.StatusCode == "4")
                                                            {
                                                                loadData.StartTrip.StepHeaderName = "START TRIP";
                                                                loadData.StartTrip.StepHeaderDescription = "On the way to AHM";
                                                                loadData.StartTrip.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                            else if (loadStatus.StatusCode == "5")
                                                            {
                                                                loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                                loadData.ConfirmArrive.StepHeaderDescription = "Arrive at AHM";
                                                                loadData.ConfirmArrive.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                            else if (loadStatus.StatusCode == "6")
                                                            {
                                                                loadData.StartLoad.StepHeaderName = "START LOAD";
                                                                loadData.StartLoad.StepHeaderDescription = "Load parts at AHM";
                                                                loadData.StartLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                            else if (loadStatus.StatusCode == "7")
                                                            {
                                                                loadData.FinishLoad.StepHeaderName = "FINISH LOAD";
                                                                loadData.FinishLoad.StepHeaderDescription = "Finish load parts at AHM";
                                                                loadData.FinishLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
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
                                                        loadData.ConfirmArrive.StepHeaderDescription = "Arrive at AHM";

                                                        loadData.StartLoad.StepHeaderName = "START LOAD";
                                                        loadData.StartLoad.StepHeaderDescription = "Load parts at AHM";

                                                        loadData.FinishLoad.StepHeaderName = "FINISH LOAD";
                                                        loadData.FinishLoad.StepHeaderDescription = "Finish load parts at AHM";

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
                                                           partnerTypeID = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerTypeID
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
                                                    FinishLoad = new TrackStep()
                                                };
                                                foreach (var unLoadStatus in unLoadStatusData)
                                                {
                                                    if (unLoadStatus.StatusCode == "4")
                                                    {
                                                        unLoadData.StartTrip.StepHeaderName = "START TRIP";
                                                        unLoadData.StartTrip.StepHeaderDescription = "On the way to AHM";
                                                        unLoadData.StartTrip.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "5")
                                                    {
                                                        unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                        unLoadData.ConfirmArrive.StepHeaderDescription = "Arrive at AHM";
                                                        unLoadData.ConfirmArrive.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "6")
                                                    {
                                                        unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                        unLoadData.StartLoad.StepHeaderDescription = "Unload parts at AHM";
                                                        unLoadData.StartLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                    }
                                                    else if (unLoadStatus.StatusCode == "7")
                                                    {
                                                        unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                        unLoadData.FinishLoad.StepHeaderDescription = "Finish Unload parts at AHM";
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
                                                unLoadData.StartTrip.StepHeaderDescription = "On the way to AHM";

                                                unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                unLoadData.ConfirmArrive.StepHeaderDescription = "Arrive at AHM";

                                                unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                unLoadData.StartLoad.StepHeaderDescription = "Unload parts at AHM";

                                                unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                unLoadData.FinishLoad.StepHeaderDescription = "Finish unload parts at AHM";
                                                orderTrackResponse.Data.Unloads.Add(unLoadData);
                                            }
                                        }
                                    }
                                    #endregion
                                    else if (orderHeader.OrderType == 2) //For Outbound, There can be multiple unloads for every Order Detail
                                    {
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
                                                                             partnerTypeID = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerTypeID
                                                                         }).ToList();
                                                if (partnerDataUnload != null && partnerDataUnload.Count > 0)
                                                {
                                                    var partners = (from pd in partnerData
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
                                                    if (partners != null && partners.Count > 0)
                                                    {
                                                        foreach (var partner in partners)
                                                        {
                                                            var loadStatusData = statusData.Where(t => t.IsLoad == false).ToList();
                                                            if (loadStatusData != null && loadStatusData.Count > 0)
                                                            {
                                                                TrackStepLoadUnload unLoadData = new TrackStepLoadUnload()
                                                                {
                                                                    TrackLoadUnloadName = "UNLOAD",
                                                                    StepHeaderNotification = String.Format("{0} from {1} Main Dealers", ++sourceNumber, totalSources),
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
                                                                        unLoadData.StartTrip.StepHeaderDescription = "On the way to Main Dealer";
                                                                        unLoadData.StartTrip.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                    }
                                                                    else if (loadStatus.StatusCode == "5")
                                                                    {
                                                                        unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                                        unLoadData.ConfirmArrive.StepHeaderDescription = "Arrive at Main Dealer";
                                                                        unLoadData.ConfirmArrive.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                    }
                                                                    else if (loadStatus.StatusCode == "9")
                                                                    {
                                                                        unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                                        unLoadData.StartLoad.StepHeaderDescription = "Unload parts at Main Dealer";
                                                                        unLoadData.StartLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                    }
                                                                    else if (loadStatus.StatusCode == "10")
                                                                    {
                                                                        unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                                        unLoadData.FinishLoad.StepHeaderDescription = "Finish unload parts at Main Dealer";
                                                                        unLoadData.FinishLoad.StepHeaderDateTime = loadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                                    }
                                                                }
                                                                orderTrackResponse.Data.Loads.Add(unLoadData);
                                                            }
                                                            else
                                                            {
                                                                TrackStepLoadUnload unLoadData = new TrackStepLoadUnload()
                                                                {
                                                                    TrackLoadUnloadName = "UNLOAD",
                                                                    StepHeaderNotification = String.Format("{0} from {1} Main Dealers", sourceNumber++, totalSources),
                                                                    StartTrip = new TrackStep(),
                                                                    ConfirmArrive = new TrackStep(),
                                                                    StartLoad = new TrackStep(),
                                                                    FinishLoad = new TrackStep()
                                                                };

                                                                unLoadData.StartTrip.StepHeaderName = "START TRIP";
                                                                unLoadData.StartTrip.StepHeaderDescription = "On the way to Main Dealer";

                                                                unLoadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                                unLoadData.ConfirmArrive.StepHeaderDescription = "Arrive at Main Dealer";

                                                                unLoadData.StartLoad.StepHeaderName = "START UNLOAD";
                                                                unLoadData.StartLoad.StepHeaderDescription = "UnlLoad parts at Main Dealer";

                                                                unLoadData.FinishLoad.StepHeaderName = "FINISH UNLOAD";
                                                                unLoadData.FinishLoad.StepHeaderDescription = "Finish unload parts at Main Dealer";

                                                                orderTrackResponse.Data.Loads.Add(unLoadData);
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        #endregion
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
                                                                       where p.OrderDetailID == orderDetailUnload.OrderDetailId
                                                                       select new
                                                                       {
                                                                           PrtnerID = p.PartnerID,
                                                                           PartnerName = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerName,
                                                                           partnerTypeID = context.Partners.Where(t => t.ID == p.PartnerID).FirstOrDefault().PartnerTypeID
                                                                       }).ToList();
                                            if (partnerDataOutbound != null && partnerDataOutbound.Count > 0)
                                            {
                                                var partners = (from pd in partnerData
                                                                join pt in context.PartnerTypes on pd.partnerTypeID equals pt.ID
                                                                where pt.PartnerTypeCode == "2"
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
                                                        TrackStepLoadUnload loadData = new TrackStepLoadUnload()
                                                        {
                                                            TrackLoadUnloadName = "LOAD",
                                                            StartTrip = new TrackStep(),
                                                            ConfirmArrive = new TrackStep(),
                                                            StartLoad = new TrackStep(),
                                                            FinishLoad = new TrackStep()
                                                        };
                                                        foreach (var unLoadStatus in unLoadStatusData)
                                                        {
                                                            if (unLoadStatus.StatusCode == "4")
                                                            {
                                                                loadData.StartTrip.StepHeaderName = "START TRIP";
                                                                loadData.StartTrip.StepHeaderDescription = "On the way to AHM";
                                                                loadData.StartTrip.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                            else if (unLoadStatus.StatusCode == "5")
                                                            {
                                                                loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                                loadData.ConfirmArrive.StepHeaderDescription = "Arrive at AHM";
                                                                loadData.ConfirmArrive.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                            else if (unLoadStatus.StatusCode == "6")
                                                            {
                                                                loadData.StartLoad.StepHeaderName = "START LOAD";
                                                                loadData.StartLoad.StepHeaderDescription = "Load parts at AHM";
                                                                loadData.StartLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                            else if (unLoadStatus.StatusCode == "7")
                                                            {
                                                                loadData.FinishLoad.StepHeaderName = "FINISH LOAD";
                                                                loadData.FinishLoad.StepHeaderDescription = "Finish load parts at AHM";
                                                                loadData.FinishLoad.StepHeaderDateTime = unLoadStatus.StatusDate.ToString("dd MMM yyyy HH:mm");
                                                            }
                                                        }
                                                        orderTrackResponse.Data.Unloads.Add(loadData);
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
                                                        loadData.StartTrip.StepHeaderDescription = "On the way to AHM";

                                                        loadData.ConfirmArrive.StepHeaderName = "CONFIRM ARRIVE";
                                                        loadData.ConfirmArrive.StepHeaderDescription = "Arrive at AHM";

                                                        loadData.StartLoad.StepHeaderName = "START LOAD";
                                                        loadData.StartLoad.StepHeaderDescription = "Load parts at AHM";

                                                        loadData.FinishLoad.StepHeaderDescription = "Finish load parts at AHM";
                                                        orderTrackResponse.Data.Unloads.Add(loadData);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #region POD Section
                                foreach (var status in statusData)
                                {
                                    if (status.StatusCode == "11")
                                    {
                                        orderTrackResponse.Data.AcceptOrder.StepHeaderDateTime = status.StatusDate.ToString("dd MMM yyyy HH:mm");
                                    }
                                }
                                #endregion

                                #region Complete Order Section
                                foreach (var status in statusData)
                                {
                                    if (status.StatusCode == "12")
                                    {
                                        orderTrackResponse.Data.AcceptOrder.StepHeaderDateTime = status.StatusDate.ToString("dd MMM yyyy HH:mm");
                                    }
                                }
                                #endregion
                            }
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

        private string GetOrderNumber(int businessAreaId, string businessArea, string applicationCode, int year)
        {
            string orderNo = businessArea + applicationCode;
            using (var context = new Data.TMSDBContext())
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

        public PackingSheetResponse CreateUpdatePackingSheet(PackingSheetRequest packingSheetRequest)
        {
            PackingSheetResponse packingSheetResponse = new PackingSheetResponse();
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    foreach (var packingSheet in packingSheetRequest.Requests)
                    {
                        var orderDetailsData = context.OrderDetails.Where(x => x.ID == packingSheet.OrderDetailId).FirstOrDefault();
                        if (orderDetailsData != null)
                        {
                            orderDetailsData.ShippingListNo = packingSheet.ShippingListNo;
                            orderDetailsData.TotalCollie = packingSheet.Collie;
                            orderDetailsData.Katerangan = packingSheet.Katerangan;
                            // context.SaveChanges();

                            if (packingSheet.PackingSheetNumbers.Count > 0)
                            {
                                foreach (var item in packingSheet.PackingSheetNumbers)
                                {
                                    Data.PackingSheet packingSheetData = new Data.PackingSheet()
                                    {
                                        // OrderDetailID = orderDetail.ID,
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
                        }
                        //packingSheetRequest.Requests = mapper.Map<List<DataModel.Role>, List<Domain.Role>>(roles);
                        //packingSheetResponse.Data = packingSheetRequest.Requests;
                        packingSheetResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        packingSheetResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                        packingSheetResponse.StatusCode = (int)HttpStatusCode.OK;

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

        public CommonResponse GetOrderIds()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                using (var context = new Data.TMSDBContext())
                {
                    var orderData = (from orderHeader in context.OrderHeaders
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
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
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
                                      where orderHeader.ID == orderId && opd.Partner.PartnerTypeID == 1
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
                                      where orderHeader.ID == orderId && opd.Partner.PartnerTypeID == 1
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
                        dealerDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
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
                                         //ActualShipment = oH.ActualShipmentDate,
                                         ActualShipmentDate = oH.ActualShipmentDate.ToString(),
                                         BusinessArea = oH.BusinessArea.BusinessAreaDescription,
                                         BusinessAreaId = oH.BusinessAreaId,
                                         DriverName = oH.DriverName,
                                         DriverNo = oH.DriverNo,
                                         EstimationShipmentDate = oH.EstimationShipmentDate.ToString(),
                                         FleetType = oH.FleetType.ID,
                                         Harga = oH.Harga,
                                         IsActive = oH.IsActive,
                                         LegecyOrderNo = oH.LegecyOrderNo,
                                         VehicleNo = oH.VehicleNo,
                                         VehicleShipmentType = oH.VehicleShipment,
                                         //OrderDate = oH.OrderDate,
                                         OrderNo = oH.OrderNo,
                                         OrderShipmentStatus = oH.OrderStatusID,
                                         OrderType = oH.OrderType,
                                         OrderWeight = oH.OrderWeight,
                                         OrderWeightUM = oH.OrderWeightUM,
                                         ShipmentScheduleImageGUID = context.ImageGuids.FirstOrDefault(t=>t.ID == oH.ShipmentScheduleImageID).ImageGuIdValue,
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
                                                    CityName = orderPartnerDetails.Partner.PostalCode.SubDistrict.City.CityDescription,
                                                    ProvinceName = orderPartnerDetails.Partner.PostalCode.SubDistrict.City.Province.ProvinceDescription,
                                                    SubDistrictName = orderPartnerDetails.Partner.PostalCode.SubDistrict.SubdistrictName,
                                                    ActualShipmentDate = orderData.ActualShipmentDate,
                                                    EstimationShipmentDate = orderData.EstimationShipmentDate,
                                                    PartnerCode = orderPartnerDetails.Partner.PartnerNo,
                                                    PartnerId = orderPartnerDetails.PartnerID,
                                                    PartnerName = orderPartnerDetails.Partner.PartnerName,
                                                    PeartnerType = orderPartnerDetails.Partner.PartnerTypeID,
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
                                                    where data.PeartnerType == 3 // && data.SequenceNo == maxSeqNo
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
                                              where data.PeartnerType == 2 //&& data.SequenceNo == maxSeqNo
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
                        //orderResponse.NumberOfRecords = delearData.Count;
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
                                    join postalcode in context.PostalCodes on partner.PostalCodeID equals postalcode.ID
                                    join subDistrict in context.SubDistricts on postalcode.SubDistrictID equals subDistrict.ID
                                    where partner.ID == partnerId
                                    select new Domain.Partner
                                    {
                                        PartnerAddress = partner.PartnerAddress,
                                        CityCode = subDistrict.City.CityCode,
                                        ProvinceCode = subDistrict.City.Province.ProvinceCode,
                                        PartnerName = partner.PartnerName
                                    }).FirstOrDefault();
                    }
                    else
                    {
                        response = (from partner in context.Partners
                                    join postalcode in context.PostalCodes on partner.PostalCodeID equals postalcode.ID
                                    join subDistrict in context.SubDistricts on postalcode.SubDistrictID equals subDistrict.ID
                                    where partner.PartnerNo == partnerNo
                                    select new Domain.Partner
                                    {
                                        PartnerAddress = partner.PartnerAddress,
                                        CityCode = subDistrict.City.CityCode,
                                        ProvinceCode = subDistrict.City.Province.ProvinceCode
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
                        orderId = context.OrderHeaders.FirstOrDefault(t => t.LegecyOrderNo == statusRequest.OrderNumber).ID;
                        if (statusRequest.SequenceNumber > 0)
                            orderDetailId = context.OrderDetails.FirstOrDefault(t => t.SequenceNo == statusRequest.SequenceNumber && t.OrderHeaderID == orderId).ID;
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

                        response.Data = request.Requests;
                        response.Status = DomainObjects.Resource.ResourceData.Success;
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                }
            }

            return response;
        }
    }
}
