using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EducationalFundingCo.Utilities;
using EducationalFundingCo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using EducationalFundingCo.Areas.Identity.Data;

namespace EducationalFundingCo.Pages.School
{
    [AllowAnonymous]
    public class SchoolOnboardingModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;


        public SchoolOnboardingModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["USAState"] = new SelectList(_context.USState, "Id", "Name");
            ViewData["Choices"] = new MultiSelectList(_context.LearningSolution, "Id", "Name", new List<string> { });
             
            return Page();
        }

        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }
        [BindProperty]
        public string isBasedUSA { get; set; }
        [BindProperty]
        public string isProspectiveStudent { get; set; }
        [BindProperty]
        public string isCPSBasedInUS { get; set; }
        [BindProperty]
        public string sol1Cps { get; set; }
        [BindProperty]
        public string sol2Cps { get; set; }
        [BindProperty]
        public string sol3Cps { get; set; }
        [BindProperty]
        public List<string> choices { get; set; }
        [BindProperty]
        public List<string> choicesCPS { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.

        public async Task<IActionResult> OnPostAsync()
        {
            
            School.CreatedOn = DateTime.Now;
            School.RecordStatus = 1;
            School.Status = "Pending";
            if (isBasedUSA == ("0"))
                School.IsBasedInUS = false;
            else
                School.IsBasedInUS = true;
            if (isProspectiveStudent == ("0"))
                School.IsProspectiveStudent = false;
            else
                School.IsProspectiveStudent = true;
            if (isCPSBasedInUS == ("0"))
                School.IsCPSBasedInUS = false;
            else
                School.IsCPSBasedInUS = true;
            if (School.State != null)
                School.USStateId = null;
            _context.School.Add(School);
            await _context.SaveChangesAsync();

            if (choices.Count > 0)
            {
                var getRecord = await Task.Run(() => _context.School.Where(x => x.Email == School.Email && x.FirstName == School.FirstName && x.LastName == School.LastName).OrderByDescending(x=>x.Id).Select(x => x.Id).FirstOrDefault());
                List<SchoolLearningSolution> SchoolLearningSolution = new List<SchoolLearningSolution>();
                foreach (var item in choices)
                {
                    SchoolLearningSolution learningSolution = new SchoolLearningSolution();
                    learningSolution.CreatedOn = DateTime.Now;
                    learningSolution.SchoolId = getRecord;
                    learningSolution.LearningSolutionId = Convert.ToInt32(item);
                    SchoolLearningSolution.Add(learningSolution);
                }
                await _context.SchoolLearningSolution.AddRangeAsync(SchoolLearningSolution);
                await _context.SaveChangesAsync();
            }
            if (choicesCPS.Count > 0)
            {
                var getRecord = await Task.Run(() => _context.School.Where(x => x.Email == School.Email && x.FirstName == School.FirstName && x.LastName == School.LastName).OrderByDescending(x=>x.Id).Select(x => x.Id).FirstOrDefault());
                List<SchoolLearningSolution> SchoolLearningSolution = new List<SchoolLearningSolution>();
                foreach (var item in choicesCPS)
                {
                    SchoolLearningSolution learningSolution = new SchoolLearningSolution();
                    learningSolution.CreatedOn = DateTime.Now;
                    learningSolution.SchoolId = getRecord;
                    learningSolution.LearningSolutionId = Convert.ToInt32(item);
                    SchoolLearningSolution.Add(learningSolution);
                }
                await _context.SchoolLearningSolution.AddRangeAsync(SchoolLearningSolution);
                await _context.SaveChangesAsync();
            }
            //return Redirect("Areas/Identity/Pages/Account/Login");
            return Redirect(HttpContext.Request.Host.Value + "/Identity/Account/Login");

            //return RedirectToPage("../Identity/Account/Login");
            //return RedirectToPage("/Account/Login");
            //return RedirectToPage("./Identity/Pages/Account/Login");
            //return RedirectToPage("./Areas/Identity/Pages/Account/Login");
            //return RedirectToPage("./Pages/Account/Login");
            //return RedirectToPage("./Account/Login");
            //return Response.Redirect("https://localhost:44378/Identity/Account/Login");
            //return new JsonResult("");

        }
        public async Task<ActionResult> OnGetSendingEmailAsync(string email, string firstName, string lastName)
        {
            var getEmail = await Task.Run(() => _context.School.Where(x => x.Email == email).FirstOrDefault());
            var checkEmail= await Task.Run(() => _context.Users.Where(x=>x.Email == email).FirstOrDefault());

            if (getEmail == null && checkEmail==null)
            {

                //SendEmailFromGmail emailSender = new SendEmailFromGmail();
                Random rnd = new Random();
                int otp = rnd.Next(1000, 9999);
                string title = firstName + " " + lastName;
                string body = "Your OTP Code is : " + otp.ToString();

                EmailSender emailSender = new EmailSender
                {
                    Subject = "OTP Verification",
                    ToEmail = email,
                    ToName = title,
                    //Content = $"Your {otp}  . <br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"
                    Content = $"{body} . <br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"
                };



                var response = await emailSender.SendEmailAsync();
                //var response = emailSender.EmailRequest(email, title, body);
                //if (response.Contains("successfully Send"))
                if (response == true)
                {
                    OTPVerification oTPVerification = new OTPVerification();
                    oTPVerification.Email = email;
                    oTPVerification.OTPGeneratedOn = DateTime.Now;
                    oTPVerification.OTPCode = otp;
                    _context.OTPVerification.Add(oTPVerification);
                    await _context.SaveChangesAsync();

                }
                return new JsonResult(response);

            }
            else
            {
                return new JsonResult("Email already registered");
            }

            //return null;
            
        }

        public async Task<ActionResult> OnGetUSAStatesAsync()
        {
            var getUSAStates = await Task.Run(() => _context.USState.ToList());
            List<SelectListItem> cities = new List<SelectListItem>();

            foreach (var row in getUSAStates)
            {
                cities.Add(new SelectListItem { Text = row.Name.ToString(), Value = row.Id.ToString() });
            }
            return new JsonResult(cities);
        }
        public async Task<ActionResult> OnGetVerifyOTPAsync(string email, string verifyOTP)
        {
            var getOtpRecord = await Task.Run(() => _context.OTPVerification.Where(x => x.Email == email && x.OTPCode == Convert.ToInt32(verifyOTP) && x.OTPGeneratedOn.Value.Date.Date == DateTime.Today.Date.Date).OrderByDescending(x => x.Id).FirstOrDefault());
            if (getOtpRecord != null)
            {
                var asd = getOtpRecord.OTPGeneratedOn.Value.Date;
                var addMinutes = getOtpRecord.OTPGeneratedOn.Value.AddMinutes(5);
                if (addMinutes >= DateTime.Now)
                {
                    getOtpRecord.OTPValidatedOn = DateTime.Now;
                    _context.OTPVerification.Update(getOtpRecord);
                    await _context.SaveChangesAsync();
                    return new JsonResult("Successfull");
                }
                else
                {
                    return new JsonResult("OTP Expire");
                }
            }
            else
                {
                return new JsonResult("OTP Not Valid");
            }
            //return new JsonResult("Error");
        }
    }
}
