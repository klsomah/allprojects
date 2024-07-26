using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authorization;

namespace EducationalFundingCo.Pages.Contract
{
    [Authorize(Roles = "SchoolAdministrator, Administrator")]
    public class IndexModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public IndexModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        public IList<Areas.Identity.Data.Contract> Contract { get;set; }

        public async Task OnGetAsync()
        {
            Contract = await _context.Contracts
                .Include(c => c.AcademyProgram)
                .Include(c => c.IdentityUser).ToListAsync();
        }
    }
}
