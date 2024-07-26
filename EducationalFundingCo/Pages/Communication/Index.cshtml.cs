using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;

namespace EducationalFundingCo.Pages.Communication
{
    public class IndexModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public IndexModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        public IList<Areas.Identity.Data.Communication> Communication { get;set; }

        public async Task OnGetAsync()
        {
            Communication = await _context.Communications
                .Include(c => c.Contract).ToListAsync();
        }
    }
}
