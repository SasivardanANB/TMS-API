using System;
using System.Collections.Generic;
using Quartz;
using System.Net;
using System.Configuration;
using ActiveUp.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Classes
{
    public class EmailJob : IJob
    {
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ReadEmails"]))
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        OrderResponse tmsOrderResponse = new OrderResponse();

                        var mailRepository = new MailRepository(
                                       ConfigurationManager.AppSettings["ImapHost"],
                                       Convert.ToInt32(ConfigurationManager.AppSettings["ImapPort"].ToString()),
                                       true,
                                       ConfigurationManager.AppSettings["ImapUserName"],
                                       ConfigurationManager.AppSettings["ImapUserPassword"]
                                   );

                        //Get all Unread Mails
                        var emailList = mailRepository.GetUnreadMails("inbox");


                        foreach (ActiveUp.Net.Mail.Message email in emailList)
                        {
                            if (email.Attachments.Count > 0)
                            {
                                foreach (MimePart attachment in email.Attachments)
                                {
                                    if (attachment.ContentType.MimeType.ToLower() == "application/pdf") // Checking For PDF files
                                    {
                                        // Uploading File into Blob storage and get GUID
                                        var fileuploadresponse = JsonConvert.DeserializeObject<ResponseDataForFileUpload>(Utility.GetFileUploadApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/media/uploadfile", Method.POST, attachment, null));

                                        if (fileuploadresponse.StatusCode == (int)HttpStatusCode.OK && !String.IsNullOrEmpty(fileuploadresponse.Guid))
                                        {
                                            // Calling OCR to get shipmentschedule data
                                            var res = Utility.GetFileUploadApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"] + "/v1/order/shipmentscheduleocr", Method.POST, attachment, null);
                                            var shipmentScheduleOcr = JsonConvert.DeserializeObject<dynamic>(res);
                                            var jsonObject = JObject.Parse(shipmentScheduleOcr);
                                            ShipmentScheduleOcrRequest shipmentScheduleOcrRequest = new ShipmentScheduleOcrRequest();
                                            if (Convert.ToBoolean(jsonObject.success))
                                            {
                                                shipmentScheduleOcrRequest.Requests = new List<ShipmentScheduleOcr>();
                                                ShipmentScheduleOcr shipmentSchedule = new ShipmentScheduleOcr()
                                                {
                                                    ImageGUID = fileuploadresponse.Guid,
                                                    DocumentType = jsonObject.documentType,
                                                    Success = jsonObject.success,
                                                    Image = jsonObject.image,
                                                    EmailFrom = email.From.Email,
                                                    EmailDateTime = email.Date,
                                                    EmailSubject = email.Subject,
                                                    EmailText = email.BodyHtml.TextStripped,
                                                    Data = new DetailsOcr()
                                                    {
                                                        DayShipment = jsonObject.data.DayShipment,
                                                        EstimatedTotalPallet = jsonObject.data.EstimatedTotalPallet,
                                                        MainDealerCode = jsonObject.data.MainDealerCode,
                                                        MainDealerName = jsonObject.data.MainDealerName,
                                                        MultiDropShipment = jsonObject.data.MultiDropShipment,
                                                        ShipmentScheduleNo = jsonObject.data.ShipmentScheduleNo,
                                                        ShipmentTime = jsonObject.data.ShipmentTime,
                                                        ShipToParty = jsonObject.data.ShipToParty,
                                                        VehicleType = jsonObject.data.VehicleType,
                                                        Weight = jsonObject.data.Weight
                                                    }
                                                };
                                                shipmentScheduleOcrRequest.Requests.Add(shipmentSchedule);
                                            }

                                            if (shipmentScheduleOcrRequest.Requests.Count > 0)
                                            {
                                                //OcrOrderResponse
                                                OrderRequest omsOrderRequest = new OrderRequest();
                                                OrderResponse ocrOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                                                      + "/v1/order/ocrorderresponse", Method.POST, shipmentScheduleOcrRequest, null));
                                                if (ocrOrderResponse != null && ocrOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                                {
                                                    LoginRequest omsLoginRequest = new LoginRequest();
                                                    string omsToken = "";
                                                    omsLoginRequest.UserName = ConfigurationManager.AppSettings["OMSLogin"];
                                                    omsLoginRequest.UserPassword = ConfigurationManager.AppSettings["OMSPassword"];
                                                    var omsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                                                      + "v1/user/login", Method.POST, omsLoginRequest, null));
                                                    if (omsLoginResponse != null && omsLoginResponse.Data.Count > 0)
                                                    {
                                                        omsToken = omsLoginResponse.TokenKey;
                                                    }
                                                    omsOrderRequest.Requests = ocrOrderResponse.Data;

                                                    OrderResponse omsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayOMSURL"]
                                                                                                                         + "v1/order/createordersfromshipmentlistocr", Method.POST, omsOrderRequest, omsToken));

                                                    string processMessage = string.Empty;
                                                    bool isOrderCreated = false;
                                                    if (omsOrderResponse.Data != null && omsOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                                    {
                                                        processMessage = omsOrderResponse.StatusMessage;

                                                        LoginRequest tmsLoginRequest = new LoginRequest();
                                                        string tmsToken = "";
                                                        tmsLoginRequest.UserName = ConfigurationManager.AppSettings["TMSLogin"];
                                                        tmsLoginRequest.UserPassword = ConfigurationManager.AppSettings["TMSPassword"];
                                                        var tmsLoginResponse = JsonConvert.DeserializeObject<UserResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                                                      + "v1/user/login", Method.POST, tmsLoginRequest, null));
                                                        if (tmsLoginResponse != null && tmsLoginResponse.Data.Count > 0)
                                                        {
                                                            tmsToken = tmsLoginResponse.TokenKey;

                                                            OrderRequest tmsOrderRequest = new OrderRequest();
                                                            tmsOrderRequest.Requests = omsOrderResponse.Data;
                                                            tmsOrderRequest.UploadType = 3;
                                                            tmsOrderResponse = JsonConvert.DeserializeObject<OrderResponse>(Utility.GetApiResponse(ConfigurationManager.AppSettings["ApiGatewayTMSURL"]
                                                                                                                                                                   + "v1/order/createordersfromshipmentlistocr", Method.POST, tmsOrderRequest, tmsToken));
                                                            if(tmsOrderResponse!=null && tmsOrderResponse.StatusCode == (int)HttpStatusCode.OK)
                                                            {
                                                                processMessage += tmsOrderResponse.StatusMessage;
                                                                isOrderCreated = true;
                                                            }
                                                            else
                                                            {
                                                                processMessage += tmsOrderResponse.StatusMessage;
                                                            }
                                                        }                                                       
                                                    }
                                                    else
                                                    {
                                                        processMessage += omsOrderResponse.StatusMessage;
                                                    }
                                                    // Update shipment schedule ocr details 
                                                    // processMessage & IsOrderCreated
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                    }
                });
            }
        }
    }
}
