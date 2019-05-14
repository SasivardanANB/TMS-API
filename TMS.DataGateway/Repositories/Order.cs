﻿using AutoMapper;
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

                            DateTime estimationShipmentDate = DateTime.ParseExact(order.EstimationShipmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.EstimationShipmentTime);
                            DateTime actualShipmentDate = DateTime.ParseExact(order.ActualShipmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) + TimeSpan.Parse(order.ActualShipmentTime);

                            #region Step 1: Business Area

                            int businessAreaId = 0;
                            if (request.UploadType == 1) // Upload via Excel
                            {
                                // Check if We have Business Area in Master data
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
                                }
                            }
                            else if (request.UploadType == 2) // Upload via UI
                            {
                                businessAreaId = order.BusinessAreaId;
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
                                        orderStatusValue = "Accepted";
                                        break;
                                    case 4:
                                        orderStatusValue = "Rejected";
                                        break;
                                    case 5:
                                        orderStatusValue = "FinishLaoding";
                                        break;
                                    case 6:
                                        orderStatusValue = "Finish Unloading";
                                        break;
                                    case 7:
                                        orderStatusValue = "Completed";
                                        break;
                                    case 8:
                                        orderStatusValue = "Cancelled";
                                        break;
                                    case 9:
                                        orderStatusValue = "Partial Billed";
                                        break;
                                    case 10:
                                        orderStatusValue = "FinalBilled";
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
                                    OrderNo = GetOrderNumber(businessAreaId, order.BusinessArea, "TMS", DateTime.Now.Year),
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
                            }

                            #endregion

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

                            #region Step 4: Check if Partners exists or not

                            int partner1Id;
                            int partner2Id;
                            int partner3Id;
                            var partner1 = (from p in context.Partners
                                            where p.PartnerNo == order.PartnerNo1
                                            select new Domain.Partner()
                                            {
                                                ID = p.ID
                                            }).FirstOrDefault();
                            if (partner1 != null)
                                partner1Id = partner1.ID;
                            else
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

                            var partner2 = (from p in context.Partners
                                            where p.PartnerNo == order.PartnerNo2
                                            select new Domain.Partner()
                                            {
                                                ID = p.ID
                                            }).FirstOrDefault();
                            if (partner2 != null)
                                partner2Id = partner2.ID;
                            else
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

                            var partner3 = (from p in context.Partners
                                            where p.PartnerNo == order.PartnerNo3
                                            select new Domain.Partner()
                                            {
                                                ID = p.ID
                                            }).FirstOrDefault();
                            if (partner3 != null)
                                partner3Id = partner3.ID;
                            else
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
                                OrderDetailID = orderDetail.ID,
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
                                OrderDetailID = orderDetail.ID,
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
                                OrderDetailID = orderDetail.ID,
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
            OrderSearchResponse orderSearchResponse = new OrderSearchResponse();
            List<Domain.OrderSearch> orderList = new List<Domain.OrderSearch>();
            try
            {
                using (var context =  new Data.TMSDBContext())
                {
                    orderList =
                        (from order in context.OrderHeaders
                         join orderStatus in context.OrderStatuses on order.OrderStatusID equals orderStatus.ID
                             // where !order.IsActive
                         select new Domain.OrderSearch
                         {
                             OrderId=order.ID,
                             OrderNumber = order.LegecyOrderNo,
                             OrderStatus = orderStatus.OrderStatusValue,
                             VehicleType = order.VehicleShipment,
                             PoliceNumber=order.VehicleNo
                         }).ToList();
                }
                // Filter
                if (orderList.Count > 0)
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

                }

                // GLobal Search Filter
                //if (!string.IsNullOrEmpty(picRequest.GlobalSearch))
                //{
                //    string globalSearch = picRequest.GlobalSearch;
                //    picList = picList.Where(s => !s.IsDeleted && s.PICName.Contains(globalSearch)
                //    || s.PICPhone.Contains(globalSearch)
                //    || s.PICEmail.ToString().Contains(globalSearch)
                //    ).ToList();
                //}

                // Sorting
                if (orderList.Count > 0 && !string.IsNullOrEmpty(orderSearchRequest.SortOrder))
                {
                    switch (orderSearchRequest.SortOrder.ToLower())
                    {
                        case "ordernumber":
                            orderList = orderList.OrderBy(o=>o.OrderNumber).ToList();
                            break;
                        case "ordernumber_desc":
                            orderList = orderList.OrderByDescending(o => o.OrderNumber).ToList();
                            break;
                        case "source":
                            orderList = orderList.OrderBy(o=>o.Source).ToList();
                            break;
                        case "source_desc":
                            orderList = orderList.OrderByDescending(o=>o.Source).ToList();
                            break;
                        case "destination":
                            orderList = orderList.OrderBy(o=>o.Destination).ToList();
                            break;
                        case "destination_desc":
                            orderList = orderList.OrderByDescending(o=>o.Destination).ToList();
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
                            orderList = orderList.OrderByDescending(o=>o.OrderId).ToList();
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
                    orderSearchResponse.Data = orderList;
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
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                orderSearchResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                orderSearchResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                orderSearchResponse.StatusMessage = ex.Message;
            }
            return orderSearchResponse;
        }

        private string GetOrderNumber(int businessAreaId, string businessArea, string applicationCode, int year)
        {
            string orderNo = businessArea + applicationCode;
            using (var context = new Data.TMSDBContext())
            {
                var order = context.OrderHeaders.Where(t => t.BusinessAreaId == businessAreaId).OrderByDescending(t => t.OrderDate).FirstOrDefault();
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
