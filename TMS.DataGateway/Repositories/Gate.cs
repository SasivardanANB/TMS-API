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


namespace TMS.DataGateway.Repositories
{
    public class Gate : IGate
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GateResponse CreateGateInGateOut(GateRequest gateRequest)
        {
            GateResponse gateResponse = new GateResponse();
            List<GateInGateOut> gateInGateOuts=new List<GateInGateOut>();
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    foreach (var gateInGateOutData in gateRequest.Requests)
                    {
                        GateInGateOut gateInGateOut = new GateInGateOut()
                        {
                            OrderId = gateInGateOutData.OrderId,
                            Info = gateInGateOutData.Info,
                            GateTypeId = gateInGateOutData.GateTypeId,
                            G2GId = gateInGateOutData.GateId,
                            CreatedTime = DateTime.Now
                        };
                        gateInGateOuts.Add(gateInGateOut);
                    }
                    tMSDBContext.GateInGateOuts.AddRange(gateInGateOuts);
                    tMSDBContext.SaveChanges();
                    gateResponse.Data = gateRequest.Requests;
                    gateResponse.StatusMessage = DomainObjects.Resource.ResourceData.GateInGateOutCreated;
                    gateResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    gateResponse.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                gateResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                gateResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                gateResponse.StatusMessage = ex.Message;
            }
            return gateResponse; 
        }

        public GateResponse GetGateList(GateRequest gateRequest)
        {
            GateResponse gateResponse = new GateResponse();
            List<Domain.Gate> gateList;
            try
            {
                using (var tMSDBContext = new TMSDBContext())
                {
                    //TODO: Need to filter to get orders, by user business area after SAMA integration

                    gateList = tMSDBContext.OrderHeaders.Select(orderHeader => new Domain.Gate
                    {
                        OrderId=orderHeader.ID,
                        VehicleNumber = orderHeader.VehicleNo,
                        OrderType = orderHeader.OrderType == 1 ? "Bongkar" : "Muat",
                        BusinessArea=orderHeader.BusinessArea.BusinessAreaDescription,
                        VehicleTypeName = tMSDBContext.VehicleTypes.Where(v => v.ID.ToString() == orderHeader.VehicleShipment).Select(i => i.VehicleTypeDescription).FirstOrDefault(),
                        ID= tMSDBContext.GateInGateOuts.Any(g => g.OrderId == orderHeader.ID) ? tMSDBContext.GateInGateOuts.Where(g => g.OrderId == orderHeader.ID).Select(i=>i.ID).FirstOrDefault():0,
                        Status = tMSDBContext.GateInGateOuts.Any(g=>g.OrderId==orderHeader.ID)?tMSDBContext.GateTypes.Where(g => g.ID == tMSDBContext.GateInGateOuts.Where(ga => ga.OrderId == orderHeader.ID).OrderByDescending(id=>id.ID).Select(i=>i.GateTypeId).FirstOrDefault()).Select(i=>i.GateTypeDescription).FirstOrDefault() :"NOT ARRIVED",
                        BusinessAreaId= orderHeader.BusinessAreaId,
                        GateName = tMSDBContext.GateInGateOuts.Any(g => g.OrderId == orderHeader.ID) ? (tMSDBContext.G2Gs.Where(g => g.ID == (tMSDBContext.GateInGateOuts.Where(ga => ga.OrderId == orderHeader.ID).OrderByDescending(id => id.ID).Select(i => i.G2GId).FirstOrDefault())).Select(i => i.G2GName).FirstOrDefault()) : "",
                    }).ToList();
                }

                // Filter
                if (gateRequest.Requests.Count > 0)
                {
                    var gateFilter = gateRequest.Requests[0];

                    if (gateFilter.ID > 0)
                    {
                        gateList = gateList.Where(s => s.ID == gateFilter.ID).ToList();
                    }

                    if (!String.IsNullOrEmpty(gateFilter.VehicleTypeName))
                    {
                        gateList = gateList.Where(s => s.VehicleTypeName.Contains(gateFilter.VehicleTypeName)).ToList();
                    }

                    if (!String.IsNullOrEmpty(gateFilter.GateName))
                    {
                        gateList = gateList.Where(s => s.GateName.ToLower().Contains(gateFilter.GateName.ToLower())).ToList();
                    }
                }

                // GLobal Search Filter
                if (!string.IsNullOrEmpty(gateRequest.GlobalSearch))
                {
                    string globalSearch = gateRequest.GlobalSearch;
                    gateList = gateList.Where(s => s.GateName.Contains(globalSearch)).ToList();
                }

                // Total NumberOfRecords
                gateResponse.NumberOfRecords = gateList.Count;

                // Paging
                int pageSize = Convert.ToInt32(gateRequest.PageSize);
                int pageNumber = (gateRequest.PageNumber ?? 1);
                if (pageSize > 0)
                {
                    gateList = gateList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                if (gateList.Count > 0)
                {
                    gateResponse.Data = gateList;
                    gateResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    gateResponse.StatusCode = (int)HttpStatusCode.OK;
                    gateResponse.StatusMessage = DomainObjects.Resource.ResourceData.Success;
                }
                else
                {
                    gateResponse.Status = DomainObjects.Resource.ResourceData.Success;
                    gateResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    gateResponse.StatusMessage = DomainObjects.Resource.ResourceData.NoRecords;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex);
                gateResponse.Status = DomainObjects.Resource.ResourceData.Failure;
                gateResponse.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                gateResponse.StatusMessage = ex.Message;
            }
            return gateResponse;
        }
    }
}
