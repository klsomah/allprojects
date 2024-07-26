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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EducationalFundingCo.Pages.Configurations
{
    [Authorize(Roles = "Administrator,SchoolAdministrator")]
    public class IndexModel : PageModel
    {
        private readonly EducationalFundingCo.Data.EducationalFundingCoContext _context;

        public IndexModel(EducationalFundingCo.Data.EducationalFundingCoContext context)
        {
            _context = context;
        }
        [BindProperty]
        public List<USState> USStateValues { get; set; }
        public IList<ConfigValue> ConfigValue { get;set; }
        [BindProperty]
        public List<string> SchoolName { get; set; }
        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }
        public async Task OnGetAsync()
        {
            int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
            ViewData["USAState"] = new SelectList(_context.USState.Select(x=>x.Name), "Id", "Name");
            //AcademyProgram = await Task.Run(() => _context.AcademyPrograms.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToListAsync());

            if (SchoolId != 0)
            {
                ConfigValue = await Task.Run(() => _context.ConfigValues.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToListAsync());
            }
            else if (SchoolId == 0)
            {
                ConfigValue = await Task.Run(() => _context.ConfigValues.Include(x => x.School).Where(x => x.School.RecordStatus == 2).ToListAsync());
            }
        }
    }
}
