using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authentication;
using EducationalFundingCo.Utilities;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EducationalFundingCo.Pages.AcademyProgram
{
    [Authorize(Roles = "Administrator, SchoolAdministrator")]
    public class IndexModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(EducationalFundingCoContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IList<Areas.Identity.Data.AcademyProgram> AcademyProgram { get;set; }

        [BindProperty]
        public EmailElements EmailElements { get; set; }

        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }
        [BindProperty]
        public string SchoolName { get; set; }

        public string UserId { get; set; }

        public async Task OnGetAsync() 
        { 

            int? SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));

            if (SchoolId == 0)
            {
                UserId = _userManager.GetUserId(User);

                SchoolId = await _context.ApplicationUser
                  .Where(x => x.IdentityUserId == UserId).Select(x => x.SchoolId).FirstOrDefaultAsync();
            }

            if (SchoolId != 0)
            {
                AcademyProgram = await Task.Run(() => _context.AcademyPrograms.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToListAsync());
            }
            else if (SchoolId == 0)
            {
                AcademyProgram = await Task.Run(() => _context.AcademyPrograms.Include(x => x.School).Where(x => x.School.RecordStatus == 2).ToListAsync());
            }


        }

        public IActionResult OnGetAjaxEmailContent(int programID , int SchoolId)
        {
            AcademyProgram = _context.AcademyPrograms.ToList();

            if (programID == 0 && SchoolId == 0)
            {
                return Page();
            }

            return OnBoardingStudent(programID, SchoolId);
        }

        private IActionResult OnBoardingStudent(int programID, int SchoolId)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());


            byte[] bytes = BitConverter.GetBytes(SchoolId);
            string base64String = Convert.ToBase64String(bytes);

            var callbackUrl = Url.Page(
                       "/Account/Register",
                       pageHandler: null,
                       //values: new { area = "Identity", Id = programID, code = token },
                       values: new { area = "Identity", Id = programID, code = token, SchoolId = base64String },
                       protocol: Request.Scheme);

            string emailBody = $"Hello, {Environment.NewLine}Please join our program by <a href={ HtmlEncoder.Default.Encode(callbackUrl)}>clicking here</a>. {Environment.NewLine} {Environment.NewLine} Thanks, {Environment.NewLine} Educational Funding, Co.";
            string justLink = callbackUrl;

            EmailElements emailElements = new EmailElements
            {
                Subject = "Education Funding Invite",
                Body = emailBody,
                Link = justLink
            };

            return new JsonResult(emailElements);
        }

        public async Task<IActionResult> OnPostUpdateEmployment()
        {
            EmailElements.Body = EmailElements.Body.Replace("\r\n", "<br/>");

            //SendEmailFromGmail sfgmail = new SendEmailFromGmail();
            //sfgmail.SendEmail(EmailElements.ToEmail, "Registrant", EmailElements.Subject,
            //        string.Format(EmailElements.Body));


            EmailSender emailSender = new EmailSender
            {
                Subject = EmailElements.Subject,
                ToEmail = EmailElements.ToEmail,
                ToName = "Registrant",
                Content = EmailElements.Body
            };

            await emailSender.SendEmailAsync();

            ModelState.Clear();
            AcademyProgram = _context.AcademyPrograms.ToList();
            //EmailElements emailElements = new EmailElements
            //{
            //    Subject = "Education Funding Invite",
            //    Body = "",
            //    Link = ""
            //};

            return RedirectToPage();
        }

    }

    public class EmailElements
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
    }
}
