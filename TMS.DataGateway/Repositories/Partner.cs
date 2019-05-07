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
    public class Partner : IPartner
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = new PartnerResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Partner, DataModel.Partner>().ReverseMap();
                    });
                    //    .ForMember(x => x.RoleName, opt => opt.Ignore())
                    //    .ForMember(x => x.BusinessAreaName, opt => opt.Ignore())
                    //    .ForMember(x => x.ApplicationNames, opt => opt.Ignore());
                    //});

                    IMapper mapper = config.CreateMapper();
                    var partners = mapper.Map<List<Domain.Partner>, List<DataModel.Partner>>(partnerRequest.Requests);
                    foreach (var partnerData in partners)
                    {
                        partnerData.IsDeleted = false;
                        if (partnerData.ID > 0) //Update User
                        {
                            partnerData.LastModifiedBy = partnerRequest.LastModifiedBy;
                            partnerData.LastModifiedTime = DateTime.Now;
                            context.Entry(partnerData).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerUpdated;
                        }
                        else //Create User
                        {
                            partnerData.CreatedBy = partnerRequest.LastModifiedBy;
                            partnerData.CreatedTime = DateTime.Now;
                            context.Partners.Add(partnerData);
                            context.SaveChanges();
                            partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerCreated;
                        }
                    }

                    partnerRequest.Requests = mapper.Map<List<DataModel.Partner>, List<Domain.Partner>>(partners);
                    partnerResponse.Data = partnerRequest.Requests;
                    partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return partnerResponse;
        }

        public PartnerResponse DeletePartner(int partnerId)
        {
            PartnerResponse partnerResponse = new PartnerResponse();
            try
            {
                using (var context = new TMSDBContext())
                {
                    var partnerData = context.Partners.Where(partner => partner.ID == partnerId).FirstOrDefault();
                    if (partnerData != null)
                    {
                        partnerData.IsDeleted = true;
                        context.SaveChanges();
                        partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                        partnerResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return partnerResponse;
        }

        public PartnerResponse GetPartners(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = new PartnerResponse();
            List<Domain.Partner> partnerList = new List<Domain.Partner>();
            try
            {
                using (var context = new TMSDBContext())
                {
                    partnerList =
                        (from partner in context.Partners
                         select new Domain.Partner
                         {
                             ID = partner.ID,
                             PartnerInitial = partner.PartnerInitial,
                             PartnerName = partner.PartnerName,
                             PartnerEmail = partner.PartnerEmail,
                             PartnerAddress = partner.PartnerAddress,
                             OrderPointCode = partner.OrderPointCode,
                             OrderPointTypeID = partner.OrderPointTypeID,
                             PostalCodeID = partner.PostalCodeID,
                             PartnerNo = partner.PartnerNo,
                             PartnerTypeID = partner.PartnerTypeID,
                             PICID = partner.PICID,
                             PICName = partner.PIC.PICName
                         }).ToList();
                }
                // Filter
                if (partnerRequest.Requests.Count > 0)
                {
                    var partnerFilter = partnerRequest.Requests[0];

                    if (partnerFilter.ID > 0)
                    {
                        partnerList = partnerList.Where(s => s.ID == partnerFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(partnerFilter.PartnerName))
                    {
                        partnerList = partnerList.Where(s => s.PartnerName.Contains(partnerFilter.PartnerName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(partnerFilter.PartnerEmail))
                    {
                        partnerList = partnerList.Where(s => s.PartnerEmail.Contains(partnerFilter.PartnerEmail)).ToList();
                    }

                    if (partnerFilter.PartnerTypeID > 0)
                    {
                        partnerList = partnerList.Where(s => s.PartnerTypeID == partnerFilter.PartnerTypeID).ToList();
                    }

                    if (partnerFilter.PostalCodeID > 0)
                    {
                        partnerList = partnerList.Where(s => s.PostalCodeID == partnerFilter.PostalCodeID).ToList();
                    }
                }

                //Sorting
                switch (partnerRequest.SortOrder.ToLower())
                {
                    case "partnername":
                        partnerList = partnerList.OrderBy(s => s.PartnerName).ToList();
                        break;
                    case "partnername_desc":
                        partnerList = partnerList.OrderByDescending(s => s.PartnerName).ToList();
                        break;
                    case "partneraddress":
                        partnerList = partnerList.OrderBy(s => s.PartnerAddress).ToList();
                        break;
                    case "partneraddress_desc":
                        partnerList = partnerList.OrderByDescending(s => s.PartnerAddress).ToList();
                        break;
                    case "picname":
                        partnerList = partnerList.OrderBy(s => s.PICName).ToList();
                        break;
                    case "picname_desc":
                        partnerList = partnerList.OrderByDescending(s => s.PICName).ToList();
                        break;
                    case "partneremail":
                        partnerList = partnerList.OrderBy(s => s.PartnerEmail).ToList();
                        break;
                    case "partneremail_desc":
                        partnerList = partnerList.OrderByDescending(s => s.PartnerEmail).ToList();
                        break;
                    case "initial":
                        partnerList = partnerList.OrderBy(s => s.PartnerInitial).ToList();
                        break;
                    case "initial_desc":
                        partnerList = partnerList.OrderByDescending(s => s.PartnerInitial).ToList();
                        break;
                    default:  // ID Descending 
                        partnerList = partnerList.OrderByDescending(s => s.ID).ToList();
                        break;
                }

                // Total NumberOfRecords
                partnerResponse.NumberOfRecords = partnerList.Count;

                // Paging
                int pageNumber = (partnerRequest.PageNumber ?? 1);
                int pageSize = Convert.ToInt32(partnerRequest.PageSize);
                if (pageSize > 0)
                {
                    partnerList = partnerList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                if (partnerList.Count > 0)
                {
                    partnerResponse.Data = partnerList;
                    partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                    partnerResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);

                partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.DataBaseException;
            }
            return partnerResponse;
        }
    }
}
