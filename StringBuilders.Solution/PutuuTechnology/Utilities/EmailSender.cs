using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace PutuuTechnology.Utilities
{
    public class EmailSender
    {
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public string? ToEmail { get; set; }
        public string? ToName { get; set; }
        public async Task<bool> SendEmailAsync()
        {
            try
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("info@putuutech.com", "Putuu Technology"),
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
