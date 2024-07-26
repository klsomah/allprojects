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

namespace EducationalFundingCo.Pages.Configurations
{
    [Authorize(Roles = "Administrator,SchoolAdministrator")]
    public class EditModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public EditModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ConfigValue).State = EntityState.Modified;

            try
            {
                //int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));

                int? SchoolId = null;
                string schoolIdStr = HttpContext.Session.GetString("SchoolId");

                if (!string.IsNullOrEmpty(schoolIdStr))
                {
                    SchoolId = Convert.ToInt32(schoolIdStr);
                    ConfigValue.SchoolId = Convert.ToInt32(SchoolId);
                }
                if (SchoolId == null)
                {
                    ConfigValue.SchoolId = SchoolId;
                }

                //ConfigValue.SchoolId=SchoolId;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigValueExists(ConfigValue.Id))
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

        private bool ConfigValueExists(int id)
        {
            return _context.ConfigValues.Any(e => e.Id == id);
        }
    }
}
