using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Identity;
using EducationalFundingCo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace EducationalFundingCo.Utilities
{
    public class CurrentUserSession
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EducationalFundingCoContext _context;

        public CurrentUserSession(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            EducationalFundingCoContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            //_logger.LogInformation("Login > OnPostAsync");
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
