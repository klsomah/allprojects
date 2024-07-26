using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Grpc.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Web.Helpers;
using EducationalFundingCo.Utilities;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace EducationalFundingCo.Pages.School
{
    //[Authorize(Roles = "Administrator")]
    [Authorize(Roles = "Administrator , SchoolAdministrator")]
    public class IndexModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IndexModel(EducationalFundingCo.Data.EducationalFundingCoContext context , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<IndexModel> logger, IEmailSender emailSender, IWebHostEnvironment hostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _hostingEnvironment = hostEnvironment;
            _roleManager = roleManager;
        }
       
          

        [BindProperty]
        public IdentityUser IdentityUser { get; set; }

        [BindProperty]
        public IList<Areas.Identity.Data.School> School { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string Email { get; set; }


        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public Areas.Identity.Data.Contract Contract { get; set; }

        [BindProperty]
        public IList<Areas.Identity.Data.USState> USState { get; set; }
        public async Task OnGetAsync()
        {

            School = await _context.School.OrderByDescending(x=>x.CreatedOn).ToListAsync();


        }

        public async Task<ActionResult> OnGetApproveRequestAsync(int Id, string Email, string FirstName, string LastName)
        {
            var email = Email;
            var userName = FirstName + " " + LastName;
            var Status = await Task.Run(() => _context.School.Where(x => x.Id == Id).FirstOrDefault());

            Status.RecordStatus = 2; //Set For Approve
            Status.Status = "Approved";
            _context.School.Update(Status);
            await _context.SaveChangesAsync();

            var userPassword = GenerateRandomPassword();

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, userPassword);

                var error = result.Errors.FirstOrDefault();

                if (result.Succeeded || (error != null && error.Code == "DuplicateUserName"))
                {
                    if (error == null)
                    {
                        var userId = await AddApplicationUser(email, Id);
                        await AddSchoolAdministratorRole(userId);
                        await AddUserRole(userId);
                    }

                    await SendLoginCredentialsEmail(email, userName, userPassword);
                }
            }

            return new JsonResult("Successfully Accepted");
        }

        private string GenerateRandomPassword()
        {
            RandomPassword randomPassowrd = new RandomPassword();
            return randomPassowrd.GenerateRandomPassword();
        }

        private async Task<string> AddApplicationUser(string email, int id)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            var userId = await Task.Run(() => _context.Users.Where(x => x.Email == email).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault());
            applicationUser.IdentityUserId = userId;
            applicationUser.SchoolId = id;
            applicationUser.CreatedOn = DateTime.Now;
            _context.ApplicationUser.Add(applicationUser);
            await _context.SaveChangesAsync();
            return userId;
        }

        private async Task AddSchoolAdministratorRole(string userId)
        {
            await _roleManager.CreateAsync(new IdentityRole(AllRoles.SchoolAdministrator));
        }

        private async Task AddUserRole(string userId)
        {
            var getRoles = await Task.Run(() => _context.Roles.Where(x => x.Name.Contains("SchoolAdministrator")).Select(x => x.Id).FirstOrDefault());
            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>();
            identityUserRole.UserId = userId;
            identityUserRole.RoleId = getRoles;
            _context.UserRoles.Add(identityUserRole);
            await _context.SaveChangesAsync();
        }

        private async Task SendLoginCredentialsEmail(string email, string userName, string userPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            EmailSender emailSender = new EmailSender
            {
                Subject = "Login Credentials | Reset Password",
                ToEmail = email,
                ToName = userName,
                Content = $"Hello {userName}, <br/> Your information on file is: Email: {email} <br/> Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. <br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"
            };


            //string loginUrl = HttpContext.Request.Host.Value + "/Identity/Account/Login";
            //EmailSender emailSender = new EmailSender
            //{
            //    Subject = "Login Credentials",
            //    ToEmail = email,
            //    ToName = userName,
            //    Content = $"Hello " + userName + ", <br/> Your Email and Password is: <br/> Email: " + email + " <br/> Password: " + userPassword + " " + "><br/>Click here to login :" + loginUrl + " <br/> " +
            //        "<br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"
            //    //Content = $"Hello, <br/> Please confirm your account by <a href=" + HtmlEncoder.Default.Encode(callbackUrl) + ">clicking here</a>. <br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"
            //};

            await emailSender.SendEmailAsync();
        }

        public async Task<ActionResult> OnGetRejectRequestAsync(int Id)  //OnPostRejectAsync
        {

            var Status = await Task.Run(() => _context.School.Where(x => x.Id == Id).FirstOrDefault());

            Status.RecordStatus = 3;
            Status.Status = "Rejected";                                  //Set For Reject
            _context.School.Update(Status);
            await _context.SaveChangesAsync();

            return new JsonResult("Successfully Rejected");


        }
        public async Task<ActionResult> OnGetDeactivateRequestAsync(int Id)  //OnPostRejectAsync
        {

            var Status = await Task.Run(() => _context.School.Where(x => x.Id == Id).FirstOrDefault());

            Status.RecordStatus = 4;
            Status.Status = "InActive";                                  //Set For Reject
            _context.School.Update(Status);
            await _context.SaveChangesAsync();

            return new JsonResult("Successfully Deactivated");


        }


    }
}
