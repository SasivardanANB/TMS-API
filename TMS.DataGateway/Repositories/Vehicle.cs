﻿using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using Domain = TMS.DomainObjects.Objects;
using DataModel = TMS.DataGateway.DataModels;

namespace TMS.DataGateway.Repositories
{
    public class Vehicle : IVehicle
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public VehicleResponse CreateUpdateVehicle(VehicleRequest vehicleRequest)
        {
            VehicleResponse vehicleResponse = new VehicleResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Vehicle, DataModel.Vehicle>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var vehicles = mapper.Map<List<Domain.Vehicle>, List<DataModel.Vehicle>>(vehicleRequest.Requests);
                    foreach (var vehicleData in vehicles)
                    {
                        //For making IsDelete initially false
                        vehicleData.IsDelete = false;

                        //For update vehicle
                        if (vehicleData.ID > 0)
                        {
                            vehicleData.LastModifiedBy = vehicleRequest.LastModifiedBy;
                            vehicleData.LastModifiedTime = DateTime.Now;
                            tMSDBContext.Entry(vehicleData).State = System.Data.Entity.EntityState.Modified;
                            tMSDBContext.SaveChanges();
                            vehicleResponse.StatusMessage = DomainObjects.Resource.ResourceData.VehicleUpdated;
                        }

                        //For create vehicle
                        else
                        {
                            vehicleData.CreatedBy = vehicleRequest.CreatedBy;
                            vehicleData.CreatedTime = DateTime.Now;
                            tMSDBContext.Vehicles.Add(vehicleData);
                            tMSDBContext.SaveChanges();
                            vehicleResponse.StatusMessage = DomainObjects.Resource.ResourceData.VehicleCreated;
                        }
                    }
                    vehicleRequest.Requests = mapper.Map<List<DataModel.Vehicle>, List<Domain.Vehicle>>(vehicles);
                    vehicleResponse.Data = vehicleRequest.Requests;
                    vehicleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    vehicleResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                vehicleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                vehicleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                vehicleResponse.StatusMessage = ex.Message;
            }
            return vehicleResponse;
        }

        public VehicleResponse DeleteVehicle(int vehicleID)
        {
            VehicleResponse VehicleResponse = new VehicleResponse();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var vehicleDetails = tMSDBContext.Vehicles.Where(i => i.ID == vehicleID).FirstOrDefault();
                    if (vehicleDetails != null)
                    {
                        //For delete vehicle (soft delete)
                        //Need to assign lastmodifiedby using session userid
                        vehicleDetails.LastModifiedTime = DateTime.Now;
                        vehicleDetails.IsDelete = true;
                        tMSDBContext.SaveChanges();
                        VehicleResponse.StatusCode = (int)HttpStatusCode.OK;
                        VehicleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        VehicleResponse.StatusMessage = DomainObjects.Resource.ResourceData.VehicleDeleted;
                    }
                    else
                    {
                        VehicleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        VehicleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        VehicleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                VehicleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                VehicleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                VehicleResponse.StatusMessage = ex.Message;
            }
            return VehicleResponse;
        }

        public VehicleResponse GetVehicles(VehicleRequest vehicleRequest)
        {
            VehicleResponse vehicleResponse = new VehicleResponse();
            List<Domain.Vehicle> vehiclesList;
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    var userDetails = tMSDBContext.Tokens.Where(t => t.TokenKey == vehicleRequest.Token).FirstOrDefault();
                    var userData = tMSDBContext.Users.Where(t => t.ID == userDetails.UserID).FirstOrDefault();
                    var picDetails = tMSDBContext.Pics.Where(p => p.PICEmail == userData.Email).Select(x => x.ID).ToList();

                    if (picDetails.Count > 0)
                    {
                        var partnerDataCk = (from partner in tMSDBContext.Partners
                                             join ptype in tMSDBContext.PartnerPartnerTypes on partner.ID equals ptype.PartnerId
                                             where ptype.PartnerTypeId == 1 && picDetails.Contains(partner.PICID.Value)
                                             select partner.ID).ToList();

                        vehiclesList = tMSDBContext.Vehicles.Where(v => !v.IsDelete && partnerDataCk.Contains(v.ShipperID)).Select(vehicle => new Domain.Vehicle
                        {
                            ID = vehicle.ID,
                            VehicleTypeID = vehicle.VehicleTypeID,
                            VehicleTypeDescription = vehicle.VehicleType.VehicleTypeDescription,
                            //VehicleTypeName = vehicle.VehicleTypeName,
                            KIRNo = vehicle.KIRNo,
                            KIRExpiryDate = vehicle.KIRExpiryDate,
                            MaxDimension = vehicle.MaxDimension,
                            PoolID = vehicle.PoolID,
                            PoolName = vehicle.Pool.PoolName,
                            ShipperID = vehicle.ShipperID,
                            ShipperName = vehicle.Partner.PartnerName,
                            MaxWeight = vehicle.MaxWeight,
                            IsDedicated = vehicle.IsDedicated,
                            PlateNumber = vehicle.PlateNumber,
                            PoliceNo = vehicle.PoliceNo,
                        }).ToList();

                    }
                    else
                    {
                        vehiclesList = tMSDBContext.Vehicles.Where(v => !v.IsDelete).Select(vehicle => new Domain.Vehicle
                        {
                            ID = vehicle.ID,
                            VehicleTypeID = vehicle.VehicleTypeID,
                            VehicleTypeDescription = vehicle.VehicleType.VehicleTypeDescription,
                            //VehicleTypeName = vehicle.VehicleTypeName,
                            KIRNo = vehicle.KIRNo,
                            KIRExpiryDate = vehicle.KIRExpiryDate,
                            MaxDimension = vehicle.MaxDimension,
                            PoolID = vehicle.PoolID,
                            PoolName = vehicle.Pool.PoolName,
                            ShipperID = vehicle.ShipperID,
                            ShipperName = vehicle.Partner.PartnerName,
                            MaxWeight = vehicle.MaxWeight,
                            IsDedicated = vehicle.IsDedicated,
                            PlateNumber = vehicle.PlateNumber,
                            PoliceNo = vehicle.PoliceNo,
                        }).ToList();
                    }
                }

                // Filter
                if (vehicleRequest.Requests.Count > 0)
                {
                    var vehicleFilter = vehicleRequest.Requests[0];

                    if (vehicleFilter.ID > 0)
                    {
                        vehiclesList = vehiclesList.Where(s => s.ID == vehicleFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.VehicleTypeName))
                    {
                        vehiclesList = vehiclesList.Where(s => s.VehicleTypeName.ToLower().Contains(vehicleFilter.VehicleTypeName.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.KIRNo))
                    {
                        vehiclesList = vehiclesList.Where(s => s.KIRNo.ToLower().Contains(vehicleFilter.KIRNo.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.KIRExpiryDate.ToString()))
                    {
                        vehiclesList = vehiclesList.Where(s => s.KIRExpiryDate.ToString().Contains(vehicleFilter.KIRExpiryDate.ToString())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.MaxDimension))
                    {
                        vehiclesList = vehiclesList.Where(s => s.MaxDimension.ToLower().Contains(vehicleFilter.MaxDimension.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.MaxWeight.ToString()) && vehicleFilter.MaxWeight > 0)
                    {
                        vehiclesList = vehiclesList.Where(s => s.MaxWeight == vehicleFilter.MaxWeight).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.PoolName))
                    {
                        vehiclesList = vehiclesList.Where(s => s.PlateNumber.ToLower().Contains(vehicleFilter.PoolName.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.ShipperName))
                    {
                        vehiclesList = vehiclesList.Where(s => s.PlateNumber.ToLower().Contains(vehicleFilter.ShipperName.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.VehicleTypeDescription))
                    {
                        vehiclesList = vehiclesList.Where(s => s.PlateNumber.ToLower().Contains(vehicleFilter.VehicleTypeDescription.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.PlateNumber))
                    {
                        vehiclesList = vehiclesList.Where(s => s.PlateNumber.ToLower().Contains(vehicleFilter.PlateNumber.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(vehicleFilter.PoliceNo))
                    {
                        vehiclesList = vehiclesList.Where(s => s.PoliceNo.Contains(vehicleFilter.PoliceNo)).ToList();
                    }

                    if (vehicleFilter.IsDedicated)
                    {
                        vehiclesList = vehiclesList.Where(s => s.IsDedicated == vehicleFilter.IsDedicated).ToList();
                    }

                    if (vehicleFilter.IsDelete)
                    {
                        vehiclesList = vehiclesList.Where(s => s.IsDelete == vehicleFilter.IsDelete).ToList();
                    }
                }

                // GLobal Search Filter
                if (!string.IsNullOrEmpty(vehicleRequest.GlobalSearch))
                {
                    string globalSearch = vehicleRequest.GlobalSearch.ToLower();
                    vehiclesList = vehiclesList.Where(s => !s.IsDelete && (s.PlateNumber != null && s.PlateNumber.ToLower().Contains(globalSearch))
                    || (s.MaxDimension != null && s.MaxDimension.ToLower().Contains(globalSearch))
                    || s.MaxWeight.ToString().Contains(globalSearch)
                    || (s.PoolName != null && s.PoolName.ToLower().Contains(globalSearch))
                    || (s.VehicleTypeDescription != null && s.VehicleTypeDescription.ToLower().Contains(globalSearch))
                    || (s.ShipperName != null && s.ShipperName.ToLower().Contains(globalSearch))
                    ).ToList();
                }

                // Sorting
                if (vehiclesList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(vehicleRequest.SortOrder))
                    {
                        switch (vehicleRequest.SortOrder.ToLower())
                        {
                            case "vehicletype":
                                vehiclesList = vehiclesList.OrderBy(s => s.VehicleTypeName).ToList();
                                break;
                            case "vehicletype_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.VehicleTypeName).ToList();
                                break;
                            case "kirnumber":
                                vehiclesList = vehiclesList.OrderBy(s => s.KIRNo).ToList();
                                break;
                            case "kirnumber_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.KIRNo).ToList();
                                break;
                            case "kirexpirydate":
                                vehiclesList = vehiclesList.OrderBy(s => s.KIRExpiryDate).ToList();
                                break;
                            case "kirexpirydate_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.KIRExpiryDate).ToList();
                                break;
                            case "maxdimension":
                                vehiclesList = vehiclesList.OrderBy(s => s.MaxDimension).ToList();
                                break;
                            case "maxdimension_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.MaxDimension).ToList();
                                break;
                            case "maxweight":
                                vehiclesList = vehiclesList.OrderBy(s => s.MaxWeight).ToList();
                                break;
                            case "maxweight_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.MaxWeight).ToList();
                                break;
                            case "platenumber":
                                vehiclesList = vehiclesList.OrderBy(s => s.PlateNumber).ToList();
                                break;
                            case "platenumber_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.PlateNumber).ToList();
                                break;
                            case "policenumber":
                                vehiclesList = vehiclesList.OrderBy(s => s.PoliceNo).ToList();
                                break;
                            case "policenumber_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.PoliceNo).ToList();
                                break;
                            case "pool":
                                vehiclesList = vehiclesList.OrderBy(s => s.PoolName).ToList();
                                break;
                            case "pool_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.PoolName).ToList();
                                break;
                            case "shippername":
                                vehiclesList = vehiclesList.OrderBy(s => s.ShipperName).ToList();
                                break;
                            case "shippername_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.ShipperName).ToList();
                                break;
                            case "vehicletypedescription":
                                vehiclesList = vehiclesList.OrderBy(s => s.VehicleTypeDescription).ToList();
                                break;
                            case "vehicletypedescription_desc":
                                vehiclesList = vehiclesList.OrderByDescending(s => s.VehicleTypeDescription).ToList();
                                break;
                            default:  // ID Descending 
                                vehiclesList = vehiclesList.OrderByDescending(s => s.ID).ToList();
                                break;
                        }
                    }
                    else
                    {
                        vehiclesList = vehiclesList.OrderByDescending(s => s.ID).ToList();
                    }
                }

                // Total NumberOfRecords
                vehicleResponse.NumberOfRecords = vehiclesList.Count;

                // Paging
                int pageSize = Convert.ToInt32(vehicleRequest.PageSize);
                int pageNumber = (vehicleRequest.PageNumber ?? 1);
                if (pageSize > 0)
                {
                    vehiclesList = vehiclesList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                if (vehiclesList.Count > 0)
                {
                    vehicleResponse.Data = vehiclesList;
                    vehicleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    vehicleResponse.StatusCode = (int)HttpStatusCode.OK;
                    vehicleResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    vehicleResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    vehicleResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    vehicleResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                vehicleResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                vehicleResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                vehicleResponse.StatusMessage = ex.Message;
            }
            return vehicleResponse;
        }

        public CommonCodeResponse GetVehiclesPlateNumbers(string searchText, int transporterId)
        {
            CommonCodeResponse commonCodeResponse = new CommonCodeResponse();
            List<Domain.CommonCode> commonCodes;
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    if (searchText != string.Empty && searchText != null && transporterId > 0)
                    {
                        commonCodes = tMSDBContext.Vehicles.Where(v => !v.IsDelete && v.PoliceNo.Contains(searchText) && v.ShipperID == transporterId).Select(vehicle => new Domain.CommonCode
                        {
                            Id = vehicle.PlateNumber,
                            Value = vehicle.PlateNumber
                        }).ToList();
                    }
                    else if (searchText != string.Empty && searchText != null && transporterId == 0)
                    {
                        commonCodes = tMSDBContext.Vehicles.Where(v => !v.IsDelete && v.PoliceNo.Contains(searchText)).Select(vehicle => new Domain.CommonCode
                        {
                            Id = vehicle.PlateNumber,
                            Value = vehicle.PlateNumber
                        }).ToList();
                    }
                    else if ((searchText == string.Empty || searchText == null) && transporterId >0)
                    {
                        commonCodes = tMSDBContext.Vehicles.Where(v => !v.IsDelete && v.ShipperID == transporterId ).Select(vehicle => new Domain.CommonCode
                        {
                            Id = vehicle.PlateNumber,
                            Value = vehicle.PlateNumber
                        }).ToList();
                    }
                    else
                    {
                        commonCodes = tMSDBContext.Vehicles.Where(v => !v.IsDelete).Select(vehicle => new Domain.CommonCode
                        {
                            Id = vehicle.PlateNumber,
                            Value = vehicle.PlateNumber
                        }).ToList();
                    }

                    if (commonCodes.Count > 0)
                    {
                        commonCodeResponse.NumberOfRecords = commonCodes.Count;
                        commonCodeResponse.Data = commonCodes;
                        commonCodeResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonCodeResponse.StatusCode = (int)HttpStatusCode.OK;
                        commonCodeResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        commonCodeResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonCodeResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        commonCodeResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                commonCodeResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                commonCodeResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                commonCodeResponse.StatusMessage = ex.Message;
            }
            return commonCodeResponse;
        }
    }
}
