using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Acklann.Plaid;
using System;
using EducationalFundingCo.Utilities;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using EducationalFundingCo.Areas.Identity.Data;

namespace EducationalFundingCo.Pages.Contract
{
    [Authorize(Roles = "SchoolAdministrator, Administrator , Student")]
    public class CreateModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;

        public CreateModel(
            EducationalFundingCoContext context, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }


        [BindProperty]
        public int ProgrmId { get; set; }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public Areas.Identity.Data.Contract Contract { get; set; }

        [BindProperty]
        public Areas.Identity.Data.AcademyProgram AcademyProgram { get; set; }



        public string LinkToken { get; set; }


        [TempData]
        public bool IsAppSigned { get; set; }


        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (User.IsInRole(AllRoles.AdminEndUser) && Id > 0)
            {
                Contract = _context.Contracts
                    .Include(a => a.AcademyProgram)
                    .Include(s => s.School)
                    .FirstOrDefault(c => c.Id == Id);
                return Page();
            }

            var userId = _userManager.GetUserId(User);
            var email = _userManager.GetUserName(User);
            IsAppSigned = false;

            Contract = _context.Contracts
                .Include(a => a.AcademyProgram)
                .Include(s => s.School)
                .FirstOrDefault(c => c.Email == email);

            if (Contract != null && Contract.SignatureUrl != null)
                IsAppSigned = true;

            // var client = new PlaidClient("5fb429e521746b0017dd950f", "3d7eae624a7c166db29cff8cb37488", "", Environment.Development);

            var client = new PlaidClient(_config["Plaid:ClientId"], _config["Plaid:Secret"], null, Acklann.Plaid.Environment.Development);
            var response = await client.CreateLinkToken(new Acklann.Plaid.Management.CreateLinkTokenRequest
            {
                ClientName = "Educational Funding Co.",
                ClientId = _config["Plaid:ClientId"],
                Secret = _config["Plaid:Secret"],
                CountryCodes = new string[] { "US" },
                Products = new string[] { "auth", "transactions" },
                User = new Acklann.Plaid.Management.CreateLinkTokenRequest.UserInfo
                {
                    ClientUserId = Guid.NewGuid().ToString()
                }
            });

            LinkToken = response.LinkToken;

            if (Id <= 0 && Contract == null)
                return Page();

            if (!string.IsNullOrWhiteSpace(Contract.FirstName) 
                && !string.IsNullOrWhiteSpace(Contract.LastName)
                && !string.IsNullOrWhiteSpace(Contract.SocialSecurityNum)
                && !string.IsNullOrWhiteSpace(Contract.Address)
                && !string.IsNullOrWhiteSpace(Contract.City)
                && !string.IsNullOrWhiteSpace(Contract.Zipcode)
                && string.IsNullOrWhiteSpace(Contract.SignatureUrl))
                return RedirectToPage("/Contract/SignContract");

            return Page();


        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
 
            _context.Attach(Contract).State = EntityState.Modified;

            try
            {
                int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
                
                if(Contract.SchoolId == null || Contract.SchoolId == 0)
                    Contract.SchoolId=SchoolId;
                await _context.SaveChangesAsync();

                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.Id == Contract.Id);
                await CreateContract(Contract);

            }
            catch (DbUpdateConcurrencyException)
            {
               
            }
            return RedirectToPage("/Contract/SignContract", new { Contract.Id });

        }

        public  void OnPostTestAsync()
        {

        }

        // To search and replace content in a document part.
        public async Task CreateContract(Areas.Identity.Data.Contract contract)
        {
            int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
            
            
            string schoolName = null;
            string authorityName = null;
            string authorityPosition = null;


            var config = _context.ConfigValues.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToList();


            //var SchoolName= config.FirstOrDefault(c => c.KeyPair.Contains("School Name"));
                schoolName = config?.FirstOrDefault(c => c.KeyPair.Contains("School Name"))?.ValuePair;
                authorityName = config.FirstOrDefault(c => c.KeyPair.Contains("School Authority Name"))?.ValuePair;
                authorityPosition = config.FirstOrDefault(c => c.KeyPair.Contains("School Authority Position"))?.ValuePair;


            CreateContract createContract = new CreateContract
            {
                TemplatePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/IncomeShareAgreementV2.docx"),
                SavedPath = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.FirstName}-{contract.LastName}-{contract.Id}.docx"),
                //SavedPathHtml = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.FirstName}-{contract.LastName}-{contract.Id}.html"),
                //SavedPathPdf = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.FirstName}-{contract.LastName}-{contract.Id}.pdf"),
                ObligorName = $"{contract.FirstName} {contract.LastName}",
                ObligorAddress = $"{contract.Address}, {contract.City}, {contract.State} {contract.Zipcode}",
                ProgramName = $"{contract.AcademyProgram.ProgramName}",
                SchoolName = schoolName ?? "COIN Education Services Center",
                AuthorityName = authorityName ?? "Nneka Chukwu",
                AuthorityPosition = authorityPosition ?? "President",
                ObligorEmail = $"{contract.Email}",
                CurrentDate = DateTime.Now.ToShortDateString(),
                //RecordStatus = 
                    
                };

                await createContract.SaveContractPerObligator();
                await createContract.SearchAndReplace();



                if (System.IO.File.Exists(createContract.SavedPath))
                {
                    var contianerName = $"{contract.FirstName}-{contract.LastName}-{contract.Id}";
                    var fileName = $"Contract.docx";

                    AzureBlobStorage azureBlobStorage = new AzureBlobStorage
                    {
                        GetConfiguration = _config,
                        ContainerName = contianerName.ToLower(),
                        FileName = fileName,
                        SourcePath = createContract.SavedPath
                    };

                    await azureBlobStorage.UploadFileToBlob();

                }
                await createContract.DisposeFile(createContract.SavedPath);

            





        }

    }
}
