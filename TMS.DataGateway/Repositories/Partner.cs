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
                        if (partnerData.ID > 0) //Update partner
                        {
                            partnerData.LastModifiedBy = partnerRequest.LastModifiedBy;
                            partnerData.LastModifiedTime = DateTime.Now;
                            context.Entry(partnerData).State = System.Data.Entity.EntityState.Modified;
                            context.Entry(partnerData).Property(p => p.PartnerNo).IsModified = false;
                            context.SaveChanges();
                            partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerUpdated;
                            partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else //Create partner
                        {
                            //var existPartner = context.Partners.Where(p => p.PartnerNo == partnerData.PartnerNo).FirstOrDefault();
                            //if (existPartner == null)
                            //{
                            partnerData.CreatedBy = partnerRequest.CreatedBy;
                            partnerData.CreatedTime = DateTime.Now;
                            partnerData.PartnerNo = GetPartnerNumber();
                            partnerData.IsActive = true;

                            context.Partners.Add(partnerData);
                            context.SaveChanges();
                            DataModel.PartnerPartnerType partnerPartnerType = new PartnerPartnerType();
                            partnerPartnerType.PartnerId = partnerData.ID;
                            partnerPartnerType.PartnerTypeId = context.PartnerTypes.FirstOrDefault(t=>t.PartnerTypeCode == "1").ID;
                            context.PartnerPartnerTypes.Add(partnerPartnerType);
                            context.SaveChanges();
                            partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerCreated;
                            partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                            //}
                            //else
                            //{
                            //    partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerNoExisted;
                            //    partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            //    partnerResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                            //}
                        }
                    }

                    partnerRequest.Requests = mapper.Map<List<DataModel.Partner>, List<Domain.Partner>>(partners);
                    partnerResponse.Data = partnerRequest.Requests;

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
                        //Need to assign lastmodifiedby using session userid
                        partnerData.LastModifiedTime = DateTime.Now;
                        partnerData.IsDeleted = true;
                        context.SaveChanges();
                        partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                        partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
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
                         join ppt in context.PartnerPartnerTypes on partner.ID equals ppt.PartnerId
                         join subdistrict in context.SubDistricts on partner.SubDistrictID equals subdistrict.ID
                         join city in context.Cities on subdistrict.CityID equals city.ID
                         where !partner.IsDeleted
                         select new Domain.Partner
                         {
                             ID = partner.ID,
                             PartnerInitial = partner.PartnerInitial,
                             PartnerName = partner.PartnerName,
                             PartnerEmail = partner.PartnerEmail,
                             PartnerAddress = partner.PartnerAddress,
                             OrderPointCode = partner.OrderPointCode,
                             OrderPointTypeID = partner.OrderPointTypeID,
                             SubDistrictID = subdistrict.ID,
                             PartnerNo = partner.PartnerNo,
                             PartnerTypeID = ppt.PartnerTypeId,
                             PICID = partner.PICID,
                             PICName = partner.PIC.PICName,
                             PICPhone=partner.PIC.PICPhone,
                             CityCode = city.CityDescription
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
                        partnerList = partnerList.Where(s => s.PartnerName.ToLower().Contains(partnerFilter.PartnerName.ToLower())).ToList();
                    }

                    if (!String.IsNullOrEmpty(partnerFilter.PartnerEmail))
                    {
                        partnerList = partnerList.Where(s => s.PartnerEmail.ToLower().Contains(partnerFilter.PartnerEmail.ToLower())).ToList();
                    }

                    if (partnerFilter.PartnerTypeID > 0)
                    {
                        partnerList = partnerList.Where(s => s.PartnerTypeID == partnerFilter.PartnerTypeID).ToList();
                    }

                    if (partnerFilter.SubDistrictID > 0)
                    {
                        partnerList = partnerList.Where(s => s.SubDistrictID == partnerFilter.SubDistrictID).ToList();
                    }
                }

                // GLobal Search Filter
                if (!string.IsNullOrEmpty(partnerRequest.GlobalSearch))
                {
                    string globalSearch = partnerRequest.GlobalSearch.ToLower();
                    partnerList = partnerList.Where(s => !s.IsDeleted && (s.PartnerName != null && s.PartnerName.ToLower().Contains(globalSearch))
                    || (s.PartnerAddress != null && s.PartnerAddress.ToLower().Contains(globalSearch))
                    || (s.PartnerInitial != null && s.PartnerInitial.ToLower().Contains(globalSearch))
                    || (s.PICName != null && s.PICName.ToLower().Contains(globalSearch))
                    ).ToList();
                }

                if (partnerList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(partnerRequest.SortOrder))
                    {
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
                    }
                    else
                    {
                        partnerList = partnerList.OrderByDescending(s => s.ID).ToList();
                    }
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
                    partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    partnerResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
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
        //SHIP01
        private string GetPartnerNumber()
        {
            string partnerNumber = string.Empty;
            using (var tMSDBContext = new TMSDBContext())
            {
                var partnerDetails = tMSDBContext.Partners.OrderByDescending(t => t.ID).FirstOrDefault();
                if (partnerDetails != null)
                {
                    Int64 lastPartnerNo = Convert.ToInt64(partnerDetails.PartnerNo.Substring(4, partnerDetails.PartnerNo.Length - 4));
                    if (lastPartnerNo > 0)
                    {
                        lastPartnerNo = lastPartnerNo + 1;
                    }
                    else
                    {
                        lastPartnerNo = 1;
                    }
                    string finalNo = lastPartnerNo.ToString().PadLeft(1, '0');
                    partnerNumber = "SHIP" + finalNo;
                }
                else
                {
                    partnerNumber = "SHIP01";
                }
            }
            return partnerNumber;
        }
    }
}
