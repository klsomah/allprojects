using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EducationalFundingCo.Utilities;
using Microsoft.Extensions.Configuration;

namespace EducationalFundingCo.Pages.Contract
{
    [Authorize(Roles = "SchoolAdministrator, Administrator , Student")]
    public class EQuestionnaireModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public EQuestionnaireModel(EducationalFundingCoContext context, 
            IWebHostEnvironment hostingEnvironment, 
            UserManager<IdentityUser> userManager, 
            IConfiguration config)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _config = config;
        }

        [BindProperty]
        public EmploymentQuestionnaire EmploymentQuestionnaire { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        [TempData]
        public string UploadMessage1 { get; set; }

        public Areas.Identity.Data.Contract Contract { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await GetContractAsync(id);

            EmploymentQuestionnaire = await _context.EmploymentQuestionnaires
                .Include(e => e.Contract).OrderByDescending(e => e.Id).FirstOrDefaultAsync(e => e.ContractId == Contract.Id);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await GetContractAsync(id);

                var empq = await _context.EmploymentQuestionnaires.OrderByDescending(e => e.Id).FirstOrDefaultAsync(e => e.ContractId == Contract.Id);

                if (empq == null) 
                    empq = new EmploymentQuestionnaire();

                if (empq.EmployerName == EmploymentQuestionnaire.EmployerName
                    && empq.Income == EmploymentQuestionnaire.Income
                    && empq.Address == EmploymentQuestionnaire.Address
                    && empq.City == EmploymentQuestionnaire.City
                    && empq.State == EmploymentQuestionnaire.State
                    && empq.Zipcode == EmploymentQuestionnaire.Zipcode
                    && empq.EmploymentStartDate == EmploymentQuestionnaire.EmploymentStartDate
                    && empq.HRContactNumber == EmploymentQuestionnaire.HRContactNumber)

                    return Page();

                EmploymentQuestionnaire.ContractId = Contract.Id;

                var contianerName = Contract.FirstName + "-" + Contract.LastName + "-" + Contract.Id;
                BlobStorageService blobStorageService = new BlobStorageService
                {
                    GetConfiguration = _config
                };

                if (Upload != null && Upload.Length > 0)
                {
                    var fileExt = Path.GetExtension(Upload.FileName);
                    var fileName = $"OfferLetter{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{fileExt}";

                    blobStorageService.UploadFileToBlob(contianerName, fileName, Upload, Upload.ContentType);
                }

                EmploymentQuestionnaire.CreatedOn = DateTime.Now;

                _context.EmploymentQuestionnaires.Add(EmploymentQuestionnaire);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Details", new { id = empq.ContractId});
            }
            catch (Exception ex)
            {
                UploadMessage1 = "Error: An Error has occurred" + ex.Message;
                return Page();
            }
        }

        private async Task GetContractAsync(int? id)
        {
            if (id == null)
            {
                var userId = _userManager.GetUserId(User);
                var email = _userManager.GetUserName(User);

                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.UserId.Trim() == userId.Trim());
            }
            else
            {
                Contract = await _context.Contracts
                    .Include(c => c.AcademyProgram)
                    .Include(c => c.IdentityUser).FirstOrDefaultAsync(m => m.Id == id);
            }
        }
    }
}
