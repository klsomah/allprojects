using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Data;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using EducationalFundingCo.Utilities;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using EducationalFundingCo.Areas.Identity.Data;

namespace EducationalFundingCo.Pages.Contract
{
    public class PreviewContractModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly IConfiguration _config;

        public PreviewContractModel(EducationalFundingCoContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        [BindProperty]
        public Areas.Identity.Data.Contract Contract { get; set; }

        public Areas.Identity.Data.EmploymentQuestionnaire EmploymentQuestionnaire { get; set; }

        [BindProperty]
        public Areas.Identity.Data.Payment Payment { get; set; }

        public decimal? PaymentAmount { get; set; }

        public decimal? PaymentTotal { get; set; }

        public DateTime? PaymentStart { get; set; }

        [TempData]
        public int PreviewId { get; set; }

        [TempData]
        public string PreviewMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //PreviewId = 0;
            PreviewMessage = string.Empty;

            if (id == null)
            {
                return NotFound();
            }

            Contract = await _context.Contracts
                .Include(c => c.AcademyProgram)
                .Include(c => c.IdentityUser).FirstOrDefaultAsync(m => m.Id == id);

            PreviewId = Contract.Id;

            if (Contract == null)
            {
                return NotFound();
            }

            return await LoadData(Contract.Id);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostUpdateContractAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var contract = await _context.Contracts
                .Include(c => c.AcademyProgram)
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == Contract.Id);

            if(Contract.AcceptedDate != null)
                contract.AcceptedDate = Contract.AcceptedDate;
            if (Contract.EstCompletedDate != null)
                contract.EstCompletedDate = Contract.EstCompletedDate;
            if (Contract.CompletedDate != null)
                contract.CompletedDate = Contract.CompletedDate;
            if (Contract.PaymentStatus != null && Contract.PaymentStatus=="Active")
            {
                contract.PaymentStatus = Contract.PaymentStatus;

                contract.RecordStatus = 1;
            }
            if (Contract.PaymentStatus != null && Contract.PaymentStatus == "Suspended")
            {
                contract.PaymentStatus = Contract.PaymentStatus;

                contract.RecordStatus = 3;
            }
            if (Contract.PaymentStatus != null && Contract.PaymentStatus == "Completed")
            {
                contract.PaymentStatus = Contract.PaymentStatus;

                contract.RecordStatus =2;
            }






            _context.Attach(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                PreviewMessage = "Success : Update was successful!";
            }
            catch (DbUpdateConcurrencyException)
            {
                PreviewMessage = "Error : An error ocurred!";
                if (!ContractExists(contract.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return await LoadData(contract.Id);
            //return Page();
        }

        public async Task<IActionResult> OnPostAddPaymentAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var contract = await _context.Contracts
                 .Include(c => c.AcademyProgram)
                 .Include(c => c.IdentityUser)
                 .FirstOrDefaultAsync(m => m.Id == Contract.Id);

            DateTime ScheduleRecurrent = new DateTime();
            decimal paymentAmount = 0;

            if (contract != null)
            {
                //int terms = Convert.ToInt32(contract.AcademyProgram.Duration.Split(' ')[0].Trim());
                int terms = Convert.ToInt32(contract.AcademyProgram.Cap / Payment.Amount);

                for (int i = 0; i < terms; i++) 
                {
                    Payment.ContractId = contract.Id;
                    Payment.ProgramId = contract.ProgramId;
                    Payment.Status = "Scheduled";
                    Payment.Income = 1;

                    if (i > 0)
                    {
                        if(ScheduleRecurrent.Day == 1)
                            ScheduleRecurrent = new DateTime(ScheduleRecurrent.Year, ScheduleRecurrent.Month, 15);
                        else if(ScheduleRecurrent.Day == 15 && ScheduleRecurrent.Month < 12)
                            ScheduleRecurrent = new DateTime(ScheduleRecurrent.Year, ScheduleRecurrent.Month + 1, 1);
                        else if (ScheduleRecurrent.Day == 15 && ScheduleRecurrent.Month == 12)
                            ScheduleRecurrent = new DateTime(ScheduleRecurrent.Year + 1, 1, 1);

                        Payment.ScheduledDate = ScheduleRecurrent;
                        Payment.Amount = paymentAmount;
                    }

                    _context.Payments.Add(Payment);

                    try
                    {
                        await _context.SaveChangesAsync();
                        ScheduleRecurrent = Payment.ScheduledDate.HasValue ? Payment.ScheduledDate.Value.DateTime : DateTime.Now;
                        paymentAmount = (decimal)Payment.Amount;
                        Payment = new Areas.Identity.Data.Payment();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        PreviewMessage = "Error : An error ocurred!";
                        if (!ContractExists(contract.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }


                }
                PreviewMessage = "Success : Update was successful!";
            }

            return await LoadData(contract.Id);

           // return RedirectToPage("./Index");
        }

        private async Task<IActionResult> LoadData(int id)
        {
            Contract = await _context.Contracts
               .Include(c => c.AcademyProgram)
               .Include(c => c.IdentityUser).FirstOrDefaultAsync(m => m.Id == id);

            if (Contract == null)
            {
                return NotFound();
            }

            EmploymentQuestionnaire = await _context.EmploymentQuestionnaires.OrderByDescending(e => e.Id).FirstOrDefaultAsync(e => e.ContractId == Contract.Id);

            if (EmploymentQuestionnaire != null && EmploymentQuestionnaire.Income > 0)
                PaymentAmount = ((Contract.AcademyProgram.IncomePercentage / 100) * EmploymentQuestionnaire.Income) / 24;

            if (EmploymentQuestionnaire != null)
            {
                PaymentStart = EmploymentQuestionnaire.EmploymentStartDate.HasValue ? EmploymentQuestionnaire.EmploymentStartDate.Value.AddDays(14) : (DateTime?)null;
                if (PaymentStart.Value.Day > 15 && PaymentStart.Value.Month < 12)
                    PaymentStart = new DateTime(PaymentStart.Value.Year, PaymentStart.Value.Month + 1, 1);
                else if (PaymentStart.Value.Day > 15 && PaymentStart.Value.Month == 12)
                    PaymentStart = new DateTime(PaymentStart.Value.Year + 1, 1, 1);
                else
                    PaymentStart = new DateTime(PaymentStart.Value.Year, PaymentStart.Value.Month, 15);

                await GetEmploymentLetterAsync();
            }

            return Page();
        }

        private async Task GetEmploymentLetterAsync()
        {
            AzureBlobStorage azureBlobStorage = new AzureBlobStorage();
            var contianerName = $"{Contract.FirstName}-{Contract.LastName}-{Contract.Id}";
            azureBlobStorage.ContainerName = contianerName.ToLower();
            azureBlobStorage.GetConfiguration = _config;
            var blobList = azureBlobStorage.GetBlobItems();

            if (blobList != null)
            {
                await foreach (BlobItem item in blobList)
                {
                    if (item.Name.StartsWith("OfferLetter"))
                    {
                        BlobClient blob = azureBlobStorage.GetBlobContainerClient.GetBlobClient(item.Name);
                        EmploymentQuestionnaire.OfferLetterLink = blob.Uri.AbsoluteUri;
                        //break;
                    }
                }
            }
        }
        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
