using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using EducationalFundingCo.Utilities;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EducationalFundingCo.Pages
{
    [Authorize(Roles = "Administrator, SchoolAdministrator" )]
    public class ApplicationsModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationsModel(EducationalFundingCoContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }
        [BindProperty]
        public Areas.Identity.Data.School School { get; set; }
        [BindProperty]
        public string SchoolName { get;set; }
        [BindProperty]
        public Areas.Identity.Data.Contract Contract { get; set; }
        public IList<Areas.Identity.Data.Contract> Contracts { get;set; }
        public IList<Areas.Identity.Data.EmploymentQuestionnaire> EmploymentQuestionnaire { get; set; }

        public PaginatedList<Areas.Identity.Data.Payment> PagedContract { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));

                if (SchoolId == 0)
                {
                    var getschoolId = await _context.ApplicationUser
                      .Where(x => x.IdentityUserId == userId).Select(x => x.SchoolId).FirstOrDefaultAsync();

                    if (getschoolId != null && getschoolId > 0)
                    {
                        SchoolId = (int)getschoolId;
                    }
                }


                if (SchoolId != 0)
                {
                    Contracts = await Task.Run(() => _context.Contracts.Include(c => c.AcademyProgram).Include(c => c.School).Where(x => x.SchoolId == SchoolId).ToList());
                }
                else if (SchoolId == 0)
                {
                    Contracts = await Task.Run(() => _context.Contracts.Include(c => c.AcademyProgram).Include(c => c.School).Where(x => x.School.RecordStatus==2).ToList());
                }



                EmploymentQuestionnaire = await _context.EmploymentQuestionnaires
                    .Include(e => e.Contract.AcademyProgram).ToListAsync();
            }
            catch(SqlException ex)
            {
                var err = ex.Message;
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.Id == id);

            if (contract.AcceptedDate == null && contract.RecordStatus ==null)
            {
                contract.AcceptedDate = DateTime.Now;
                contract.RecordStatus = 1;
            }

            else
            {
                contract.AcceptedDate = null;
            }
                

            _context.Attach(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Contracts = await _context.Contracts
               .Include(c => c.AcademyProgram)
               .Include(c => c.School)
               .Where(c => c.AcceptedDate == null)
               .Include(c => c.IdentityUser).ToListAsync();

            EmploymentQuestionnaire = await _context.EmploymentQuestionnaires
                .Include(e => e.Contract.AcademyProgram).ToListAsync();

            return Page();
        }
       
        public async Task<IActionResult> OnGetDeleteRecord1Async(int? id)
        {
            try
            {


                if (id == null)
                {
                    return NotFound();
                }

                Contract = await _context.Contracts.FindAsync(id);

                Contract.RecordStatus = 0;                                //Soft Delete

                 _context.Contracts.Update(Contract);
                await _context.SaveChangesAsync();
                return new JsonResult("Updated");

            }
            catch (Exception ex)
            {
                return null;
            }

            //}

            //return RedirectToPage("./Index");
            //return new JsonResult("Deleted");
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
