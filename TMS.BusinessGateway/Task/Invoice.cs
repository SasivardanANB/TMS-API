using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TMS.DataGateway.Repositories.Interfaces;
using TMS.DomainGateway.Task;
using TMS.DomainObjects.Objects;
using TMS.DomainObjects.Request;
using TMS.DomainObjects.Response;

namespace TMS.BusinessGateway.Task
{
    public partial class BusinessInvoiceTask : InvoiceTask
    {
        private readonly IInvoice _invoiceRepository;

        public BusinessInvoiceTask(IInvoice invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public override InvoiceResponse GenerateInvoice(InvoiceRequest invoiceRequest)
        {
            InvoiceResponse invoiceResponse = new InvoiceResponse
            {
                Data = new List<Invoice>()
            };

            try
            {
                foreach (var invoice in invoiceRequest.Requests)
                {
                    string headerData = string.Empty; string detailData = string.Empty;
                    //headerData = "HS|27|20190625|H101OMS00000002||5103992943||10000000||1304901930|APPROVED|1||20190625|SYSTEM|||Jangan Dibanting||H101|0005||||0|"; // Sample String
                    headerData = invoice.GeneralPOHeader.GeneralPOHeaderId + "|" +
                    invoice.GeneralPOHeader.DepartementId + "|" +
                    invoice.GeneralPOHeader.OrderDate + "|" +
                    invoice.GeneralPOHeader.OrderNo + "|" +
                    invoice.GeneralPOHeader.LocationId + "|" +
                    invoice.GeneralPOHeader.VendorCode + "|" +
                    invoice.GeneralPOHeader.ReviewerDepartementId + "|" +
                    invoice.GeneralPOHeader.TotalPrice + "|" +
                    invoice.GeneralPOHeader.Currency + "|" +
                    invoice.GeneralPOHeader.Reference + "|" +
                    invoice.GeneralPOHeader.Status + "|" +
                    invoice.GeneralPOHeader.StatusSAP + "|" +
                    invoice.GeneralPOHeader.IsDeleted + "|" +
                    invoice.GeneralPOHeader.CreatedDate + "|" +
                    invoice.GeneralPOHeader.CreatedBy + "|" +
                    invoice.GeneralPOHeader.ModifiedDate + "|" +
                    invoice.GeneralPOHeader.ModifiedBy + "|" +
                    invoice.GeneralPOHeader.Note + "|" +
                    invoice.GeneralPOHeader.IsFromSAP + "|" +
                    invoice.GeneralPOHeader.BusinessArea + "|" +
                    invoice.GeneralPOHeader.CompanyCode + "|" +
                    invoice.GeneralPOHeader.GRNumber + "|" +
                    invoice.GeneralPOHeader.GRDate + "|" +
                    invoice.GeneralPOHeader.GRTime + "|" + "0" + "|";

                    //detailData = "IS||40|20190625|||10000000||27|||||||||||"; //Sample String
                    detailData = invoice.GeneralPODetails.GRNumber + "|" +
                    invoice.GeneralPODetails.MaterialNumber + "|" +
                    invoice.GeneralPODetails.GeneralPODetailId + "|" +
                    invoice.GeneralPODetails.GeneralPOHeaderId + "|" +
                    invoice.GeneralPODetails.OrderDescription + "|" +
                    invoice.GeneralPODetails.Qty + "|" +
                    invoice.GeneralPODetails.DeliveryDate + "|" +
                    invoice.GeneralPODetails.UnitPrice + "|" +
                    invoice.GeneralPODetails.PPN + "|" +
                    invoice.GeneralPODetails.TotalPrice + "|" +
                    invoice.GeneralPODetails.Jenis + "|" +
                    invoice.GeneralPODetails.DepartementId + "|" +
                    invoice.GeneralPODetails.Currency + "|" +
                    invoice.GeneralPODetails.ItemNo + "|" +
                    invoice.GeneralPODetails.IsDeleted + "|" +
                    invoice.GeneralPODetails.CreatedDate + "|" +
                    invoice.GeneralPODetails.CreatedBy + "|" +
                    invoice.GeneralPODetails.ModifiedDate + "|" +
                    invoice.GeneralPODetails.ModifiedBy + "|" +
                    invoice.GeneralPODetails.MaterialDesc + "|";

                    XmlDocument doc = new XmlDocument();

                    //(1) the xml declaration is recommended, but not mandatory
                    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", string.Empty, string.Empty);
                    XmlElement root = doc.DocumentElement;
                    doc.InsertBefore(xmlDeclaration, root);


                    //(2) string.Empty makes cleaner code
                    XmlElement element1 = doc.CreateElement("GeneralPO");
                    element1.SetAttribute("xmlns", ConfigurationManager.AppSettings["B2BElementURL1"]);
                    doc.AppendChild(element1);

                    // Order Node
                    XmlElement element2 = doc.CreateElement("DataHSIS");
                    element1.AppendChild(element2);

                    // Order Header Node
                    XmlElement item1 = doc.CreateElement("string");
                    item1.SetAttribute("xmlns", ConfigurationManager.AppSettings["B2BElementURL2"]);
                    XmlText text1 = doc.CreateTextNode(headerData);
                    item1.AppendChild(text1);
                    element2.AppendChild(item1);
                    // Order Details Node
                    XmlElement item2 = doc.CreateElement("string");
                    item2.SetAttribute("xmlns", ConfigurationManager.AppSettings["B2BElementURL2"]);
                    XmlText text2 = doc.CreateTextNode(detailData);
                    item2.AppendChild(text2);
                    element2.AppendChild(item2);
                    //Password Node
                    XmlElement password = doc.CreateElement("Password");
                    password.InnerText = "password";
                    element1.AppendChild(password);
                    // systemIDField node
                    XmlElement systemIDField = doc.CreateElement("SystemID");
                    systemIDField.InnerText = "S99";
                    element1.AppendChild(systemIDField);
                    //Username node
                    XmlElement username = doc.CreateElement(string.Empty, "Username", string.Empty);
                    username.InnerText = "superservice";
                    element1.AppendChild(username);

                    foreach (XmlNode node in doc)
                    {
                        if (node.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            doc.RemoveChild(node);
                        }
                    }

                    byte[] bytes = Encoding.UTF8.GetBytes(doc.InnerXml.ToString());
                    HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["B2BAPIUrl"]);
                    request1.Method = "POST";
                    request1.ContentLength = bytes.Length;
                    request1.ContentType = "text/xml";
                    using (Stream requestStream = request1.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }

                    HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();

                    invoiceResponse.StatusCode = (int)response1.StatusCode;
                    invoiceResponse.StatusMessage = "Success";
                    if (response1.StatusCode != HttpStatusCode.OK)
                    {
                        string message = String.Format("POST failed. Received HTTP {0}",
                        response1.StatusCode);
                        invoiceResponse.StatusMessage = message;
                    }
                }
                invoiceResponse.Data = invoiceRequest.Requests;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                invoiceResponse.StatusMessage = msg;
                return invoiceResponse;
            }
            return invoiceResponse;
        }
    }
}
