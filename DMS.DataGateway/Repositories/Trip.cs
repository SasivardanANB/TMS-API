﻿using AutoMapper;
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

namespace DMS.DataGateway.Repositories
{
    public class Trip : ITrip
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
                                               PoliceNumber = trip.PoliceNumber
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

        public UpdateTripStatusResponse UpdateTripStatusEventLog(UpdateTripStatusRequest request)
        {
            // To Do: Has to implement code to get trips list.
            UpdateTripStatusResponse response = new UpdateTripStatusResponse()
            {
                Data = new List<Domain.TripStatusEventLog>()
            };

            try
            {
                using (var context = new DMSDBContext())
                {
                    //Step 1 : Get Request from 0 index
                    var tripStatusEventLogFilter = request.Requests[0];

                    DataModel.TripStatusEventLog dataTripStatusEventLog = new DataModel.TripStatusEventLog()
                    {
                        TripStatusId = tripStatusEventLogFilter.TripStatusId,
                        StopPointId = tripStatusEventLogFilter.StopPointId,
                        Remarks = tripStatusEventLogFilter.Remarks,
                        StatusDate = DateTime.Now
                    };
                    context.TripStatusEventLogs.Add(dataTripStatusEventLog);
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
                            foreach(var sp in lstCurrentTripStopPoints)
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
                                                    LocationName = location.Name
                                                };
                                lstEventLogs.AddRange(eventLog.ToList());
                            }
                            // Order By status date
                            response.Data.AddRange(lstEventLogs.OrderByDescending(t => t.StatusDate));
                        }
                    }                    

                    response.Status = DomainObjects.Resource.ResourceData.Success;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.StatusMessage = DomainObjects.Resource.ResourceData.TripStatusUpdated;
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
    }
}