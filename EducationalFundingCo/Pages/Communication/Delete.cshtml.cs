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
    public class DeleteModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public DeleteModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EducationalFundingCo.Areas.Identity.Data.Communication Communication { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Communication = await _context.Communications
                .Include(c => c.Contract).FirstOrDefaultAsync(m => m.Id == id);

            if (Communication == null)
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

            Communication = await _context.Communications.FindAsync(id);

            if (Communication != null)
            {
                _context.Communications.Remove(Communication);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
