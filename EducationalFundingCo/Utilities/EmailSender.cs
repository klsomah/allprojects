using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Utilities
{
    public class EmailSender
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public async Task<bool> SendEmailAsync()
        {
            try
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("support@educationfunding.co", "CoinGroup"),
                    Subject = Subject,
                    //PlainTextContent = Content,
                    HtmlContent = Content
                };
                msg.AddTo(new EmailAddress(ToEmail, ToName));
                var response = await client.SendEmailAsync(msg);

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                var yy = e.Message;
                return false;
            }
        }

    }
}
