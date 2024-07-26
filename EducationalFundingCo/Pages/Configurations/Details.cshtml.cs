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

namespace EducationalFundingCo.Pages.Configurations
{
    [Authorize(Roles = "Administrator,SchoolAdministrator")]
    public class DetailsModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public DetailsModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        public ConfigValue ConfigValue { get; set; }
        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ConfigValue = await _context.ConfigValues.FirstOrDefaultAsync(m => m.Id == id);

            if (ConfigValue == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
