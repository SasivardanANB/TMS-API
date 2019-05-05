
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Helper.Model.Common;

namespace Helper.Model.Common
{
    public class EmailHelper
    {

        private MailMessage _mailMessage = null;
        private SmtpClient _smtpClient = null;

        #region Constructors

        public EmailHelper()
        {
            _mailMessage = new MailMessage();
            // _smtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"]);
            _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPSERVER"].ToString());
        }

        public EmailHelper(string mailFrom, string mailTo)
        {
            _mailMessage = new MailMessage(mailFrom, mailTo);
            //   _smtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"]);
            _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPSERVER"].ToString());
        }

        public EmailHelper(string mailFrom, string mailTo, string subject, string body)
        {
            _mailMessage = new MailMessage(mailFrom, mailTo, subject, body);
            // _smtpClient = new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"]);
            _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPSERVER"].ToString());
        }

        #endregion

        #region Properties

        #region Attachments

        public AttachmentCollection Attachments
        {
            get { return _mailMessage.Attachments; }
        }

        #endregion

        #region Bcc

        public string Bcc
        {
            set
            {
                _mailMessage.Bcc.Clear();
                _mailMessage.Bcc.Add(value.Normalize());
            }
            get
            {
                if (_mailMessage.Bcc.Count == 1)
                {
                    return _mailMessage.Bcc[0].Address;
                }
                return string.Empty;
            }
        }

        #endregion

        #region Body

        public string Body
        {
            set { _mailMessage.Body = value; }
            get { return _mailMessage.Body; }
        }

        #endregion

        #region From

        public string From
        {
            set { _mailMessage.From = new MailAddress(value.Normalize()); }
            get { return _mailMessage.From.Address; }
        }

        #endregion

        #region FromDisplayName

        public string FromDisplayName
        {
            set { _mailMessage.From = new MailAddress(_mailMessage.From.Address, value, Encoding.UTF8); }
            get { return _mailMessage.From.DisplayName + " [" + _mailMessage.From.Address + "]"; }
        }

        #endregion

        #region Sender

        public string Sender
        {
            set { _mailMessage.Sender = new MailAddress(value.Normalize()); }
            get { return _mailMessage.Sender.Address; }
        }

        #endregion

        #region IsBodyHtml

        public bool IsBodyHtml
        {
            set { _mailMessage.IsBodyHtml = value; }
            get { return _mailMessage.IsBodyHtml; }
        }

        #endregion

        #region Priority

        public MailPriority Priority
        {
            set { _mailMessage.Priority = value; }
            get { return _mailMessage.Priority; }
        }

        #endregion

        #region ReadReceiptRequired

        private bool _readReceiptRequired = false;

        public bool ReadReceiptRequired
        {
            set { _readReceiptRequired = value; }
            get { return _readReceiptRequired; }
        }

        #endregion

        #region Subject

        public string Subject
        {
            set { _mailMessage.Subject = value; }
            get { return _mailMessage.Subject; }
        }

        #endregion

        #region To

        public string To
        {
            set
            {
                _mailMessage.To.Clear();
                _mailMessage.To.Add(value);
            }
            get
            {
                if (_mailMessage.To.Count == 1)
                {
                    return _mailMessage.To[0].Address;
                }
                return string.Empty;
            }
        }

        #endregion
        public bool EnableSsl { set; get; }
        public int Port { set; get; }

        #endregion

        #region Send

        public void Send()
        {

            if (_readReceiptRequired)
            {
                _mailMessage.Headers.Add("Disposition-Notification-To", _mailMessage.From.Address);
            }
            try
            {
                //_mailMessage.HeadersEncoding = Encoding.UTF8;
                _mailMessage.SubjectEncoding = Encoding.UTF8;
                _mailMessage.BodyEncoding = Encoding.UTF8;
                _smtpClient.Port = Port;
                _smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                _smtpClient.UseDefaultCredentials = false;
                _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                _smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                _smtpClient.Send(_mailMessage);
                _mailMessage.Dispose();
                _smtpClient.Dispose();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion
    }
}
