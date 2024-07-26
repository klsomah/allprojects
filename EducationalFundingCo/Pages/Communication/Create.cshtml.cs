using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;

namespace EducationalFundingCo.Pages.Communication
{
    public class CreateModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public CreateModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public EducationalFundingCo.Areas.Identity.Data.Communication Communication { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Communications.Add(Communication);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
