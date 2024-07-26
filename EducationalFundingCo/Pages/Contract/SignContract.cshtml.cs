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
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace EducationalFundingCo.Pages.Contract
{
    //[Authorize(Roles = "Student, Administrator")]
    [Authorize(Roles = "SchoolAdministrator, Administrator , Student")]
    public class SignContractModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;

        public SignContractModel(
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
        public string ContractFile { get; set; }

        [TempData]
        public bool IsAppSigned { get; set; }


        public async Task<IActionResult> OnGetAsync(int Id)
        {
            if (User.IsInRole(AllRoles.AdminEndUser) && Id > 0)
            {
                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.Id == Id);
                return Page();
            }

            var userId = _userManager.GetUserId(User);
            var email = _userManager.GetUserName(User);

            Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.UserId.Trim() == userId.Trim());

            IsAppSigned = false;
            if (Contract != null && Contract.SignatureUrl != null)
                IsAppSigned = true;


            if (Contract == null)
                return RedirectToPage("/Index");
            else if (string.IsNullOrWhiteSpace(Contract.FirstName) && string.IsNullOrWhiteSpace(Contract.LastName))
                return RedirectToPage("/Contract/Create");
            else if(string.IsNullOrWhiteSpace(Contract.SocialSecurityNum))
                return RedirectToPage("/Contract/Create");
            else
            {
                AzureBlobStorage azureBlobStorage = new AzureBlobStorage();
                var contianerName = $"{Contract.FirstName}-{Contract.LastName}-{Contract.Id}";
                azureBlobStorage.ContainerName = contianerName.ToLower();
                azureBlobStorage.GetConfiguration = _config;
                bool hasContract = false;

                var blobList = azureBlobStorage.GetBlobItems();

                if (blobList != null)
                {
                    await foreach (BlobItem item in blobList)
                    {
                        if (item.Name == "Contract.docx")
                        {
                            BlobClient blob = azureBlobStorage.GetBlobContainerClient.GetBlobClient(item.Name);
                            ContractFile = blob.Uri.AbsoluteUri;
                            hasContract = true;
                            break;
                        }
                    }
                }
                if (!hasContract)
                {
                    return Page();
                    //return RedirectToPage("/Index");
                }

            }

            return Page();


        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = _userManager.GetUserId(User);
            var email = _userManager.GetUserName(User);

            var contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.UserId.Trim() == userId.Trim());
            contract.SignatureUrl = Contract.SignatureUrl;

            if (!IsAppSigned)
            {
                contract.DateSigned = DateTime.Now;
            }
               
            _context.Attach(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                var callbackUrl = Url.Page(
                        "/Contract/UploadDoc",
                        pageHandler: null,
                        values: new { userId = Contract.UserId },
                        protocol: Request.Scheme);

                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.Id == Contract.Id);
                //await CreateContract(Contract);

                var attachment = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.FirstName}-{contract.LastName}-{contract.Id}.docx");
                //var acdProgram = _context.AcademyPrograms.FirstOrDefault(a => a.Id == Contract.ProgramId).ProgramName;

                //SendEmailFromGmail sfgmail = new SendEmailFromGmail();
                //sfgmail.SendEmail(Contract.Email, Contract.FirstName + " " + Contract.LastName, "Income Share Agreement",
                //        $"Hello {Contract.FirstName}, <br/><br/> Thank you for starting your Income Share Agreement with <b>Education Funding Co.</b><br/><br/> " +
                //        $"To secure your seat in our next <b>{Contract.AcademyProgram.ProgramName}</b> cohort, you will need to <b>upload</b> the following by, <b>{DateTime.Now.AddDays(10).ToShortDateString()}</b>.<br/><br/>" +
                //        $"<ol><li>A copy of your driver’s license</li><li>A copy of your Social Security card</li></ol><br/>" +
                //        $"You can upload the documents by <a href={HtmlEncoder.Default.Encode(callbackUrl)}>clicking here</a>.<br/><br/>" +
                //        $"Please reply to this email with any questions.<br/><br/>" +
                //        $"All the best,<br/><br/>" +
                //        $"Admissions<br/>" +
                //        $"Education Funding Co.", attachment);

                EmailSender emailSender = new EmailSender
                {
                    Subject = "Income Share Agreement",
                    ToEmail = Contract.Email,
                    ToName = Contract.FirstName + " " + Contract.LastName,
                    Content = $"Hello {Contract.FirstName}, <br/><br/> Thank you for starting your Income Share Agreement with <b>Education Funding Co.</b><br/><br/> " +
                        $"To secure your seat in our next <b>{Contract.AcademyProgram.ProgramName}</b> cohort, you will need to <b>upload</b> the following by, <b>{DateTime.Now.AddDays(10).ToShortDateString()}</b>.<br/><br/>" +
                        $"<ol><li>A copy of your driver’s license</li><li>A copy of your Social Security card</li></ol><br/>" +
                        $"You can upload the documents by <a href={HtmlEncoder.Default.Encode(callbackUrl)}>clicking here</a>.<br/><br/>" +
                        $"Please reply to this email with any questions.<br/><br/>" +
                        $"All the best,<br/><br/>" +
                        $"Admissions<br/>" +
                        $"Education Funding Co."
                };

                await emailSender.SendEmailAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!ContractExists(Contract.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
            return RedirectToPage("/Contract/Details", new { Contract.Id });

        }

        public  void OnPostTestAsync()
        {

        }

        // To search and replace content in a document part.
        public async Task CreateContract(Areas.Identity.Data.Contract contract)
        {
            CreateContract createContract = new CreateContract();

            createContract.TemplatePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/IncomeShareAgreementV2.docx");
            createContract.SavedPath = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.Id}_{ contract.FirstName}_{contract.LastName}.docx");
            createContract.SavedPathHtml = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.Id}_{ contract.FirstName}_{contract.LastName}.html");
            createContract.SavedPathPdf = Path.Combine(_hostingEnvironment.WebRootPath, $"Contracts/{contract.Id}_{ contract.FirstName}_{contract.LastName}.pdf");
            createContract.ObligorName = $"{contract.FirstName} {contract.LastName}";
            createContract.ObligorAddress = $"{contract.Address}, {contract.City}, {contract.State} {contract.Zipcode}";
            createContract.ProgramName = $"{contract.AcademyProgram.ProgramName}";
            createContract.SchoolName = "COIN Education Services Center";
            createContract.AuthorityName = "Nneka Chukwu";
            createContract.AuthorityPosition = "President";
            createContract.ObligorEmail = $"{contract.Email}";
            createContract.CurrentDate = DateTime.Now.ToShortDateString();

            await createContract.SaveContractPerObligator();
            await createContract.SearchAndReplace();
        }

       

    }
}
