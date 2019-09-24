using AutoMapper;
using OMS.DataGateway.Repositories.Interfaces;
using OMS.DomainObjects.Request;
using OMS.DomainObjects.Response;
using System;
using System.Net;
using Domain = OMS.DomainObjects.Objects;
using DataModel = OMS.DataGateway.DataModels;
using System.Collections.Generic;
using OMS.DataGateway.DataModels;
using System.Linq;

namespace OMS.DataGateway.Repositories
{
    public class Master : IMaster
    {
        public PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = new PartnerResponse();

            try
            {
                using (var context = new OMSDBContext())
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
                            partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerUpdated;
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
                            partnerResponse.StatusMessage = DomainObjects.Resource.ResourceData.PartnerCreated;
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
