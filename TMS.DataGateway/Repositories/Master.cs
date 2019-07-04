﻿using AutoMapper;
using NLog;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DataGateway.DataModels;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataModel = TMS.DataGateway.DataModels;
using Domain = TMS.DomainObjects.Objects;
using System.Configuration;
using System.Data.Common;

namespace TMS.DataGateway.Repositories
{
    public class Master : IMaster
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PartnerSearchResponse GetPartners(PartnerSearchRequest partnerSearchRequest)
        {
            PartnerSearchResponse partnerSearchResponse = new PartnerSearchResponse();

            List<Domain.Common> partnerList = new List<Domain.Common>();
            try
            {
                PartnerSearch partnerSearch = partnerSearchRequest.Requests[0];
                using (var context = new TMSDBContext())
                {
                    if (string.IsNullOrEmpty(partnerSearch.SearchText))
                    {
                        partnerList =
                                                (from partner in context.Partners
                                                 join ppt in context.PartnerPartnerTypes on partner.ID equals ppt.PartnerId
                                                 where ppt.PartnerTypeId == partnerSearch.PartnerTypeId && partner.IsActive == true
                                                 select new Domain.Common
                                                 {
                                                     Id = partner.ID,
                                                     Value = partner.PartnerName
                                                 }).ToList();
                    }
                    else
                    {
                        partnerList =
                            (from partner in context.Partners
                             join ppt in context.PartnerPartnerTypes on partner.ID equals ppt.PartnerId
                             where partner.PartnerName.Contains(partnerSearch.SearchText) && ppt.PartnerTypeId == partnerSearch.PartnerTypeId && partner.IsActive == true
                             select new Domain.Common
                             {
                                 Id = partner.ID,
                                 Value = partner.PartnerName
                             }).ToList();
                    }
                }

                // Total NumberOfRecords
                partnerSearchResponse.NumberOfRecords = partnerList.Count;

                if (partnerList.Count > 0)
                {
                    partnerSearchResponse.Data = partnerList;
                    partnerSearchResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    partnerSearchResponse.StatusCode = (int)HttpStatusCode.OK;
                    partnerSearchResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    partnerSearchResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    partnerSearchResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    partnerSearchResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                partnerSearchResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerSearchResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerSearchResponse.StatusMessage = ex.Message;
            }
            return partnerSearchResponse;
        }


        public CommonCodeResponse GetDriverNames()
        {
            CommonCodeResponse commonResponse = new CommonCodeResponse();
            using (var context = new TMSDBContext())
            {
                var driversList = context.Drivers.Where(driver => !driver.IsDelete && driver.IsActive).Select(response => new Domain.CommonCode()
                {
                    Id = response.DriverNo,
                    Value = response.UserName
                }).ToList();

                // Total NumberOfRecords
                commonResponse.NumberOfRecords = driversList.Count;

                if (driversList.Count > 0)
                {
                    commonResponse.Data = driversList;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
                return commonResponse;
            }
        }

        public CommonResponse GetVehicleTypeNames()
        {
            CommonResponse commonResponse = new CommonResponse();
            using (var context = new TMSDBContext())
            {
                var vehicleTypeList = context.VehicleTypes.Select(response => new Domain.Common()
                {
                    Id = response.ID,
                    Value = response.VehicleTypeDescription
                }).ToList();

                // Total NumberOfRecords
                commonResponse.NumberOfRecords = vehicleTypeList.Count;

                if (vehicleTypeList.Count > 0)
                {
                    commonResponse.Data = vehicleTypeList;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
                return commonResponse;
            }
        }

        public CommonResponse GetFleetTypeNames()
        {
            CommonResponse commonResponse = new CommonResponse();
            using (var context = new TMSDBContext())
            {
                var fleetTypeList = context.FleetTypes.Select(response => new Domain.Common()
                {
                    Id = response.ID,
                    Value = response.FleetTypeDescription
                }).ToList();

                // Total NumberOfRecords
                commonResponse.NumberOfRecords = fleetTypeList.Count;

                if (fleetTypeList.Count > 0)
                {
                    commonResponse.Data = fleetTypeList;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
                return commonResponse;
            }
        }

        public PartnerDetilasResponse GetPartnersDetails(int partnerId)
        {
            PartnerDetilasResponse partnerResponse = new PartnerDetilasResponse();

            try
            {
                using (var context = new TMSDBContext())
                {
                    var partnerDetails = (from partner in context.Partners
                                          join postalcode in context.PostalCodes on partner.PostalCodeID equals postalcode.ID
                                          join subDistrict in context.SubDistricts on postalcode.SubDistrictID equals subDistrict.ID
                                          where partner.ID == partnerId
                                          select new Domain.PartnerDeatils
                                          {
                                              Address = partner.PartnerAddress,
                                              CityId = subDistrict.City.ID,
                                              CityName = subDistrict.City.CityDescription,
                                              SubDistrictName = subDistrict.SubdistrictName,
                                              SubDistrictId = subDistrict.ID,
                                              ProvinceId = subDistrict.City.Province.ID,
                                              ProvinceName = subDistrict.City.Province.ProvinceDescription,
                                              PostalCode = postalcode.PostalCodeNo,
                                              PostalCodeId = postalcode.ID
                                          }).ToList();

                    if (partnerDetails.Count > 0)
                    {
                        partnerResponse.Data = partnerDetails;
                        partnerResponse.NumberOfRecords = partnerDetails.Count;
                        partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                        partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        partnerResponse.NumberOfRecords = 0;
                        partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        partnerResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerResponse.StatusMessage = ex.Message;
            }
            return partnerResponse;
        }

        public SubDistrictDetailsResponse GetSubDistrictDetails(string searchText)
        {
            SubDistrictDetailsResponse subDistrictDetailsResponse = new SubDistrictDetailsResponse();

            try
            {
                using (var context = new TMSDBContext())
                {
                    List<Domain.SubDistrictDeatils> subDistrictDateils = new List<Domain.SubDistrictDeatils>();
                    if (string.IsNullOrEmpty(searchText))
                    {
                        subDistrictDateils = (from subDistrict in context.SubDistricts
                                                  join postlCode in context.PostalCodes on subDistrict.ID equals postlCode.SubDistrictID
                                                  select new Domain.SubDistrictDeatils
                                                  {
                                                      SubDistrictName = subDistrict.SubdistrictName,
                                                      CityName = subDistrict.City.CityDescription,
                                                      ProvinceName = subDistrict.City.Province.ProvinceDescription,
                                                      PostalCodeId = postlCode.ID,
                                                      PostalCode = postlCode.PostalCodeNo
                                                  }).ToList();
                    }
                    else
                    {
                        subDistrictDateils = (from subDistrict in context.SubDistricts
                                                  join postlCode in context.PostalCodes on subDistrict.ID equals postlCode.SubDistrictID
                                                  where subDistrict.SubdistrictName.Contains(searchText)
                                                  select new Domain.SubDistrictDeatils
                                                  {
                                                      SubDistrictName = subDistrict.SubdistrictName,
                                                      CityName = subDistrict.City.CityDescription,
                                                      ProvinceName = subDistrict.City.Province.ProvinceDescription,
                                                      PostalCodeId = postlCode.ID,
                                                      PostalCode = postlCode.PostalCodeNo
                                                  }).ToList();
                    }
                    
                    if (subDistrictDateils.Count > 0)
                    {
                        subDistrictDetailsResponse.Data = subDistrictDateils;
                        subDistrictDetailsResponse.NumberOfRecords = subDistrictDateils.Count;
                        subDistrictDetailsResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        subDistrictDetailsResponse.StatusCode = (int)HttpStatusCode.OK;
                        subDistrictDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        subDistrictDetailsResponse.NumberOfRecords = 0;
                        subDistrictDetailsResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        subDistrictDetailsResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        subDistrictDetailsResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                subDistrictDetailsResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                subDistrictDetailsResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                subDistrictDetailsResponse.StatusMessage = ex.Message;
            }
            return subDistrictDetailsResponse;

        }

        public CommonResponse GetPoolNames(string searchText)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                using (var context = new TMSDBContext())
                {

                    var poolData = new List<Domain.Common>();
                    if (searchText != "")
                    {
                        poolData = (from pool in context.Pools
                                    where pool.PoolName.Contains(searchText)
                                    select new Domain.Common
                                    {
                                        Id = pool.ID,
                                        Value = pool.PoolName
                                    }).ToList();
                    }
                    else
                    {
                        poolData = (from pool in context.Pools
                                        // where pool.PoolName.Contains(searchText)
                                    select new Domain.Common
                                    {
                                        Id = pool.ID,
                                        Value = pool.PoolName
                                    }).ToList();
                    }

                    if (poolData.Count > 0)
                    {
                        commonResponse.Data = poolData;
                        commonResponse.NumberOfRecords = poolData.Count;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.OK;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        commonResponse.NumberOfRecords = 0;
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

        public CommonResponse GetShipperNames(string searchText)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {

                using (var context = new TMSDBContext())
                {
                    var shipperData = new List<Domain.Common>();
                    if (searchText != "")
                    {
                        shipperData = (from partner in context.Partners
                                       join ppt in context.PartnerPartnerTypes on partner.ID equals ppt.PartnerId
                                       where partner.PartnerName.Contains(searchText) && ppt.PartnerTypeId == context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == "1").ID
                                       select new Domain.Common
                                       {
                                           Id = partner.ID,
                                           Value = partner.PartnerName
                                       }).ToList();
                    }
                    else
                    {
                        shipperData = (from partner in context.Partners
                                       join ppt in context.PartnerPartnerTypes on partner.ID equals ppt.PartnerId
                                       where ppt.PartnerTypeId == context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == "1").ID // partner.PartnerName.Contains(searchText) && 
                                       select new Domain.Common
                                       {
                                           Id = partner.ID,
                                           Value = partner.PartnerName
                                       }).ToList();
                    }
                    if (shipperData.Count > 0)
                    {
                        commonResponse.Data = shipperData;
                        commonResponse.NumberOfRecords = shipperData.Count;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.OK;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        commonResponse.NumberOfRecords = 0;
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

        public CommonResponse GetCityNames(string searchText)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var cityData = new List<Domain.Common>();
                    if (searchText != "")
                    {
                        cityData = (from city in context.Cities
                                    where city.CityDescription.Contains(searchText)
                                    select new Domain.Common
                                    {
                                        Id = city.ID,
                                        Value = city.CityDescription
                                    }).ToList();
                    }
                    else
                    {
                        cityData = (from city in context.Cities
                                        // where city.CityDescription.Contains(searchText)
                                    select new Domain.Common
                                    {
                                        Id = city.ID,
                                        Value = city.CityDescription
                                    }).ToList();
                    }
                    if (cityData.Count > 0)
                    {
                        commonResponse.Data = cityData;
                        commonResponse.NumberOfRecords = cityData.Count;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.OK;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        commonResponse.NumberOfRecords = 0;
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

        public CommonResponse GetGateNamesByBusinessArea(int businessAreaId, int gateTypeId)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var gateData = new List<Domain.Common>();

                    gateData = context.G2Gs.Where(b => b.BusinessAreaId == businessAreaId && b.GateTypeId== gateTypeId).Select(data=>new Domain.Common {
                        Id=data.ID,
                        Value=data.G2GName
                    }).ToList();

                    if (gateData.Count > 0)
                    {
                        commonResponse.Data = gateData;
                        commonResponse.NumberOfRecords = gateData.Count;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.OK;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        commonResponse.NumberOfRecords = 0;
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

        public CommonResponse GetTripStatusNames(string requestType)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var tripstatusData = new List<Domain.Common>();
                    if (!string.IsNullOrEmpty(requestType))
                    {
                        tripstatusData = context.OrderStatuses.Where(c=>c.OrderStatusCode=="1"|| c.OrderStatusCode == "2"|| 
                        c.OrderStatusCode == "3" || c.OrderStatusCode == "13"|| c.OrderStatusCode == "15" || c.OrderStatusCode == "16").Select(data => new Domain.Common
                        {
                            Id = data.ID,
                            Value = data.OrderStatusValue
                        }).ToList();
                    }
                    else
                    {

                        tripstatusData = context.OrderStatuses.Select(data => new Domain.Common
                        {
                            Id = data.ID,
                            Value = data.OrderStatusValue
                        }).ToList();
                    }
                    if (tripstatusData.Count > 0)
                    {
                        commonResponse.Data = tripstatusData;
                        commonResponse.NumberOfRecords = tripstatusData.Count;
                        commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        commonResponse.StatusCode = (int)HttpStatusCode.OK;
                        commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                    }
                    else
                    {
                        commonResponse.NumberOfRecords = 0;
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

        public CommonCodeResponse GetDriversByTransporter(int transporterId)
        {
            CommonCodeResponse commonResponse = new CommonCodeResponse();
            using (var context = new TMSDBContext())
            {
                var driversList = context.Drivers.Where(driver => !driver.IsDelete && driver.IsActive && driver.TransporterId == transporterId).Select(response => new Domain.CommonCode()
                {
                    Id = response.DriverNo,
                    Value = response.UserName
                }).ToList();

                // Total NumberOfRecords
                commonResponse.NumberOfRecords = driversList.Count;

                if (driversList.Count > 0)
                {
                    commonResponse.Data = driversList;
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.OK;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    commonResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    commonResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    commonResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
                return commonResponse;
            }
        }
    }
}
