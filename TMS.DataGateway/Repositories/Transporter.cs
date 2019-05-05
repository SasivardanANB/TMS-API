using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;
using Domain = TMS.DomainObjects.Objects;
using DataModel = TMS.DataGateway.DataModels;
using TMS.DataGateway.Repositories.Iterfaces;

namespace TMS.DataGateway.Repositories
{
    public class Transporter : ITransporter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TransporterResponse CreateUpdateTransporter(TransporterRequest transporterRequest)
        {
            TransporterResponse transporterResponse = new TransporterResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Transporter, DataModel.Expeditor>().ReverseMap();
                    });
                    //    .ForMember(x => x.RoleName, opt => opt.Ignore())
                    //    .ForMember(x => x.BusinessAreaName, opt => opt.Ignore())
                    //    .ForMember(x => x.ApplicationNames, opt => opt.Ignore());
                    //});

                    IMapper mapper = config.CreateMapper();
                    var expeditors = mapper.Map<List<Domain.Transporter>, List<DataModel.Expeditor>>(transporterRequest.Requests);
                    foreach (var expeditorData in expeditors)
                    {
                        if (expeditorData.ID > 0) //Update User
                        {
                            context.Entry(expeditorData).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            transporterResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersUpdated;
                        }
                        else //Create User
                        {
                            context.Expeditors.Add(expeditorData);
                            context.SaveChanges();
                            transporterResponse.StatusMessage = DomainObjects.Resource.ResourceData.UsersCreated;
                        }
                    }

                    transporterRequest.Requests = mapper.Map<List<DataModel.Expeditor>, List<Domain.Transporter>>(expeditors);
                    transporterResponse.Data = transporterRequest.Requests;
                    transporterResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    transporterResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                transporterResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                transporterResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                transporterResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return transporterResponse;
        }

        public TransporterResponse DeleteTransporter(int transporterId)
        {
            TransporterResponse transporterResponse = new TransporterResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var transporterData =
                        (from transporter in context.Expeditors
                         where transporter.ID == transporterId
                         select new Domain.Transporter
                         {
                             ID = transporter.ID,
                             Initial = transporter.Initial,
                             ExpeditorName = transporter.ExpeditorName,
                             ExpeditorEmail = transporter.ExpeditorEmail,
                             Address = transporter.Address,
                             ExpeditorTypeID = transporter.ExpeditorTypeID,
                             PostalCodeID = transporter.PostalCodeID
                         }).FirstOrDefault();
                    if (transporterData != null)
                    {
                        transporterResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        transporterResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        transporterResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        transporterResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                transporterResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                transporterResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                transporterResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return transporterResponse;
        }

        public TransporterResponse GetTransporters(TransporterRequest transporterRequest)
        {
            TransporterResponse transporterResponse = new TransporterResponse();
            List<Domain.Transporter> transporterList = new List<Domain.Transporter>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    transporterList =
                        (from transporter in context.Expeditors
                         select new Domain.Transporter
                         {
                             ID = transporter.ID,
                             Initial = transporter.Initial,
                             ExpeditorName = transporter.ExpeditorName,
                             ExpeditorEmail = transporter.ExpeditorEmail,
                             Address = transporter.Address,
                             ExpeditorTypeID = transporter.ExpeditorTypeID,
                             PostalCodeID = transporter.PostalCodeID
                         }).ToList();
                }
                // Filter
                if (transporterRequest.Requests.Count > 0)
                {
                    var transporterFilter = transporterRequest.Requests[0];

                    if (transporterFilter.ID > 0)
                    {
                        transporterList = transporterList.Where(s => s.ID == transporterFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(transporterFilter.ExpeditorName))
                    {
                        transporterList = transporterList.Where(s => s.ExpeditorName.Contains(transporterFilter.ExpeditorName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(transporterFilter.ExpeditorEmail))
                    {
                        transporterList = transporterList.Where(s => s.ExpeditorEmail.Contains(transporterFilter.ExpeditorEmail)).ToList();
                    }

                    if (transporterFilter.ExpeditorTypeID > 0)
                    {
                        transporterList = transporterList.Where(s => s.ExpeditorTypeID == transporterFilter.ExpeditorTypeID).ToList();
                    }

                    if (transporterFilter.PostalCodeID > 0)
                    {
                        transporterList = transporterList.Where(s => s.PostalCodeID == transporterFilter.PostalCodeID).ToList();
                    }
                }

                // Sorting
                switch (transporterRequest.SortOrder.ToLower())
                {
                    case "expeditorname":
                        transporterList = transporterList.OrderBy(s => s.ExpeditorName).ToList();
                        break;
                    case "expeditorname_desc":
                        transporterList = transporterList.OrderByDescending(s => s.ExpeditorName).ToList();
                        break;
                    case "ExpeditorEmail":
                        transporterList = transporterList.OrderBy(s => s.ExpeditorEmail).ToList();
                        break;
                    case "ExpeditorEmail_Desc":
                        transporterList = transporterList.OrderByDescending(s => s.ExpeditorEmail).ToList();
                        break;
                    case "Initial":
                        transporterList = transporterList.OrderBy(s => s.Initial).ToList();
                        break;
                    case "Initial_Desc":
                        transporterList = transporterList.OrderByDescending(s => s.Initial).ToList();
                        break;
                    default:  // ID Descending 
                        transporterList = transporterList.OrderByDescending(s => s.ID).ToList();
                        break;
                }

                // Total NumberOfRecords
                transporterResponse.NumberOfRecords = transporterList.Count;

                // Paging
                int pageNumber = (transporterRequest.PageNumber ?? 1);
                int pageSize = Convert.ToInt32(transporterRequest.PageSize);
                transporterList = transporterList.Skip(pageNumber - 1 * pageSize).Take(pageSize).ToList();

                if (transporterList.Count > 0)
                {
                    transporterResponse.Data = transporterList;
                    transporterResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    transporterResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    transporterResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    transporterResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                transporterResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                transporterResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                transporterResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return transporterResponse;
        }
    }
}
