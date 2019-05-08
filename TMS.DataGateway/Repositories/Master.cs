using AutoMapper;
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
                    partnerList =
                        (from partner in context.Partners
                         where partner.PartnerName.Contains(partnerSearch.SearchText)
                         select new Domain.Common
                         {
                             Id = partner.ID,
                             Value = partner.PartnerName
                         }).ToList();
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


        public CommonResponse GetDriverNames()
        {
            CommonResponse commonResponse = new CommonResponse();
            using (var context = new TMSDBContext())
            {
                var driversList = context.Drivers.Where(driver => !driver.IsDelete && driver.IsActive).Select(response => new Domain.Common()
                {
                    Id = response.ID,
                    Value = response.DriverNo
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

                if (vehicleTypeList.Count>0)
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
    }
}
