using Microsoft.Extensions.Azure;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EducationalFundingCo.Utilities
{
    public class SendEmailFromGmail
    {
        public void SendEmail(string toEmail, string toName, string subject, string body, string attachmentPath = null)
        {
            var fromAddress = new MailAddress("support@educationfunding.co", "Educational Funding Co");
            //var fromAddress = new MailAddress("stringbuild1@gmail.com", "Educational Funding Co");
            var toAddress = new MailAddress(toEmail, toName);
            //var copy = new MailAddress("stringbuild1@gmail.com");
            //const string fromPassword = "Trinity@2016";
            var copy = new MailAddress("support@educationfunding.co");
            const string fromPassword = "NjXtPV%L0ON60oAEH!iM";

            var smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)

            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            if (attachmentPath != null)
            {
                Attachment att = new Attachment(attachmentPath);
                message.Attachments.Add(att);
            }

            message.CC.Add(copy);
            message.IsBodyHtml = true;
            smtp.Send(message);

        }

        public string EmailRequest(string email, string title, string body)
        {
            //var senderPassword = "NjXtPV%L0ON60oAEH!iM";
            //var senderEmail = new MailAddress("support@educationfunding.co", "Educational Funding Co");
            var senderPassword = "dnbzknposqxriehv";
            var senderEmail = new MailAddress("activofms2023@gmail.com", "Activo-FMS Alerts");
            var receiverEmail = new MailAddress(email, "Receiver");
            var password = senderPassword;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,

                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                //UseDefaultCredentials = true,

                Credentials = new NetworkCredential(senderEmail.Address, password),
                EnableSsl = true,
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = "Service Request Email",
                Body = body
            })
                try
                {
                    smtp.Send(mess);
                    return "successfully Send";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            //  smtp.UseDefaultCredentials = false;
            return null;
        }
    }
}
