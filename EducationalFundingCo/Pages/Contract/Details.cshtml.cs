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
using Microsoft.AspNetCore.Identity;
using EducationalFundingCo.Utilities;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace EducationalFundingCo.Pages.Contract
{
    //[Authorize(Roles = "Student, Administrator")]
    [Authorize(Roles = "SchoolAdministrator, Administrator , Student")]
    public class DetailsModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public DetailsModel(EducationalFundingCoContext context, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        public Areas.Identity.Data.Contract Contract { get; set; }

        [BindProperty]
        public EmploymentQuestionnaire EmploymentQuestionnaire { get; set; }

        [TempData]
        public int YourContractId { get; set; }

        public IQueryable<Areas.Identity.Data.Payment> Payments { get; set; }
        public PaginatedList<Areas.Identity.Data.Payment> PagedPayments { get; set; }

        public IEnumerable<IListBlobItem> AllBlobItems { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal PaymentTotal { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            YourContractId = 0;
            IQueryable<Areas.Identity.Data.Payment> payments = new List<Areas.Identity.Data.Payment>().AsQueryable();
            int pageSize = 3;

            if (id == null)
            {
                var userId = _userManager.GetUserId(User);
                var email = _userManager.GetUserName(User);

                Contract = _context.Contracts
                    .Include(a => a.AcademyProgram)
                    .Include(c => c.IdentityUser)
                    .FirstOrDefault(c => c.UserId.Trim() == userId.Trim());

              
            }
            else if(id > 0)
            {
                Contract = await _context.Contracts
               .Include(c => c.AcademyProgram)
               .Include(c => c.IdentityUser)
               .FirstOrDefaultAsync(m => m.Id == id);
            }
            else
            {
                if (Contract == null)
                {
                    return NotFound();
                }
            }

            Payments = _context.Payments
                 .Include(p => p.AcademyProgram)
                 .Include(p => p.Contract)
                 .OrderBy(p => p.ScheduledDate)
                 .Where(p => p.Contract.Id == Contract.Id);

            PaymentTotal = (decimal)Payments
                .Where(p => p.PaymentMethod != null
                            && p.CompleteDate != null
                            && p.TransactionDate != null)
                .Sum(p => p.Amount);

            payments = Payments
                 .Where(p => p.PaymentMethod == null
                            && p.CompleteDate == null
                            && p.TransactionDate == null).AsQueryable();

            if(payments != null)
                PagedPayments = await PaginatedList<Areas.Identity.Data.Payment>.CreateAsync(payments.AsNoTracking(), pageIndex ?? 1, pageSize);

            if (Contract != null)
            {
                if(string.IsNullOrWhiteSpace(Contract.SignatureUrl))
                    return RedirectToPage("/Contract/SignContract");

                id = Contract.Id;
                YourContractId = Contract.Id;

                var contianerName = Contract.FirstName + "-" + Contract.LastName + "-" + Contract.Id;
                BlobStorageService blobStorageService = new BlobStorageService
                {
                    GetConfiguration = _config
                };

                AllBlobItems = await blobStorageService.ListBlobsFlatListingAsync(contianerName.ToLower(), null);

                return Page();
            }

            return RedirectToPage("/Contract/Create");

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            EmploymentQuestionnaire.ContractId = YourContractId;
            EmploymentQuestionnaire.CreatedOn = DateTime.Now;

            _context.EmploymentQuestionnaires.Add(EmploymentQuestionnaire);
            await _context.SaveChangesAsync();

            return Page();
        }
    }
}
