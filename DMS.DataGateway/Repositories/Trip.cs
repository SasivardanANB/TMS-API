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
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
                            var driver = context.Users.FirstOrDefault(t => t.UserName == trip.DriverName);
                            if (driver != null)
                                userId = driver.ID;
                            else
                            {
                                transaction.Rollback();
                                response.Status = DomainObjects.Resource.ResourceData.Failure;
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                response.StatusMessage = "User " + trip.DriverName + " is not available in DMS";
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

                            #region Create/Update Trip
                            int tripId = 0;
                            #region Check if OrderNumber already exists

                            var existingTrip = context.TripDetails.FirstOrDefault(t => t.OrderNumber == trip.OrderNumber);

                            #endregion
                            if (existingTrip != null)
                            {
                                existingTrip.TransporterCode = trip.TransporterCode;
                                existingTrip.TransporterName = trip.TransporterName;
                                existingTrip.UserId = userId;
                                existingTrip.VehicleType = trip.VehicleType;
                                existingTrip.VehicleNumber = trip.VehicleNumber;
                                existingTrip.TripType = trip.TripType;
                                existingTrip.Weight = trip.Weight;
                                existingTrip.PoliceNumber = trip.PoliceNumber;
                                existingTrip.CurrentTripStatusId = statusId;

                                context.Entry(existingTrip).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                                tripId = existingTrip.ID;
                            }
                            else
                            {

                                DataModel.TripDetails tripRequest = new DataModel.TripDetails()
                                {
                                    OrderNumber = trip.OrderNumber,
                                    TripNumber = GetTripNumber(trip.BusinessAreaCode),
                                    TransporterCode = trip.TransporterCode,
                                    TransporterName = trip.TransporterName,
                                    UserId = userId,
                                    VehicleType = trip.VehicleType,
                                    VehicleNumber = trip.VehicleNumber,
                                    TripType = trip.TripType,
                                    Weight = trip.Weight,
                                    PoliceNumber = trip.PoliceNumber,
                                    CurrentTripStatusId = statusId,
                                    OrderType = trip.OrderType,
                                    TripDate = DateTime.Now,
                                    BusinessAreaCode = trip.BusinessAreaCode
                                };
                                context.TripDetails.Add(tripRequest);
                                context.SaveChanges();
                                tripId = tripRequest.ID;

                                foreach (var tripLocation in trip.TripLocations)
                                {
                                    int locationId = 0;

                                    #region Check If Locations exists
                                    var location = context.Locations.FirstOrDefault(t => t.Name == tripLocation.Name);
                                    #endregion

                                    if (location != null)
                                        locationId = location.ID;
                                    else
                                    {
                                        int cityId = 0;
                                        var city = context.Cities.FirstOrDefault(t => t.CityCode == tripLocation.CityCode);
                                        if (city != null)
                                        {
                                            cityId = city.ID;
                                            #region Create Location
                                            DataModel.Location locationData = new DataModel.Location()
                                            {
                                                TypeofLocation = tripLocation.TypeofLocation,
                                                CityId = cityId,
                                                Name = tripLocation.Name,
                                                Place = tripLocation.Place,
                                                Address = tripLocation.Address
                                            };
                                            context.Locations.Add(locationData);
                                            context.SaveChanges();
                                            locationId = locationData.ID;
                                            #endregion

                                        }
                                        else
                                        {
                                            transaction.Rollback();
                                            response.Status = DomainObjects.Resource.ResourceData.Failure;
                                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                                            response.StatusMessage = "City : + " + tripLocation.CityCode + " is not available in DMS";
                                            return response;
                                        }
                                    }
                                    #region Create Stop Points

                                    DataModel.StopPoints stopPoint = new DataModel.StopPoints()
                                    {
                                        TripID = tripId,
                                        LocationID = locationId,
                                        SequenceNumber = tripLocation.SequnceNumber,
                                        ActualDeliveryDate = tripLocation.ActualDeliveryDate,
                                        EstimatedDeliveryDate = tripLocation.EstimatedDeliveryDate
                                    };
                                    context.StopPoints.Add(stopPoint);
                                    context.SaveChanges();
                                    int stopPointId = stopPoint.ID;

                                    #endregion

                                    #region Create Trip Event Log
                                    DataModel.TripStatusEventLog tripEventLog = new DataModel.TripStatusEventLog()
                                    {
                                        StopPointId = stopPointId,
                                        StatusDate = DateTime.Now,
                                        Remarks = "Driver Assigned to Trip",
                                        TripStatusId = statusId
                                    };
                                    context.TripStatusEventLogs.Add(tripEventLog);
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

        private string GetTripNumber(string businessAreaCode)
        {
            string tripNo = businessAreaCode.ToUpper() + "TRIP";
            using (var context = new DMSDBContext())
            {
                var trip = context.TripDetails.Where(t => t.BusinessAreaCode == businessAreaCode).OrderByDescending(t => t.ID).FirstOrDefault();
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
                        var stopPoints = (from stopPoint in context.StopPoints
                                          where stopPoint.TripID == requestStopPoint.TripId
                                          select new Domain.StopPoints
                                          {
                                              ID = stopPoint.ID,
                                              TripId = stopPoint.TripID,
                                              LocationId = stopPoint.LocationID,
                                              LocationName = stopPoint.Location.Name,
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

                        var tripsByUser = (from trip in context.TripDetails
                                           where trip.UserId == tripFilter.UserId
                                           select new Domain.TripDetails
                                           {
                                               ID = trip.ID,
                                               TripNumber = trip.TripNumber,
                                               OrderNumber = trip.OrderNumber,
                                               TransporterName = trip.TransporterName,
                                               UserId = trip.UserId,
                                               VehicleType = trip.VehicleType,
                                               VehicleNumber = trip.VehicleNumber,
                                               TripType = trip.TripType,
                                               Weight = trip.Weight,
                                               PoliceNumber = trip.PoliceNumber,
                                               TripStatus = trip.TripStatus.StatusName,
                                               TripStatusId = trip.CurrentTripStatusId,
                                               OrderType = trip.OrderType
                                           }).ToList();

                        if (tripFilter.OrderType != 0)
                        {
                            tripsByUser = tripsByUser.Where(t => t.OrderType == tripFilter.OrderType).ToList();
                        }
                        foreach (var trip in tripsByUser)
                        {
                            var stopPoints = (from sp in context.StopPoints
                                              where sp.TripID == trip.ID
                                              select new Domain.StopPoints
                                              {
                                                  ID = sp.ID,
                                                  TripId = sp.TripID,
                                                  LocationId = sp.LocationID,
                                                  LocationName = context.Locations.FirstOrDefault(t => t.ID == sp.LocationID).Name,
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

                        DataModel.TripStatusEventLog dataTripStatusEventLog = new DataModel.TripStatusEventLog()
                        {
                            TripStatusId = tripStatusEventLogFilter.TripStatusId,
                            StopPointId = tripStatusEventLogFilter.StopPointId,
                            Remarks = tripStatusEventLogFilter.Remarks,
                            StatusDate = DateTime.Now,

                        };

                        //For getting trip deatails and updating trip status as assigned
                        var tripID = context.StopPoints.Where(t => t.ID == tripStatusEventLogFilter.StopPointId).Select(t => t.TripID).FirstOrDefault();
                        var tripDetails = context.TripDetails.Where(t => t.ID == tripID).FirstOrDefault();
                        tripDetails.CurrentTripStatusId = tripStatusEventLogFilter.TripStatusId;

                        context.TripStatusEventLogs.Add(dataTripStatusEventLog);
                        context.SaveChanges();
                        int guidCountValue = 0;
                        if (request.Requests[0].ShipmentImageGuIds.Count > 0 && request.Requests[0].ShipmentImageGuIds != null)
                        {
                            foreach (var imageGuid in request.Requests[0].ShipmentImageGuIds)
                            {
                                if (string.IsNullOrEmpty(request.Requests[0].ShipmentImageGuIds[guidCountValue]))
                                {
                                    TripGuid tripGuid = new TripGuid()
                                    {
                                        TripEventLogID = dataTripStatusEventLog.ID,
                                        ImageID = InsertImageGuid(request.Requests[0].ShipmentImageGuIds[guidCountValue], request.CreatedBy)
                                    };
                                    context.TripGuids.Add(tripGuid);
                                }
                                guidCountValue++;
                            }
                        }
                        context.SaveChanges();

                        var tripCurrentStopPoint = context.StopPoints.Where(t => t.ID == tripStatusEventLogFilter.StopPointId).FirstOrDefault();
                        if (tripCurrentStopPoint != null)
                        {
                            int tripId = tripCurrentStopPoint.TripID;
                            var lstCurrentTripStopPoints = (from sp in context.StopPoints
                                                            where sp.TripID == tripId
                                                            select sp).ToList();

                            if (lstCurrentTripStopPoints != null && lstCurrentTripStopPoints.Any())
                            {
                                var lstEventLogs = new List<Domain.TripStatusEventLog>();
                                foreach (var sp in lstCurrentTripStopPoints)
                                {
                                    var location = (from loc in context.Locations
                                                    where loc.ID == sp.LocationID
                                                    select loc).FirstOrDefault();

                                    var eventLog = from tsEventLog in context.TripStatusEventLogs
                                                   where sp.ID == tsEventLog.StopPointId
                                                   select new Domain.TripStatusEventLog
                                                   {
                                                       ID = tsEventLog.ID,
                                                       StopPointId = tsEventLog.StopPointId,
                                                       Remarks = tsEventLog.Remarks,
                                                       StatusDate = tsEventLog.StatusDate,
                                                       TripStatusId = tsEventLog.TripStatusId,
                                                       LocationName = location.Name,
                                                       ShipmentImageIds = context.TripGuids.Where(i => i.TripEventLogID == tsEventLog.ID).Select(i => i.ID).ToList()
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
                        var tripDetails = context.TripDetails.Where(t => t.ID == tripFilter.ID).FirstOrDefault();
                        tripDetails.CurrentTripStatusId = tripFilter.TripStatusId;
                        context.SaveChanges();

                        var tripsByUser = (from trip in context.TripDetails
                                           where trip.UserId == tripFilter.UserId
                                           select new Domain.TripDetails
                                           {
                                               ID = trip.ID,
                                               TripNumber = trip.TripNumber,
                                               OrderNumber = trip.OrderNumber,
                                               TransporterName = trip.TransporterName,
                                               UserId = trip.UserId,
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
                            var stopPoints = (from sp in context.StopPoints
                                              where sp.TripID == trip.ID
                                              select new Domain.StopPoints
                                              {
                                                  ID = sp.ID,
                                                  TripId = sp.TripID,
                                                  LocationId = sp.LocationID,
                                                  LocationName = context.Locations.FirstOrDefault(t => t.ID == sp.LocationID).Name,
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

        //For inserting new record into ImageGuid table
        public int InsertImageGuid(string imageGuidValue, string createdBy)
        {
            try
            {
                using (var dMSDBContext = new DMSDBContext())
                {
                    //Inserting new record along with IsActive true
                    ImageGuid imageGuidObject = new ImageGuid()
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
    }
}
