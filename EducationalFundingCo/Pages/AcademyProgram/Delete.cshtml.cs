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

namespace EducationalFundingCo.Pages.AcademyProgram
{
    [Authorize(Roles = "Administrator , SchoolAdministrator")]
    public class DeleteModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public DeleteModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Areas.Identity.Data.AcademyProgram AcademyProgram { get; set; }

        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AcademyProgram = await _context.AcademyPrograms.Include(x=>x.School).FirstOrDefaultAsync(m => m.Id == id);

            if (AcademyProgram == null)
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

            AcademyProgram = await _context.AcademyPrograms.FindAsync(id);

            if (AcademyProgram != null)
            {
                _context.AcademyPrograms.Remove(AcademyProgram);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
