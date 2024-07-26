using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using EducationalFundingCo.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace EducationalFundingCo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly EducationalFundingCoContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            EducationalFundingCoContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment hostEnvironment,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _hostingEnvironment = hostEnvironment;
            _context = context;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public Data.Contract Contract { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGetAsync(string Id, string code, string SchoolId, string returnUrl = null)
        {

            //byte[] bytes = Convert.FromBase64String(SchoolId) ? new byte[0] : Convert.FromBase64String(SchoolId);

            //var School_Id = SchoolId;

            //if (School_Id != "")
            //{
            //    byte[] bytes = string.IsNullOrEmpty(SchoolId) ? new byte[0] : Convert.FromBase64String(SchoolId);
            //    int schoolId = BitConverter.ToInt32(bytes, 0);

            //}

            int schoolId = 0; // Declare schoolId outside of the if block

            var School_Id = SchoolId;

            if (!string.IsNullOrEmpty(School_Id)) // Simplify the check for a non-empty string
            {
                byte[] bytes = Convert.FromBase64String(School_Id);
                if (bytes.Length >= 4) // Check that bytes contains at least 4 bytes
                {
                    schoolId = BitConverter.ToInt32(bytes, 0); // Update schoolId if bytes contains 4 or more bytes
                }
            }



            //byte[] bytes = string.IsNullOrEmpty(SchoolId) ? new byte[0] : Convert.FromBase64String(SchoolId);
            //int schoolId = 0;
            //if (bytes.Length >= 4 && schoolId!=0)
            //{
            //    schoolId = BitConverter.ToInt32(bytes, 0);
            //}

            TempData["admin"] = "";
            TempData["prgmId"] = "";
            TempData["SchoolId"] = schoolId;
            ReturnUrl = returnUrl;
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["adminreg"]) && HttpContext.Request.Query["adminreg"].ToString() == "1")
            {
                TempData["admin"] = "admin";
                return Page();
            }

            if (Id == null && code == null && SchoolId == null)
            {
                return RedirectToPage("/NoInvite");
            }

            ReturnUrl = returnUrl;

            TempData["prgmId"] = Id;

            byte[] data = Convert.FromBase64String(code);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            if (when < DateTime.UtcNow.AddHours(-168))
            {
                return RedirectToPage("/NoInvite");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string Id, string code, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            int SchoolId = Convert.ToInt32( TempData["SchoolId"]);
             
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    if (SchoolId > 0)
                    {
                        ApplicationUser applicationUser = new ApplicationUser();
                        var userId = await Task.Run(() => _context.Users.Where(x => x.Email == Input.Email).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault());
                        applicationUser.IdentityUserId = userId;
                        applicationUser.SchoolId = SchoolId;
                        applicationUser.CreatedOn = DateTime.Now;
                        _context.ApplicationUser.Add(applicationUser);
                        await _context.SaveChangesAsync();
                    }
                    
                    if (!await _roleManager.RoleExistsAsync(AllRoles.AdminEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(AllRoles.AdminEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(AllRoles.StudentEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(AllRoles.StudentEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(AllRoles.SchoolAdministrator))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(AllRoles.SchoolAdministrator));
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code1 = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code1));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    if (!string.IsNullOrEmpty(TempData["admin"].ToString()))
                    {
                        await _userManager.AddToRoleAsync(user, AllRoles.AdminEndUser);
                        //await _userManager.ConfirmEmailAsync(user, code);
                        return RedirectToPage("Login");
                    }

                    if (!string.IsNullOrEmpty(TempData["prgmId"].ToString()))
                    {
                        await _userManager.AddToRoleAsync(user, AllRoles.StudentEndUser);

                        var pId = Convert.ToInt32(TempData["prgmId"].ToString());

                        Contract = new Data.Contract
                        {
                            Email = Input.Email,
                            UserId = user.Id,
                            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            ProgramId = pId,
                            SchoolId = SchoolId,
                            RecordStatus = null,
                            
                        };

                        _context.Contracts.Add(Contract);
                        await _context.SaveChangesAsync();


                    }

                    //var path = Path.Combine(_hostingEnvironment.WebRootPath, $"assets/images/NH-Logo.png");


                    EmailSender emailSender = new EmailSender
                    {
                        Subject = "Confirm your email",
                        ToEmail = Input.Email,
                        ToName = "Registrant",
                        Content = $"Hello, <br/> Please confirm your account by <a href=" + HtmlEncoder.Default.Encode(callbackUrl) + ">clicking here</a>. <br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"
                    };

                    await emailSender.SendEmailAsync();

                    //SendEmailFromGmail sfgmail = new SendEmailFromGmail();
                    //sfgmail.SendEmail(Input.Email, "Registrant", "Confirm your email",
                    //        string.Format("Hello, <br/> Please confirm your account by <a href=" + HtmlEncoder.Default.Encode(callbackUrl) + ">clicking here</a>. <br/><br/>Thanks, <br/> Educational Funding Co.<br/><br/>"), null);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
