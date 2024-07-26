using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EducationalFundingCo.Data;
using EducationalFundingCo.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace EducationalFundingCo.Pages.Contract
{
    [Authorize(Roles = AllRoles.StudentEndUser)]
    public class UploadDocModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EducationalFundingCoContext _context;
        private readonly IConfiguration _config;

        [BindProperty]
        public IFormFile DrivesLicenseUpload { get; set; }

        [BindProperty]
        public IFormFile SocialSecurityCardUpload { get; set; }

        [BindProperty]
        public IFormFile BackgroundCheckUpload { get; set; }

        [BindProperty]
        public IFormFile BackgroundConsentUpload { get; set; }

        public Areas.Identity.Data.Contract Contract { get; set; }

        [TempData]
        public string UploadMessage { get; set; }

        public IEnumerable<IListBlobItem> AllBlobItems { get; set; }

        public UploadDocModel(UserManager<IdentityUser> userManager, 
            EducationalFundingCoContext context, 
            IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        public async Task OnGetAsync(int? id)
        {
            UploadMessage = string.Empty;
            await GetContractAsync(id);

            if (Contract != null) 
            {
                var contianerName = Contract.FirstName + "-" + Contract.LastName + "-" + Contract.Id;
                BlobStorageService blobStorageService = new BlobStorageService
                {
                    GetConfiguration = _config
                };

                AllBlobItems = await blobStorageService.ListBlobsFlatListingAsync(contianerName.ToLower(), null);
            }

        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            try
            {
                StringBuilder uploadStrs = new StringBuilder();
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                await GetContractAsync(id);

                var contianerName = Contract.FirstName + "-" + Contract.LastName + "-" + Contract.Id;
                BlobStorageService blobStorageService = new BlobStorageService
                {
                    GetConfiguration = _config
                };

                AzureBlobStorage azureBlobStorage = new AzureBlobStorage
                {
                    GetConfiguration = _config,
                    ContainerName = contianerName.ToLower()
                };


                if (SocialSecurityCardUpload.Length > 0)
                {
                    var fileExt = Path.GetExtension(SocialSecurityCardUpload.FileName);
                    var fileName = "SocialSecurityCard" + fileExt;

                    blobStorageService.UploadFileToBlob(contianerName, fileName, SocialSecurityCardUpload, SocialSecurityCardUpload.ContentType);

                    uploadStrs.Append(" Social Security Card,");
                }

                if (DrivesLicenseUpload.Length > 0)
                {
                    var fileExt = Path.GetExtension(DrivesLicenseUpload.FileName);
                    var fileName = "DrivesLicense" + fileExt;

                    blobStorageService.UploadFileToBlob(contianerName, fileName, DrivesLicenseUpload, DrivesLicenseUpload.ContentType);

                    uploadStrs.Append(" Drives License,");
                }

                AllBlobItems = await blobStorageService.ListBlobsFlatListingAsync(contianerName.ToLower(), null);

                UploadMessage = "The following files has be upload successful:" + uploadStrs.ToString();
                return Page();
            }
            catch (Exception ex)
            {
                UploadMessage = "Error: An Error has occurred" + ex.Message;
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
