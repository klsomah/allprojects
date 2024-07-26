using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using EducationalFundingCo.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace EducationalFundingCo.Pages
{
    [Authorize(Roles = "Student, Administrator , SchoolAdministrator" )]
    public class IndexModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(EducationalFundingCoContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Areas.Identity.Data.Contract> Contracts { get; set; }
        public IList<Areas.Identity.Data.Payment> Payments { get; set; }
        public IList<Areas.Identity.Data.EmploymentQuestionnaire> EmploymentQuestionnaire { get; set; }
        public IList<Areas.Identity.Data.School> School { get; set; }
        public PaginatedList<Areas.Identity.Data.Payment> PagedContract { get; set; }
        public int ActiveContracts { get; set; }
        public int SuspendedContracts { get; set; }
        public int FulfilledContracts { get; set; }
        public decimal YearToDatePayment { get; set; }
        public decimal YearToDate { get; set; }
        public int OverallPayment { get; set; }
        public int ContractId { get; set; }
        public string ChartData { get; set; }
        public decimal SelectedTotal { get; set; }
        public decimal SelectedTotalLate { get; set; }
        public decimal SelectedTotalFee { get; set; }
        public IList<Areas.Identity.Data.Payment> LatePayments { get; set; }
        public IList<Areas.Identity.Data.School> Schools { get; set; }
        public int NumOfMonths { get; set; }

        public int DdlValue { get; set; }
        public string UserId { get; set; }

        public int SelectedSch { get; set; }
        public string SelectedSchName { get; set; }
        public int SchoolId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            
            if (User.IsInRole(AllRoles.StudentEndUser))
                return RedirectToPage("/Contract/Details");
            try
            {
                
                SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
                Schools = _context.School.OrderByDescending(s => s.Name).ToList();

                if(SchoolId == 0)
                {
                    UserId = _userManager.GetUserId(User);

                    var getschoolId = await _context.ApplicationUser
                      .Where(x => x.IdentityUserId == UserId).Select(x => x.SchoolId).FirstOrDefaultAsync();

                    if (getschoolId != null && getschoolId > 0)
                    {
                        SchoolId = (int)getschoolId;
                    }
                }

                if (SchoolId != 0)
                {
                    var contracts = await _context.Contracts
                        .Include(c => c.AcademyProgram)
                        .Include(s => s.School)
                        .Include(c => c.IdentityUser).Where(x => x.SchoolId == SchoolId).ToListAsync();

                    Contracts = contracts
                        .OrderBy(c => c.CompletedDate)
                        .OrderBy(c => c.EstCompletedDate)
                        .Where(c => c.PaymentStatus == "Active" && c.SchoolId == SchoolId)
                        .ToList();


                    ActiveContracts = contracts.Where(c => c.PaymentStatus == "Active" && c.SchoolId == SchoolId).Count();
                    SuspendedContracts = contracts.Where(c => c.PaymentStatus == "Suspended" && c.SchoolId == SchoolId).Count();
                    FulfilledContracts = contracts.Where(c => c.PaymentStatus == "Completed" && c.SchoolId == SchoolId).Count();

                    NumOfMonths = 30;
                    ConfigureChart(NumOfMonths);

                }
                else if (SchoolId == 0)
                {

                    var contracts = await _context.Contracts
                     .Include(c => c.AcademyProgram)
                     .Include(c => c.IdentityUser)
                     .Include(s => s.School)
                      .ToListAsync();

                    Contracts = contracts
                        .OrderBy(c => c.CompletedDate)
                        .OrderBy(c => c.EstCompletedDate)
                        .Where(c => c.PaymentStatus == "Active")
                        .ToList();


                    //ActiveContracts = contracts.Where(c => c.PaymentStatus == "Active" ).Count();
                    //SuspendedContracts = contracts.Where(c => c.PaymentStatus == "Suspended").Count();
                    //FulfilledContracts = contracts.Where(c => c.PaymentStatus == "Completed").Count();

                    ActiveContracts = contracts.Where(c => c.PaymentStatus == "Active").Count();
                    SuspendedContracts = contracts.Where(c => c.PaymentStatus == "Suspended").Count();
                    FulfilledContracts = contracts.Where(c => c.PaymentStatus == "Completed").Count();

                    NumOfMonths = 30;
                    ConfigureChart(NumOfMonths);
                }

                SelectedSch = SchoolId;
                SelectedSchName = Schools.FirstOrDefault(n => n.Id == SelectedSch).Name;

            }
            catch (SqlException ex)
            {
                var err = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (User.IsInRole(AllRoles.StudentEndUser))
                return RedirectToPage("/Contract/Details");
            try
            {
                await PostData(id);

            }
            catch (SqlException ex)
            {
                var err = ex.Message;
            }

            return Page();
        }

        private async Task PostData(int id)
        {
            SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
            Schools = _context.School.OrderByDescending(s => s.Name).ToList();

            var userId = _userManager.GetUserId(User);
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
                var contracts = await _context.Contracts.Include(c => c.AcademyProgram)
                    .Include(c => c.IdentityUser).Where(x => x.SchoolId == SchoolId).ToListAsync();

                Contracts = contracts
                    .OrderBy(c => c.CompletedDate)
                    .OrderBy(c => c.EstCompletedDate)
                    .Where(c => c.PaymentStatus == "Active" && c.SchoolId == SchoolId)
                    .ToList();

                ActiveContracts = contracts.Where(c => c.PaymentStatus == "Active" && c.SchoolId == SchoolId).Count();
                SuspendedContracts = contracts.Where(c => c.PaymentStatus == "Suspended" && c.SchoolId == SchoolId).Count();
                FulfilledContracts = contracts.Where(c => c.PaymentStatus == "Completed" && c.SchoolId == SchoolId).Count();

                ConfigureChart(id);


            }
            else if (SchoolId == 0)
            {
                var contracts = await _context.Contracts.Include(c => c.AcademyProgram).Include(c => c.IdentityUser).ToListAsync();

                Contracts = contracts
                    .OrderBy(c => c.CompletedDate)
                    .OrderBy(c => c.EstCompletedDate)
                    .Where(c => c.PaymentStatus == "Active")
                    .ToList();


                ActiveContracts = contracts.Where(c => c.PaymentStatus == "Active").Count();
                SuspendedContracts = contracts.Where(c => c.PaymentStatus == "Suspended").Count();
                FulfilledContracts = contracts.Where(c => c.PaymentStatus == "Completed").Count();

                ConfigureChart(id);
            }

            SelectedSch = SchoolId;
            SelectedSchName = Schools.FirstOrDefault(n => n.Id == SelectedSch).Name;
        }

        public IActionResult OnPostChartdataRefresh(int days)
        {
            DdlValue = days;
            return ConfigureChart(days);
        }
        
        private  JsonResult ConfigureChart(int days)
        {
            if(SchoolId == 0)
                SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));

            if (SchoolId == 0)
            {
                UserId = _userManager.GetUserId(User);

                int? getschoolId = _context.ApplicationUser
                  .Where(x => x.IdentityUserId == UserId).Select(x => x.SchoolId).FirstOrDefault();

                if (getschoolId != null && getschoolId > 0)
                {
                    SchoolId = (int)getschoolId;
                }
            }

            if (SchoolId != 0)
            {
                Payments = _context.Payments.Include(x => x.Contract.School).Where(x => x.Contract.SchoolId == SchoolId).ToList();
            }
            else if (SchoolId == 0)
            {
                Payments = _context.Payments.Include(x => x.Contract.School).Where(x => x.Contract.School.RecordStatus == 2).ToList();
            }
           
            YearToDate = Payments.Where(a => a.ContractId == ContractId).Sum(p => (decimal)p.Amount);
            OverallPayment = Payments.Count;

            var paymentSummaries = new List<PaymentData>();
            YearToDatePayment = Payments.Where(p => p.CompleteDate != null && p.Status == "Succeeded").Sum(p => (decimal)p.Amount);
            NumOfMonths = days;
            var cPayments = Payments.Where(p => p.ScheduledDate.Value >= DateTime.Now.AddDays(-NumOfMonths) && p.ScheduledDate.Value <= DateTime.Now && p.Contract.PaymentStatus == "Active");

            PaymentData paymentData = new PaymentData
            {
                History = "Total Due",
                Amount = cPayments.Sum(p => (decimal)p.Amount)
            };
            SelectedTotal = paymentData.Amount;
            SelectedTotalFee = cPayments.Where(p => p.ProcessingFee.HasValue).Sum(p => (decimal)p.ProcessingFee);
            SelectedTotalFee += cPayments.Where(p => p.LateFees.HasValue).Sum(p => (decimal)p.LateFees);
            paymentSummaries.Add(paymentData);

            paymentData = new PaymentData
            {
                History = "Total Payments",
                Amount = cPayments.Where(p => p.CompleteDate != null && p.Status == "Succeeded").Sum(p => (decimal)p.Amount)
            };
            paymentSummaries.Add(paymentData);

            paymentData = new PaymentData
            {
                History = "Total Late Payments",
                Amount = cPayments.Where(p => p.CompleteDate != null && p.Status == "Succeeded" && p.ScheduledDate.Value.AddDays(4) < p.CompleteDate).Sum(p => (decimal)p.Amount)
            };
            paymentSummaries.Add(paymentData);

            paymentData = new PaymentData
            {
                History = "Late Unpaid",
                Amount = cPayments.Where(p => p.CompleteDate == null && p.Status == "Scheduled" && p.ScheduledDate.Value.AddDays(3) < DateTime.Now.Date && p.Contract.PaymentStatus == "Active").Sum(p => (decimal)p.Amount)
            };
            paymentSummaries.Add(paymentData);
            SelectedTotalLate = paymentData.Amount;

            LatePayments = cPayments.Where(p => p.CompleteDate == null && p.Status == "Scheduled" && p.ScheduledDate.Value.AddDays(4) < DateTime.Now.Date && p.Contract.PaymentStatus == "Active").ToList();

            ChartData = JsonConvert.SerializeObject(paymentSummaries);

            return new JsonResult(ChartData);
        }

        public async Task<IActionResult> OnPostDropDownChange(int id)
        {

            if (User.IsInRole(AllRoles.StudentEndUser))
                return RedirectToPage("/Contract/Details");
            try
            {

                SchoolId = id;//Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
                HttpContext.Session.SetString("SchoolId", id.ToString());

                await PostData(30);

            }
            catch (SqlException ex)
            {
                var err = ex.Message;
            }

            return Page();
        }

    }

}
