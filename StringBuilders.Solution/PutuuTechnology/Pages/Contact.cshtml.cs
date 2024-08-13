using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PutuuTechnology.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PutuuTechnology.Pages
{
    public class InputModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

       
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
 

    }
    public class ContactModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }
        public void OnGet()
        {
        }
        public async Task OnPostAsync()
        {
            bool message1 = await EmailForPutuu();
            bool message2 = await EmailForContact();

            if (message1 && message2)
            {
                ViewData["Message"] = string.Format($"Your message has been send. We will contact you within 24 hours. Innovating Today for a Better Tomorrow. StringBuilders LLC.");
            }
            else
            {
                ViewData["Message"] = string.Format($"Your message has not been send. We are experiencing technical difficulties.");
            }
        }

        private async Task<bool> EmailForPutuu()
        {
            var emailBody = $"Message from {Input.FirstName} {Input.LastName}.  <br/><br/> Contact Information: Phone: {Input.PhoneNumber}<br/> Email: {Input.Email}  <br/><br/>Message:<br/> {Input.Message}";
            EmailSender emailSender = new EmailSender
            {
                ToEmail = "info@putuutech.com",
                ToName = "Putuu Technology",
                Subject = "Message from website",
                Content = emailBody
            };

            return await emailSender.SendEmailAsync();
        }

        private async Task<bool> EmailForContact()
        {
            var emailBody = $"Dear {Input.FirstName}, <br/><br/> Thank you for contacting us. We will be in touch soon. <br/><br/>Innovating Today for a Better Tomorrow,<br/>Putuu Technology LLC. <br/><br/>";
            EmailSender emailSender = new EmailSender
            {
                ToEmail = Input.Email,
                ToName = $"{Input.FirstName} {Input.LastName}",
                Subject = "Putuu Technology",
                Content = emailBody
            };

            return await emailSender.SendEmailAsync();
        }
    }


}
