using AutoMapper;
using DMS.DataGateway.DataModels;
using DMS.DataGateway.Repositories.Interfaces;
using DMS.DomainObjects.Request;
using DMS.DomainObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain = DMS.DomainObjects.Objects;
using DataModel = DMS.DataGateway.DataModels;
using System.Net;

namespace DMS.DataGateway.Repositories
{
    public class Master : IMaster
    {
        public PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = new PartnerResponse();

            try
            {
                using (var context = new DMSDBContext())
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Domain.Partner, DataModel.Partner>().ReverseMap();
                    });

                    IMapper mapper = config.CreateMapper();
                    var partners = mapper.Map<List<Domain.Partner>, List<DataModel.Partner>>(partnerRequest.Requests);
                    foreach (var partnerData in partners)
                    {
                        partnerData.IsActive = true;
                        if (partnerData.ID > 0) //Update partner
                        {
                            partnerData.LastModifiedBy = partnerRequest.LastModifiedBy;
                            partnerData.LastModifiedTime = DateTime.Now;
                            context.Entry(partnerData).State = System.Data.Entity.EntityState.Modified;
                            context.Entry(partnerData).Property(p => p.PartnerNo).IsModified = false;
                            context.SaveChanges();

                            partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else //Create partner
                        {
                            partnerData.CreatedBy = partnerRequest.CreatedBy;
                            partnerData.CreatedTime = DateTime.Now;

                            context.Partners.Add(partnerData);
                            context.SaveChanges();

                            DataModel.PartnerPartnerType partnerPartnerType = new PartnerPartnerType();
                            partnerPartnerType.PartnerId = partnerData.ID;
                            partnerPartnerType.PartnerTypeId = context.PartnerTypes.FirstOrDefault(t => t.PartnerTypeCode == "1").ID;
                            context.PartnerPartnerTypes.Add(partnerPartnerType);
                            context.SaveChanges();

                            partnerResponse.Status = DomainObjects.Resource.ResourceData.Success;
                            partnerResponse.StatusCode = (int)HttpStatusCode.OK;
                        }
                    }

                    partnerRequest.Requests = mapper.Map<List<DataModel.Partner>, List<Domain.Partner>>(partners);
                    partnerResponse.Data = partnerRequest.Requests;

                }
            }
            catch (Exception ex)
            {
                partnerResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                partnerResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                partnerResponse.StatusMessage = ex.Message;
            }

            return partnerResponse;
        }
    }
}
