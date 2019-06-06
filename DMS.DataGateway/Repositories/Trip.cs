using AutoMapper;
using NLog;
using DMS.DataGateway.Repositories.Iterfaces;
using DMS.DataGateway.DataModels;
using DMS.DomainObjects.Objects;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataModel = DMS.DataGateway.DataModels;
using Domain = DMS.DomainObjects.Objects;
using System.Configuration;
using System.Data.Entity;

namespace DMS.DataGateway.Repositories
{
    public class Trip : ITrip
    {
        #region Private variables and Methods
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private string GetTripNumber(string businessAreaCode)
        {
            string tripNo = businessAreaCode.ToUpper() + "TRIP";
            using (var context = new DMSDBContext())
            {
                int businessAreaId = context.BusinessAreas.FirstOrDefault(t => t.BusinessAreaCode == businessAreaCode).ID;
                var trip = context.TripHeaders.Where(t => t.BusinessAreaId == businessAreaId).OrderByDescending(t => t.ID).FirstOrDefault();
                if (trip != null)
                {
                    int lastOrderYear = trip.TripDate.Year;
                    if (DateTime.Now.Year != lastOrderYear)
                    {
                        tripNo += "000000000001";
                    }
                    else
                    {
                        string tripSequnceString = trip.TripNumber.Substring(trip.TripNumber.Length - 11);
                        int tripSequnceNumber = Convert.ToInt32(tripSequnceString) + 1;

                        tripNo += tripSequnceNumber.ToString().PadLeft(11, '0');
                    }
                }
                else
                {
                    tripNo += "000000000001";
                }
            }
            return tripNo;
        }
        private string GetLatestStopPointStatus(int stopPointId)
        {
            string lastStatus = string.Empty;
            using (var context = new DataModel.DMSDBContext())
            {
                var stopPointStatusHistories = context.TripStatusHistories.Where(t => t.StopPointId == stopPointId).OrderByDescending(t => t.StatusDate);
                if (stopPointStatusHistories != null)
                {
                    int statusId = stopPointStatusHistories.FirstOrDefault().TripStatusId;
                    lastStatus = context.TripStatuses.FirstOrDefault(t => t.ID == statusId).StatusCode;
                }
            }
            return lastStatus;
        }
        private int GetImageTypeId(int imageTypeCode)
        {
            int imageTypeId = 0;

            using (var context = new DataModel.DMSDBContext())
            {
                var imageType = context.ImageTypes.FirstOrDefault(t => t.ImageTypeCode == imageTypeCode);
                if (imageType != null)
                {
                    imageTypeId = imageType.ID;
                }
            }
            return imageTypeId;
        }
        #endregion

        public TripResponse CreateUpdateTrip(TripRequest request)
        {
            TripResponse response = new TripResponse();
            using (var context = new DMSDBContext())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var trip in request.Requests)
                        {
                            int userId = 0;
                            int statusId = 0;
                            #region Check if Driver Exists
                            var driver = context.Drivers.FirstOrDefault(t => t.DriverNo == trip.DriverNo);
                            if (driver != null)
                                userId = driver.ID;
                            else
                            {
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = "Driver Number " + trip.DriverName + " is not available in DMS";
                                return response;
                            }
                            #endregion

                            #region Check if Trip Status Exists
                            var status = context.TripStatuses.FirstOrDefault(t => t.StatusCode == trip.TripStatusCode);
                            if (status != null)
                                statusId = status.ID;
                            else
                            {
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = "Trip Status " + trip.TripStatusCode + " is not available in DMS";
                                return response;
                            }
                            #endregion

                            #region Check if Business Area Exists
                            int businessAreaId = 0;
                            var businessArea = context.BusinessAreas.FirstOrDefault(t => t.BusinessAreaCode == trip.BusinessAreaCode);
                            if (businessArea != null)
                                businessAreaId = businessArea.ID;
                            else
                            {
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = "Business Area Code " + trip.BusinessAreaCode + " is not available in DMS";
                                return response;
                            }
                            #endregion

                            #region Check if Shipment Schedule Image exists
                            int? imageId = null;
                            if (!string.IsNullOrEmpty(trip.ShipmentScheduleImageGUID))
                            {
                                var shipmentImageData = context.ImageGuids.FirstOrDefault(t => t.ImageGuIdValue == trip.ShipmentScheduleImageGUID);
                                if (shipmentImageData != null)
                                    imageId = shipmentImageData.ID;
                                else
                                {
                                    DataModel.ImageGuId imageGuId = new ImageGuId()
                                    {
                                        ImageGuIdValue = trip.ShipmentScheduleImageGUID,
                                        IsActive = true
                                    };
                                    context.ImageGuids.Add(imageGuId);
                                    imageId = imageGuId.ID;
                                }
                            }

                            #endregion

                            #region Check if OrderNumber already exists
                            int tripId = 0;
                            var existingTrip = context.TripHeaders.FirstOrDefault(t => t.OrderNumber == trip.OrderNumber);
                            #endregion

                            #region Update Trip Data
                            if (existingTrip != null)
                            {
                                existingTrip.TransporterCode = trip.TransporterCode;
                                existingTrip.TransporterName = trip.TransporterName;
                                existingTrip.DriverId = userId;
                                existingTrip.VehicleType = trip.VehicleType;
                                existingTrip.VehicleNumber = trip.VehicleNumber;
                                existingTrip.TripType = trip.TripType;
                                existingTrip.Weight = trip.Weight;
                                existingTrip.PoliceNumber = trip.PoliceNumber;
                                existingTrip.CurrentTripStatusId = statusId;
                                existingTrip.BusinessAreaId = businessAreaId;
                                existingTrip.ShipmentScheduleImageID = imageId;

                                context.Entry(existingTrip).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                tripId = existingTrip.ID;
                                foreach (var tripLocation in trip.TripLocations)
                                {
                                    #region Check if Partner Types Exists
                                    int partnerTypeId = 0;
                                    string partnerTypeCode = tripLocation.PartnerType.ToString();
                                    var partnerType = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partnerTypeCode);
                                    if (partnerType != null)
                                        partnerTypeId = partnerType.ID;
                                    else
                                    {
                                        transaction.Rollback();
                                        response.Status = DomainObjects.Resource.ResourceData.Failure;
                                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                                        response.StatusMessage = "Partner Type " + tripLocation.PartnerType + " is not available in DMS";
                                        return response;
                                    }
                                    #endregion

                                    #region Check If Partners exists
                                    int locationId = 0;
                                    var location = (from partner in context.Partners
                                                    join ppt in context.PartnerPartnerTypes on partner.ID equals ppt.PartnerId
                                                    where partner.PartnerNo == tripLocation.PartnerNo && ppt.PartnerTypeId == partnerTypeId
                                                    select new
                                                    {
                                                        ID = partner.ID
                                                    }).FirstOrDefault();

                                    if (location != null)
                                        locationId = location.ID;
                                    else
                                    {
                                        transaction.Rollback();
                                        response.Status = DomainObjects.Resource.ResourceData.Failure;
                                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                                        response.StatusMessage = tripLocation.PartnerNo + " is not available in DMS";
                                        return response;
                                    }
                                    #endregion

                                    #region Create Trip Detail if not Exist already
                                    int stopPointId = 0;
                                    var existingTripDetail = context.TripDetails.FirstOrDefault(t => t.SequenceNumber == tripLocation.SequnceNumber && t.TripID == tripId);
                                    if (existingTripDetail != null)
                                    {
                                        existingTripDetail.PartnerId = locationId;
                                        existingTripDetail.ActualDeliveryDate = tripLocation.ActualDeliveryDate;
                                        existingTripDetail.EstimatedDeliveryDate = tripLocation.EstimatedDeliveryDate;

                                        context.Entry(existingTripDetail).State = System.Data.Entity.EntityState.Modified;
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        DataModel.TripDetail tripDetail = new DataModel.TripDetail()
                                        {
                                            TripID = tripId,
                                            PartnerId = locationId,
                                            SequenceNumber = tripLocation.SequnceNumber,
                                            ActualDeliveryDate = tripLocation.ActualDeliveryDate,
                                            EstimatedDeliveryDate = tripLocation.EstimatedDeliveryDate,
                                            CreatedBy = "SYSTEM",
                                            CreatedTime = DateTime.Now
                                        };

                                        context.TripDetails.Add(tripDetail);
                                        context.SaveChanges();
                                        stopPointId = tripDetail.ID;
                                    }
                                    #endregion

                                    #region Create Trip Event Log
                                    DataModel.TripStatusHistory tripEventLog = new DataModel.TripStatusHistory()
                                    {
                                        StopPointId = stopPointId,
                                        StatusDate = DateTime.Now,
                                        Remarks = "Driver Assigned to Trip",
                                        TripStatusId = statusId
                                    };
                                    context.TripStatusHistories.Add(tripEventLog);
                                    context.SaveChanges();
                                    #endregion
                                }
                            }
                            #endregion
                            #region Create Trip data
                            else
                            {

                                DataModel.TripHeader tripRequest = new DataModel.TripHeader()
                                {
                                    OrderNumber = trip.OrderNumber,
                                    TripNumber = GetTripNumber(trip.BusinessAreaCode),
                                    TransporterCode = trip.TransporterCode,
                                    TransporterName = trip.TransporterName,
                                    DriverId = userId,
                                    VehicleType = trip.VehicleType,
                                    VehicleNumber = trip.VehicleNumber,
                                    TripType = trip.TripType,
                                    Weight = trip.Weight,
                                    PoliceNumber = trip.PoliceNumber,
                                    CurrentTripStatusId = statusId,
                                    OrderType = trip.OrderType,
                                    TripDate = DateTime.Now,
                                    BusinessAreaId = businessAreaId,
                                    ShipmentScheduleImageID = imageId
                                };
                                context.TripHeaders.Add(tripRequest);
                                context.SaveChanges();
                                tripId = tripRequest.ID;

                                foreach (var tripLocation in trip.TripLocations)
                                {
                                    #region Check if Partner Types Exists
                                    int partnerTypeId = 0;
                                    string partnerTypeCode = tripLocation.PartnerType.ToString();
                                    var partnerType = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == partnerTypeCode);
                                    if (partnerType != null)
                                        partnerTypeId = partnerType.ID;
                                    else
                                    {
                                        transaction.Rollback();
                                        response.Status = DomainObjects.Resource.ResourceData.Failure;
                                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                                        response.StatusMessage = "Partner Type " + tripLocation.PartnerType + " is not available in DMS";
                                        return response;
                                    }
                                    #endregion

                                    #region Check If Partners exists
                                    int locationId = 0;

                                    var partner = context.Partners.FirstOrDefault(t => t.PartnerNo.Trim().Equals(tripLocation.PartnerNo));
                                    if (partner != null)
                                    {
                                        var location = (from ppt in context.PartnerPartnerTypes
                                                        where ppt.PartnerTypeId == partnerTypeId
                                                        select new
                                                        {
                                                            ID = ppt.PartnerId
                                                        }).FirstOrDefault();

                                        if (location != null)
                                            locationId = location.ID;
                                        else
                                        {
                                            transaction.Rollback();
                                            response.Status = DomainObjects.Resource.ResourceData.Failure;
                                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                                            response.StatusMessage = tripLocation.PartnerNo + " is not available in DMS";
                                            return response;
                                        }
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        response.Status = DomainObjects.Resource.ResourceData.Failure;
                                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                                        response.StatusMessage = tripLocation.PartnerNo + " is not available in DMS";
                                        return response;
                                    }

                                    #endregion

                                    #region Create Trip Detail

                                    DataModel.TripDetail tripDetail = new DataModel.TripDetail()
                                    {
                                        TripID = tripId,
                                        PartnerId = locationId,
                                        SequenceNumber = tripLocation.SequnceNumber,
                                        ActualDeliveryDate = tripLocation.ActualDeliveryDate,
                                        EstimatedDeliveryDate = tripLocation.EstimatedDeliveryDate,
                                        CreatedBy = "SYSTEM",
                                        CreatedTime = DateTime.Now
                                    };

                                    context.TripDetails.Add(tripDetail);
                                    context.SaveChanges();
                                    int stopPointId = tripDetail.ID;

                                    #endregion

                                    #region Create Trip Event Log
                                    DataModel.TripStatusHistory tripEventLog = new DataModel.TripStatusHistory()
                                    {
                                        StopPointId = stopPointId,
                                        StatusDate = DateTime.Now,
                                        Remarks = "Driver Assigned to Trip",
                                        TripStatusId = statusId
                                    };
                                    context.TripStatusHistories.Add(tripEventLog);
                                    context.SaveChanges();
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        #region Return Response with Success and Commit Changes
                        transaction.Commit();
                        response.Status = DomainObjects.Resource.ResourceData.Success;
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusMessage = "Trip(s) created/updated successfully";
                        return response;
                        #endregion
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
            return response;
        }

        public StopPointOrderItemsResponse GetOrderItemsByStopPoint(StopPointsRequest stopPointsByTripRequest)
        {
            StopPointOrderItemsResponse tripResponse = new StopPointOrderItemsResponse()
            {
                Data = new List<Domain.ShipmentListDetails>()
            };

            using (var context = new DMSDBContext())
            {
                try
                {
                    foreach (var stopPoint in stopPointsByTripRequest.Requests)
                    {
                        var orderItems = (from orderItem in context.ShipmentListDetails
                                          where orderItem.StopPointId == stopPoint.ID
                                          select new Domain.ShipmentListDetails
                                          {
                                              ID = orderItem.ID,
                                              NumberOfBoxes = orderItem.NumberOfBoxes,
                                              Note = orderItem.Note,
                                              PackingSheetNumber = orderItem.PackingSheetNumber,
                                              StopPointId = orderItem.StopPointId
                                          }).ToList();

                        tripResponse.Data.AddRange(orderItems);
                    }

                    tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    tripResponse.StatusCode = (int)HttpStatusCode.OK;
                    tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                    tripResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    tripResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    tripResponse.StatusMessage = ex.Message;
                }
            }

            return tripResponse;
        }

        public StopPointsResponse GetStopPointsByTrip(StopPointsRequest stopPointsByTripRequest)
        {
            StopPointsResponse tripResponse = new StopPointsResponse()
            {
                Data = new List<Domain.StopPoints>()
            };

            using (var context = new DMSDBContext())
            {
                try
                {
                    foreach (var requestStopPoint in stopPointsByTripRequest.Requests)
                    {
                        var stopPoints = (from stopPoint in context.TripDetails
                                          where stopPoint.TripID == requestStopPoint.TripId
                                          select new Domain.StopPoints
                                          {
                                              ID = stopPoint.ID,
                                              TripId = stopPoint.TripID,
                                              LocationId = stopPoint.PartnerId,
                                              LocationName = stopPoint.Partner.PartnerName,
                                              SequenceNumber = stopPoint.SequenceNumber,
                                              ActualDeliveryDate = stopPoint.ActualDeliveryDate,
                                              EstimatedDeliveryDate = stopPoint.EstimatedDeliveryDate
                                          }).ToList();

                        tripResponse.Data.AddRange(stopPoints);
                    }

                    tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    tripResponse.StatusCode = (int)HttpStatusCode.OK;
                    tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                    tripResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    tripResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    tripResponse.StatusMessage = ex.Message;
                }
            }

            return tripResponse;
        }

        public TripResponse GetTripsByDriver(TripsByDriverRequest tripsByDriverRequest)
        {
            TripResponse tripResponse = new TripResponse()
            {
                Data = new List<Domain.TripDetails>()
            };

            using (var context = new DMSDBContext())
            {
                try
                {
                    if (tripsByDriverRequest.Requests.Count > 0)
                    {
                        var tripFilter = tripsByDriverRequest.Requests[0];

                        var tripsByUser = (from trip in context.TripHeaders
                                           where trip.DriverId == tripFilter.UserId
                                           orderby trip.TripDate ascending
                                           select new Domain.TripDetails
                                           {
                                               ID = trip.ID,
                                               TripNumber = trip.TripNumber,
                                               OrderNumber = trip.OrderNumber,
                                               TransporterName = trip.TransporterName,
                                               UserId = trip.DriverId,
                                               VehicleType = trip.VehicleType,
                                               VehicleNumber = trip.VehicleNumber,
                                               TripType = trip.TripType,
                                               Weight = trip.Weight,
                                               PoliceNumber = trip.PoliceNumber,
                                               TripStatus = trip.TripStatus.StatusName,
                                               TripStatusId = trip.CurrentTripStatusId,
                                               OrderType = trip.OrderType,
                                               TripDate = trip.TripDate,
                                               LastModifiedTime = trip.LastModifiedTime,
                                               ShipmentScheduleImageGUID = trip.ImageGuId.ImageGuIdValue
                                           }).ToList();

                        if (tripFilter.OrderType != 0)
                        {
                            tripsByUser = tripsByUser.Where(t => t.OrderType == tripFilter.OrderType).ToList();
                        }
                        else
                            tripsByUser = tripsByUser.OrderByDescending(t => t.LastModifiedTime).ToList();
                        foreach (var trip in tripsByUser)
                        {
                            var stopPoints = (from sp in context.TripDetails
                                              where sp.TripID == trip.ID
                                              select new Domain.StopPoints
                                              {
                                                  ID = sp.ID,
                                                  TripId = sp.TripID,
                                                  LocationId = sp.PartnerId,
                                                  LocationName = context.Partners.FirstOrDefault(t => t.ID == sp.PartnerId).PartnerName,
                                                  SequenceNumber = sp.SequenceNumber,
                                                  EstimatedDeliveryDate = sp.EstimatedDeliveryDate
                                              }).ToList();
                            if (stopPoints != null && stopPoints.Count > 0)
                            {
                                foreach (var stopPoint in stopPoints)
                                {
                                    stopPoint.TripStatusCode = GetLatestStopPointStatus(stopPoint.ID);
                                }
                            }

                            trip.StopPoints = stopPoints;
                        }
                        tripResponse.Data.AddRange(tripsByUser);
                    }

                    tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    tripResponse.StatusCode = (int)HttpStatusCode.OK;
                    tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                    tripResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    tripResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    tripResponse.StatusMessage = ex.Message;
                }
            }
            return tripResponse;
        }

        public UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest request)
        {
            // To Do: Has to implement code to get trips list.
            UpdateTripStatusResponse response = new UpdateTripStatusResponse()
            {
                Data = new List<Domain.TripStatusEventLog>()
            };

            using (var context = new DMSDBContext())
            {
                using (var beginDBTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //Step 1 : Get Request from 0 index
                        var tripStatusEventLogFilter = request.Requests[0];

                        DataModel.TripStatusHistory dataTripStatusEventLog = new DataModel.TripStatusHistory()
                        {
                            TripStatusId = tripStatusEventLogFilter.TripStatusId,
                            StopPointId = tripStatusEventLogFilter.StopPointId,
                            Remarks = tripStatusEventLogFilter.Remarks,
                            StatusDate = DateTime.Now,

                        };

                        //For getting trip deatails and updating trip status as assigned
                        var tripID = context.TripDetails.Where(t => t.ID == tripStatusEventLogFilter.StopPointId).Select(t => t.TripID).FirstOrDefault();
                        var tripDetails = context.TripHeaders.Where(t => t.ID == tripID).FirstOrDefault();
                        tripDetails.CurrentTripStatusId = tripStatusEventLogFilter.TripStatusId;
                        tripDetails.LastModifiedBy = "SYSTEM";
                        tripDetails.LastModifiedTime = DateTime.Now;

                        context.TripStatusHistories.Add(dataTripStatusEventLog);
                        context.SaveChanges();
                        if (tripStatusEventLogFilter.ShipmentImageGuIds != null && tripStatusEventLogFilter.ShipmentImageGuIds.Count > 0)
                        {
                            foreach (var imageGuid in tripStatusEventLogFilter.ShipmentImageGuIds)
                            {
                                DataModel.StopPointImages stopPointImages = new DataModel.StopPointImages()
                                {
                                    StopPointId = tripStatusEventLogFilter.StopPointId,
                                    ImageId = InsertImageGuid(imageGuid, request.CreatedBy),
                                    ImageTypeId = GetImageTypeId(tripStatusEventLogFilter.ImageTypeCode)
                                };
                                context.StopPointImages.Add(stopPointImages);
                                context.SaveChanges();
                            }
                        }

                        var tripCurrentStopPoint = context.TripDetails.Where(t => t.ID == tripStatusEventLogFilter.StopPointId).FirstOrDefault();
                        if (tripCurrentStopPoint != null)
                        {
                            int tripId = tripCurrentStopPoint.TripID;
                            var lstCurrentTripStopPoints = (from sp in context.TripDetails
                                                            where sp.TripID == tripId
                                                            select sp).ToList();

                            if (lstCurrentTripStopPoints != null && lstCurrentTripStopPoints.Any())
                            {
                                var lstEventLogs = new List<Domain.TripStatusEventLog>();
                                foreach (var sp in lstCurrentTripStopPoints)
                                {
                                    var location = (from loc in context.Partners
                                                    where loc.ID == sp.PartnerId
                                                    select loc).FirstOrDefault();

                                    var eventLog = from tsEventLog in context.TripStatusHistories
                                                   where sp.ID == tsEventLog.StopPointId
                                                   select new Domain.TripStatusEventLog
                                                   {
                                                       ID = tsEventLog.ID,
                                                       StopPointId = tsEventLog.StopPointId,
                                                       Remarks = tsEventLog.Remarks,
                                                       StatusDate = tsEventLog.StatusDate,
                                                       TripStatusId = tsEventLog.TripStatusId,
                                                       LocationName = location.PartnerName,
                                                       ShipmentImageGuIds = context.ImageGuids.Where(i => i.ID == tsEventLog.ID).Select(i => i.ImageGuIdValue).ToList()
                                                   };
                                    lstEventLogs.AddRange(eventLog.ToList());
                                }
                                // Order By status date
                                response.Data.AddRange(lstEventLogs.OrderByDescending(t => t.StatusDate));
                            }
                        }

                        beginDBTransaction.Commit();
                        response.Status = DomainObjects.Resource.ResourceData.Success;
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusMessage = DomainObjects.Resource.ResourceData.TripStatusUpdated;
                    }
                    catch (Exception ex)
                    {
                        beginDBTransaction.Rollback();
                        _logger.Log(LogLevel.Error, ex);
                        response.Status = DomainObjects.Resource.ResourceData.Failure;
                        response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                        response.StatusMessage = ex.Message;
                    }
                }
            }

            return response;
        }

        public TripResponse UpdateEntireTripStatus(TripsByDriverRequest tripsByDriverRequest)
        {
            TripResponse tripResponse = new TripResponse()
            {
                Data = new List<Domain.TripDetails>()
            };

            using (var context = new DMSDBContext())
            {
                try
                {
                    if (tripsByDriverRequest.Requests.Count > 0)
                    {
                        var tripFilter = tripsByDriverRequest.Requests[0];

                        //For getting trip deatails and updating trip status
                        var tripDetails = context.TripHeaders.Where(t => t.ID == tripFilter.ID).FirstOrDefault();
                        tripDetails.CurrentTripStatusId = tripFilter.TripStatusId;
                        tripDetails.LastModifiedBy = "SYSTEM";
                        tripDetails.LastModifiedTime = DateTime.Now;
                        context.SaveChanges();

                        var tripsByUser = (from trip in context.TripHeaders
                                           where trip.DriverId == tripFilter.UserId
                                           select new Domain.TripDetails
                                           {
                                               ID = trip.ID,
                                               TripNumber = trip.TripNumber,
                                               OrderNumber = trip.OrderNumber,
                                               TransporterName = trip.TransporterName,
                                               UserId = trip.DriverId,
                                               VehicleType = trip.VehicleType,
                                               VehicleNumber = trip.VehicleNumber,
                                               TripType = trip.TripType,
                                               Weight = trip.Weight,
                                               PoliceNumber = trip.PoliceNumber,
                                               TripStatus = trip.TripStatus.StatusName,
                                               TripStatusId = trip.CurrentTripStatusId
                                           }).ToList();


                        foreach (var trip in tripsByUser)
                        {
                            var stopPoints = (from sp in context.TripDetails
                                              where sp.TripID == trip.ID
                                              select new Domain.StopPoints
                                              {
                                                  ID = sp.ID,
                                                  TripId = sp.TripID,
                                                  LocationId = sp.PartnerId,
                                                  LocationName = context.Partners.FirstOrDefault(t => t.ID == sp.PartnerId).PartnerName,
                                                  SequenceNumber = sp.SequenceNumber,
                                                  EstimatedDeliveryDate = sp.EstimatedDeliveryDate
                                              }).ToList();

                            trip.StopPoints = stopPoints;
                        }

                        tripResponse.Data.AddRange(tripsByUser);
                    }

                    tripResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    tripResponse.StatusCode = (int)HttpStatusCode.OK;
                    tripResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex);
                    tripResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    tripResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    tripResponse.StatusMessage = ex.Message;
                }
            }
            return tripResponse;
        }

        public int InsertImageGuid(string imageGuidValue, string createdBy)
        {
            try
            {
                using (var dMSDBContext = new DMSDBContext())
                {
                    //Inserting new record along with IsActive true
                    ImageGuId imageGuidObject = new ImageGuId()
                    {
                        ImageGuIdValue = imageGuidValue,
                        CreatedBy = createdBy,
                        IsActive = true
                    };

                    dMSDBContext.ImageGuids.Add(imageGuidObject);
                    dMSDBContext.SaveChanges();
                    return imageGuidObject.ID;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
            }
            return 0;
        }
        public string GetOrderNumber(int stopPointId)
        {
            string orderNumber = "";
            using (var context = new DMSDBContext())
            {
                try
                {
                    int tripId = context.TripDetails.FirstOrDefault(t => t.ID == stopPointId).TripID;
                    orderNumber = context.TripHeaders.FirstOrDefault(t => t.ID == tripId).OrderNumber;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
            return orderNumber;
        }

        public string GetOrderStatusCode(int tripStatusId)
        {
            string tripStatusCode = "";
            using (var context = new DMSDBContext())
            {
                try
                {
                    tripStatusCode = context.TripStatuses.FirstOrDefault(t => t.ID == tripStatusId).StatusCode;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
            return tripStatusCode;
        }

        public int GetOrderSequnceNumber(int stopPointId)
        {
            int sequnceNumber = 0;
            using (var context = new DMSDBContext())
            {
                try
                {
                    sequnceNumber = context.TripDetails.FirstOrDefault(t => t.ID == stopPointId).SequenceNumber;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
            return sequnceNumber;
        }
        public StopPointsResponse GetLastTripStatusData(int stopPointId)
        {
            StopPointsResponse response = new StopPointsResponse()
            {
                Data = new List<StopPoints>()
            };

            using (var context = new DataModel.DMSDBContext())
            {
                try
                {
                    var statusData = (from status in context.TripStatusHistories
                                      join td in context.TripDetails on status.StopPointId equals td.ID
                                      where status.StopPointId == stopPointId orderby status.StatusDate descending
                                      select new StopPoints
                                      {
                                          ID = status.StopPointId,
                                          TripId = td.TripID,
                                          LocationId = td.PartnerId,
                                          LocationName = td.Partner.PartnerName,
                                          SequenceNumber = td.SequenceNumber,
                                          ActualDeliveryDate = td.ActualDeliveryDate,
                                          EstimatedDeliveryDate = td.EstimatedDeliveryDate,
                                          TripStatusCode = status.TripStatus.StatusCode
                                      }).ToList();
                    response.Data = statusData;
                    response.Status = DomainObjects.Resource.ResourceData.Success;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.StatusMessage = "Last status retrieved.";
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
