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
using Microsoft.AspNetCore.Http;

namespace EducationalFundingCo.Pages.AcademyProgram
{
    [Authorize(Roles = "Administrator , SchoolAdministrator")]
    public class EditModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public EditModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Areas.Identity.Data.AcademyProgram AcademyProgram { get; set; }

        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        [BindProperty]
        public string SchoolName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //string schoolName = HttpContext.Session.GetString("SchoolName");
            //SchoolName = schoolName;
            AcademyProgram = await _context.AcademyPrograms.FirstOrDefaultAsync(m => m.Id == id);

            if (AcademyProgram == null)
            {
                return NotFound();
            }
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
            int? SchoolId = null;
            string schoolIdStr = HttpContext.Session.GetString("SchoolId");
            if (!string.IsNullOrEmpty(schoolIdStr))
            {
                SchoolId = Convert.ToInt32(schoolIdStr);
                AcademyProgram.SchoolId = SchoolId;
            }

            if (schoolIdStr == null)
            {
                AcademyProgram.SchoolId = Convert.ToInt32(schoolIdStr);
            }


            AcademyProgram.Duration = AcademyProgram.Duration + " Months";
            AcademyProgram.MaxDefermentPeriod = AcademyProgram.MaxDefermentPeriod + " Months";


            _context.Attach(AcademyProgram).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcademyProgramExists(AcademyProgram.Id))
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

        private bool AcademyProgramExists(int id)
        {
            return _context.AcademyPrograms.Any(e => e.Id == id);
        }
    }
}
