using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authorization;
using EducationalFundingCo.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace EducationalFundingCo.Pages.AcademyProgram
{
    [Authorize(Roles = "Administrator , SchoolAdministrator")]
    //[Authorize(Roles = AllRoles.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }

        [BindProperty]
        public Areas.Identity.Data.AcademyProgram AcademyProgram { get; set; }

        [BindProperty]
        public string SchoolName { get; set; }

        
        public CreateModel(EducationalFundingCoContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> OnGet()
        {

            //var userLogin = HttpContext.User.Identity.Name.ToString();
            //var getUserId = await Task.Run(() => _context.Users.Where(x => x.Email == userLogin).Select(x => x.Id).FirstOrDefault());
            //var getschoolId = await Task.Run(() => _context.ApplicationUser.Where(x => x.IdentityUserId == getUserId).Select(x => x.SchoolId).FirstOrDefault());
            //var getschoolName = await Task.Run(() => _context.School.Where(x => x.Id == getschoolId).Select(x=>x.Name).FirstOrDefault());
            //string schoolName = HttpContext.Session.GetString("SchoolName");

            string schoolName = (HttpContext.Session.GetString("SchoolName") ?? "").ToString();


            //School=_context.AcademyPrograms.Include(x=>x.School).ToListAsync(); 


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
            //int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
            //int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId") ?? "");

            int? SchoolId = null;
            string schoolIdStr = HttpContext.Session.GetString("SchoolId");
            if (!string.IsNullOrEmpty(schoolIdStr))
            {
                SchoolId = Convert.ToInt32(schoolIdStr);
                AcademyProgram.SchoolId = SchoolId;
            }

            if (SchoolId == null)
            {
                AcademyProgram.SchoolId = SchoolId;
            }


            
            AcademyProgram.Duration = AcademyProgram.Duration + " Months";
            AcademyProgram.MaxDefermentPeriod = AcademyProgram.MaxDefermentPeriod + " Months";

            _context.AcademyPrograms.Add(AcademyProgram);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
