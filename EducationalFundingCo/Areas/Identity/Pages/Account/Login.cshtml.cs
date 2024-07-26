using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EducationalFundingCo.Utilities;
using EducationalFundingCo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace EducationalFundingCo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly EducationalFundingCoContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager,
            EducationalFundingCoContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            //_logger.LogInformation("Login > OnPostAsync");
        }

        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
      {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Login > OnPostAsync", returnUrl);
                var email = Input.Email;
                var userid = await Task.Run(() => _context.Users.Where(x => x.Email.ToLower() == email.ToLower()).Select(x => x.Id).FirstOrDefault()); 
                var schoolId = await Task.Run(() => _context.ApplicationUser.Where(x=>x.IdentityUserId== userid).Select(x => x.SchoolId).FirstOrDefaultAsync());
                var Status = await Task.Run(() => _context.School.Where(x => x.Id == schoolId).FirstOrDefault());  //checking Student association with StudentAdmin and fetching Id from School
                          
                var app = await Task.Run(() => _context.School.Where(C => C.Email.ToLower() == Input.Email.ToLower() && C.RecordStatus == 4).FirstOrDefault());  //SchoolAdministrator
     
                int statusId = Status?.RecordStatus ?? 0;
              

                if  (app!=null || statusId == 4)
                {

                    TempData["ErrorMessage"] = "Login not allowed for this account";
                    return RedirectToPage("./Login");
                    
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
               
                    var userId = _userManager.GetUserId(User);
                    var user = await _userManager.FindByIdAsync(userId);
                    if (userId == null)
                    {
                        user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == Input.Email.ToLower());
                    }
                    
                    _logger.LogInformation("User logged in.");
                    if (await _userManager.IsInRoleAsync(user, AllRoles.AdminEndUser))
                    {                   

                        return RedirectToPage("/Index");
                    }

                    if (await _userManager.IsInRoleAsync(user, AllRoles.StudentEndUser))
                    {
                   
                        await SetSessionForCurrentUserAsync(user.Id);


                        return RedirectToPage("/Contract/Details");
                    }
                    if (await _userManager.IsInRoleAsync(user, AllRoles.SchoolAdministrator))
                    {
                        await SetSessionForCurrentUserAsync(user.Id);


                        return RedirectToPage("/Index");
                    }

                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            return Page();
        }

        public async Task<(string SchoolName, int? SchoolId)> SetSessionForCurrentUserAsync(string userId)
        {
            var getschoolId = await Task.Run(() => _context.ApplicationUser
                .Where(x => x.IdentityUserId == userId)
                .Select(x => x.SchoolId)
                .FirstOrDefault());

            string getschoolName = null;
            if (getschoolId != null)
            {
                getschoolName = await Task.Run(() => _context.School
                    .Where(x => x.Id == getschoolId)
                    .Select(x => x.Name)
                    .FirstOrDefault());
            }

            HttpContext.Session.SetString("SchoolId", Convert.ToString(getschoolId));
            if (getschoolName != null)
            {
                HttpContext.Session.SetString("SchoolName", Convert.ToString(getschoolName));
            }
          
            return (SchoolName: getschoolName, SchoolId: getschoolId.Value);
        }
 

    }
}
