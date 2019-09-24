using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BusinessGateway.Classes;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DataGateway.Repositories.Iterfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessPartnerTask : PartnerTask
    {
        private readonly IPartner _partnerRepository;

        public BusinessPartnerTask(IPartner partnerRepository)
        {
            _partnerRepository = partnerRepository;
        }

        public override PartnerResponse CreateUpdatePartner(PartnerRequest partnerRequest)
        {
            int partnerID = partnerRequest.Requests[0].ID;

            PartnerResponse partnerResponse = _partnerRepository.CreateUpdatePartner(partnerRequest);

            if (partnerID == 0)
            {
                #region CreateUpdate Partner in OMS

                LoginRequest loginRequest = new LoginRequest();
                string token = string.Empty;
                loginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                loginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                var OmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                    + "/v1/user/login", Method.POST, loginRequest, null));
                if (OmsLoginResponse != null && OmsLoginResponse.Data.Count > 0)
                {
                    token = OmsLoginResponse.TokenKey;
                }

                PartnerRequest omsPartnerRequest = new PartnerRequest()
                {
                    Requests = new List<DomainObjects.Objects.Partner>()
                {
                    new DomainObjects.Objects.Partner()
                    {
                        PartnerNo = partnerResponse.Data[0].PartnerNo,
                        PartnerName = partnerResponse.Data[0].PartnerName
                    }
                },
                    CreatedBy = partnerRequest.CreatedBy,
                    LastModifiedBy = partnerRequest.LastModifiedBy
                };

                PartnerResponse omsPartnerResponse = JsonConvert.DeserializeObject<PartnerResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                     + "v1/master/createupdatepartner", Method.POST, omsPartnerRequest, token));

                partnerResponse.StatusMessage = partnerResponse.StatusMessage + omsPartnerResponse.StatusMessage;

                #endregion

                #region CreateUpdate Partner in DMS

                LoginRequest dmlLoginRequest = new LoginRequest();
                string dmsToken = string.Empty;
                dmlLoginRequest.UserName = ConfigurationManager.AppSettings["DMSLogin"];
                dmlLoginRequest.UserPassword = ConfigurationManager.AppSettings["DMSPassword"];
                var dmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                    + "/v1/user/login", Method.POST, dmlLoginRequest, null));
                if (dmsLoginResponse != null && dmsLoginResponse.Data.Count > 0)
                {
                    dmsToken = dmsLoginResponse.TokenKey;
                }

                PartnerRequest dmsPartnerRequest = new PartnerRequest()
                {
                    Requests = new List<DomainObjects.Objects.Partner>()
                {
                    new DomainObjects.Objects.Partner()
                    {
                        PartnerNo = partnerResponse.Data[0].PartnerNo,
                        PartnerName = partnerResponse.Data[0].PartnerName
                    }
                },
                    CreatedBy = partnerRequest.CreatedBy,
                    LastModifiedBy = partnerRequest.LastModifiedBy
                };

                PartnerResponse dmsPartnerResponse = JsonConvert.DeserializeObject<PartnerResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayDMSURL"]
                     + "v1/master/createupdatepartner", Method.POST, dmsPartnerRequest, dmsToken));

                partnerResponse.StatusMessage = partnerResponse.StatusMessage + dmsPartnerResponse.StatusMessage;

                #endregion
            }

            return partnerResponse;
        }

        public override PartnerResponse DeletePartner(int partnerId)
        {
            PartnerResponse partnerResponse = _partnerRepository.DeletePartner(partnerId);
            return partnerResponse;
        }

        public override PartnerResponse GetPartners(PartnerRequest partnerRequest)
        {
            PartnerResponse partnerResponse = _partnerRepository.GetPartners(partnerRequest);
            return partnerResponse;
        }
    }
}
