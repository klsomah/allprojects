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
    public class DeleteModel : PageModel
    {

        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public DeleteModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Areas.Identity.Data.Contract Contract { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contract = await _context.Contracts
                .Include(c => c.AcademyProgram)
                .Include(c => c.IdentityUser).FirstOrDefaultAsync(m => m.Id == id);

            if (Contract == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRecordAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contract = await _context.Contracts.FindAsync(id);

            if (Contract != null)
            {
                _context.Contracts.Remove(Contract);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contract = await _context.Contracts.FindAsync(id);
            if (Contract != null)
            {
                _context.Contracts.Remove(Contract);
                await _context.SaveChangesAsync();
            }


            return RedirectToPage("./Index");

        }






    }
}
