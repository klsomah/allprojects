using EducationalFundingCo.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace EducationalFundingCo.Utilities
{
    public class IdentityTesting
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        

        public IdentityTesting(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> IdentityUserTesting(ApplicationUser applicationUser)
        {
            _context.ApplicationUser.Add(applicationUser);
            await _context.SaveChangesAsync();


            return new JsonResult("");
        }



    }
}
