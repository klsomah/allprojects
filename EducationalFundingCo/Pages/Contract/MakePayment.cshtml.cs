using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EducationalFundingCo.Areas.Identity.Data;
using EducationalFundingCo.Data;
using Microsoft.AspNetCore.Identity;
using Acklann.Plaid;
using Microsoft.Extensions.Configuration;
using EducationalFundingCo.Utilities;
using Stripe;
using Acklann.Plaid.Management;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EducationalFundingCo.Pages.Contract
{
    [Authorize(Roles = AllRoles.StudentEndUser)]
    public class MakePaymentModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly StripeApiFactory _apiFactory;

        public MakePaymentModel(EducationalFundingCoContext context, UserManager<IdentityUser> userManager, IConfiguration config, StripeApiFactory apiFactory)
        {
            _context = context;
            _apiFactory = apiFactory;
            _config = config;
            _userManager = userManager;
        }

        [BindProperty]
        public Areas.Identity.Data.Payment Payment { get; set; }

        [BindProperty]
        public decimal PaymentAmount { get; set; }
        public string PublishableKey { get; set; }

        public string LinkToken { get; set; }

        public bool IsLate { get; set; }

        public List<Areas.Identity.Data.Payment> Payments { get; set; }

        public Areas.Identity.Data.Contract Contract { get; set; }

        public List<Areas.Identity.Data.ConfigValue> ConfigValues { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal LateFees { get; set; }
        public decimal ProcessingFees { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                var userId = _userManager.GetUserId(User);
                var email = _userManager.GetUserName(User);

                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.UserId == userId);

                Payments = await _context.Payments
                   .Include(p => p.AcademyProgram)
                   .Include(p => p.Contract)
                   .Where(p => p.Contract.UserId == userId)
                   .ToListAsync();

                var totalPayment = Payments
                    .Where(p => p.PaymentMethod != null
                       && p.CompleteDate != null
                       && p.TransactionDate != null
                       && p.Status.Trim() == "Succeeded")
                    .Sum(p => p.Amount);

                PaymentBalance = (decimal)(Contract.AcademyProgram.Cap - totalPayment);

                Payments = Payments
                   .Where(p => p.PaymentMethod == null
                       && p.CompleteDate == null
                       && p.TransactionDate == null
                       && p.ScheduledDate < DateTime.Now.AddDays(-4)).ToList();

                Payment = await _context.Payments
                   .Include(p => p.AcademyProgram)
                   .Include(p => p.Contract)
                   .OrderBy(p => p.ScheduledDate)
                   .FirstOrDefaultAsync(p => p.Contract.UserId == userId
                            && p.PaymentMethod == null
                            && p.CompleteDate == null
                            && p.TransactionDate == null);

                int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
                ConfigValues = await _context.ConfigValues.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToListAsync();

                if (ConfigValues != null)
                {
                    ProcessingFees = Convert.ToDecimal(ConfigValues.FirstOrDefault(c => c.KeyPair == "Processing Fees").ValuePair) / 100;
                    LateFees = Convert.ToDecimal(ConfigValues.FirstOrDefault(c => c.KeyPair == "Late Fees").ValuePair) / 100;
                }

                if (Payment != null)
                {
                    if (Payment.ScheduledDate < DateTime.Now.AddDays(-4))
                    {
                        decimal fees = 1 + ProcessingFees + LateFees;
                        foreach (var pay in Payments)
                        {
                            if (pay.ScheduledDate < DateTime.Now.AddDays(-4))
                            {
                                PaymentAmount += Convert.ToInt64(pay.Amount * fees);
                                IsLate = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        decimal fees = 1 + ProcessingFees;
                        PaymentAmount = Convert.ToInt64(Payment.Amount * fees);
                        IsLate = false;
                    }
                }
            }
            else
            {
                Payment = await _context.Payments
                    .Include(p => p.AcademyProgram)
                    .Include(p => p.Contract).FirstOrDefaultAsync(m => m.Id == id);

                if (Payment != null)
                    Payment.Amount = Convert.ToInt64(Payment.Amount * Convert.ToDecimal(1.03));
            }

            PublishableKey = _apiFactory.GetPublishableKey();
            return Page();
        }

        public async Task<IActionResult> OnGetProcessAchPaymentAsync(string publicToken, string bankToken, string student, string amount)
        {
            StringBuilder errorString = new StringBuilder();

            if (string.IsNullOrWhiteSpace(publicToken) || string.IsNullOrWhiteSpace(bankToken))
                errorString.Append("Comunication with bank was not established!");

            // Set your secret key. Remember to switch to your live secret key in production.
            // See your keys here: https://dashboard.stripe.com/apikeys
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new CustomerCreateOptions
            {
                Description = student,
                Source = publicToken, // Get the bank token submitted by the form
            };
            var service = new CustomerService();
            var customer = service.Create(options);

            var options2 = new BankAccountVerifyOptions
            {
                Amounts = new List<long> { 32, 45 },
            };
            var service2 = new BankAccountService();
            service2.Verify(
              customer.Id,
              bankToken,
              options2
            );

            var options1 = new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(amount),
                Currency = "usd",
                Customer = customer.Id,
            };
            var service1 = new ChargeService();
            var charge = service1.Create(options1);

            if (charge.Status == "pending")
            {
                var userId = _userManager.GetUserId(User);
                var email = _userManager.GetUserName(User);
                StringBuilder emailMsg = new StringBuilder();
                decimal paymentTotal = 0;

                var payments = await _context.Payments
                .Include(p => p.AcademyProgram)
                .Include(p => p.Contract).Where(p => p.Contract.UserId == userId && p.PaymentMethod == null && p.CompleteDate == null && p.TransactionDate == null).ToListAsync();

                var payment = _context.Payments
                    .Include(p => p.Contract)
                    .FirstOrDefault(p => p.Contract.UserId == userId && p.PaymentMethod == null && p.CompleteDate == null && p.TransactionDate == null);
               
                int SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
                ConfigValues = await _context.ConfigValues.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToListAsync();

                if (ConfigValues != null)
                {
                    ProcessingFees = Convert.ToDecimal(ConfigValues.FirstOrDefault(c => c.KeyPair == "Processing Fees").ValuePair) / 100;
                    LateFees = Convert.ToDecimal(ConfigValues.FirstOrDefault(c => c.KeyPair == "Late Fees").ValuePair) / 100;
                }

                if (payment != null)
                {
                    if (payment.ScheduledDate < DateTime.Now.AddDays(-4))
                    {
                        decimal fees = 1 + ProcessingFees + LateFees;
                        emailMsg.Append("<ul>");
                        foreach (var pay in payments)
                        {
                            if (pay.ScheduledDate < DateTime.Now.AddDays(-4))
                            {
                                pay.Status = "Pending";
                                //pay.Amount = Convert.ToInt64(pay.Amount * Convert.ToDecimal(1.115));
                                pay.PaymentMethod = "Bank";
                                pay.CompleteDate = DateTime.Now;
                                pay.TransactionDate = DateTime.Now;
                                pay.LateFees = LateFees * pay.Amount;
                                pay.ProcessingFee = ProcessingFees * pay.Amount;
                                //PaymentAmount += Convert.ToInt64(pay.Amount * Convert.ToDecimal(1.115));
                                IsLate = true;

                                emailMsg.Append("<li>" + ((decimal)pay.Amount * Convert.ToDecimal(fees)).ToString("c") + " Late payment</li>");
                                paymentTotal += (decimal)pay.Amount * Convert.ToDecimal(fees);
                                _context.Attach(pay).State = EntityState.Modified;
                            }
                            else
                            {
                                break;
                            }
                        }
                        emailMsg.Append("</ul>");

                    }
                    else
                    {
                        decimal fees = 1 + ProcessingFees;
                        payment.Status = "Pending";
                        //payment.Amount = Convert.ToInt64(payment.Amount * Convert.ToDecimal(1.115));
                        payment.PaymentMethod = "Bank";
                        payment.CompleteDate = DateTime.Now;
                        payment.TransactionDate = DateTime.Now;
                        payment.ProcessingFee = ProcessingFees * payment.Amount;

                        emailMsg.Append("<ul>");
                        emailMsg.Append("<li>" + ((decimal)payment.Amount * Convert.ToDecimal(fees)).ToString("c") + "</li>");
                        emailMsg.Append("</ul>");
                        paymentTotal = (decimal)payment.Amount * Convert.ToDecimal(fees);

                        _context.Attach(payment).State = EntityState.Modified;

                        //PaymentAmount = Convert.ToInt64(payment.Amount * Convert.ToDecimal(1.015));
                        IsLate = false;
                    }
                }

                await _context.SaveChangesAsync();

                Payment = await _context.Payments
                  .Include(p => p.AcademyProgram)
                  .Include(p => p.Contract)
                  .OrderBy(p => p.ScheduledDate)
                  .FirstOrDefaultAsync(p => p.Contract.UserId == userId && p.PaymentMethod == null && p.CompleteDate == null && p.TransactionDate == null);

                Payment.Amount = Convert.ToInt64(Payment.Amount * Convert.ToDecimal(1.015));

                //SendEmailFromGmail sfgmail = new SendEmailFromGmail();

                //sfgmail.SendEmail(Payment.Contract.Email, Payment.Contract.FirstName + " " + Payment.Contract.LastName, "Income Share Agreement",
                //     $"Hello {Payment.Contract.FirstName}, <br/><br/> Thank you for your payment. See summary below:<br/><br/> " +
                //     emailMsg.ToString() +
                //     $"Total:<b> {paymentTotal.ToString("c")} </b><br/><br/>" +
                //    $"All the best,<br/><br/>" +
                //    $"Admissions<br/>" +
                //    $"Education Funding Co.", null);

                EmailSender emailSender = new EmailSender
                {
                    Subject = "Income Share Agreement",
                    ToEmail = Payment.Contract.Email,
                    ToName = Payment.Contract.FirstName + " " + Payment.Contract.LastName,
                    Content = $"Hello {Payment.Contract.FirstName}, <br/><br/> Thank you for your payment. See summary below:<br/><br/> " +
                        emailMsg.ToString() +
                        $"Total:<b> {paymentTotal.ToString("c")} </b><br/><br/>" +
                        $"All the best,<br/><br/>" +
                        $"Admissions<br/>" +
                        $"Education Funding Co."
                };

                await emailSender.SendEmailAsync();
            }

            //return RedirectToPage("/contract/details");
            return new JsonResult(charge);
        }


        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }


        private void OnPostProcessPayment(string publicToken)
        {
            // Set your secret key. Remember to switch to your live secret key in production.
            // See your keys here: https://dashboard.stripe.com/apikeys
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new CustomerCreateOptions
            {
                Description = "Test",//$"Payment By: {Payment.Contract.FirstName} {Payment.Contract.LastName}",
                Source = publicToken, // Get the bank token submitted by the form
            };
            var service = new CustomerService();
            var customer = service.Create(options);

            var options1 = new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(PaymentAmount),
                Currency = "usd",
                Customer = customer.Id,
            };
            var service1 = new ChargeService();
            var result = service1.Create(options1);

           

        }
     
    }
}
