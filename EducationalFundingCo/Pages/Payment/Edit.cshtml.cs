using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authorization;

namespace EducationalFundingCo.Pages.Payment
{
    [Authorize(Roles = "SchoolAdministrator, Administrator")]
    public class EditModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public EditModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EducationalFundingCo.Areas.Identity.Data.Payment Payment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await _context.Payments
                .Include(p => p.AcademyProgram)
                .Include(p => p.Contract).FirstOrDefaultAsync(m => m.Id == id);

            if (Payment == null)
            {
                return NotFound();
            }
           ViewData["ProgramId"] = new SelectList(_context.AcademyPrograms, "Id", "Id");
           ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(Payment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
