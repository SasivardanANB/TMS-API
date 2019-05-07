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

                if (partnerList.Count > 0)
                {
                    partnerSearchResponse.Data = partnerList;
                    partnerSearchResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    partnerSearchResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    partnerSearchResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    partnerSearchResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                partnerSearchResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerSearchResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerSearchResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return partnerSearchResponse;
        }


        public CommonResponse GetDriverNames()
        {
            CommonResponse driverNames = new CommonResponse();
            using (var context = new TMSDBContext())
            {
                var driversList = context.Drivers.Where(driver => !driver.IsDelete && driver.IsActive).Select(response => new Domain.Common()
                {
                    Id = response.ID,
                    Value = response.DriverNo
                }).ToList();
                if (driversList != null)
                {
                    driverNames.NumberOfRecords = driverNames.Data.Count;
                    driverNames.Data = driversList;
                }
                else
                {
                    driverNames.NumberOfRecords = 0;
                }
                return driverNames;
            }
        }

        public CommonResponse GetVehicleTypeNames()
        {
            CommonResponse vehicleTypeNames = new CommonResponse();
            using (var context = new TMSDBContext())
            {
                var vehicleTypeList = context.VehicleTypes.Select(response => new Domain.Common()
                {
                    Id = response.ID,
                    Value = response.VehicleTypeDescription
                }).ToList();
                if (vehicleTypeList != null)
                {
                    vehicleTypeNames.NumberOfRecords = vehicleTypeNames.Data.Count;
                    vehicleTypeNames.Data = vehicleTypeList;
                }
                else
                {
                    vehicleTypeNames.NumberOfRecords = 0;
                }
                return vehicleTypeNames;
            }
        }
    }
}
