using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EducationalFundingCo.Pages.Configurations
{
    [Authorize(Roles = "Administrator,SchoolAdministrator")]
    public class CreateModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public CreateModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            string schoolName = HttpContext.Session.GetString("SchoolName");
            return Page();
        }

        [BindProperty]
        public ConfigValue ConfigValue { get; set; }
        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));

            int? SchoolId = null;
            string schoolIdStr = HttpContext.Session.GetString("SchoolId");
            if (!string.IsNullOrEmpty(schoolIdStr))
            {
                SchoolId = Convert.ToInt32(schoolIdStr);
                ConfigValue.SchoolId = SchoolId;
            }

            if (schoolIdStr == null)
            {
               
                ConfigValue.SchoolId = SchoolId;
            }
            
            _context.ConfigValues.Add(ConfigValue);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
