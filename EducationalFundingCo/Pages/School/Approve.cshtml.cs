using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;

namespace EducationalFundingCo.Pages.School
{
    public class ApproveModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public ApproveModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            School = await _context.School.FirstOrDefaultAsync(m => m.Id == id);

            if (School == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnApproveGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            School = await _context.School.FirstOrDefaultAsync(m => m.Id == id);

            if (School == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            School = await _context.School.FindAsync(id);
            School.RecordStatus = 1;

            if (School != null)
            {
                _context.School.Update(School);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
