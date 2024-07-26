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
using System.Text;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace EducationalFundingCo.Pages.Contract
{
    public class MakePaymentStripeModel : PageModel
    {
        private readonly EducationalFundingCoContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly StripeApiFactory _apiFactory;

        public MakePaymentStripeModel(EducationalFundingCoContext context, 
            UserManager<IdentityUser> userManager, 
            IConfiguration config, 
            StripeApiFactory apiFactory)
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
        public DateTimeOffset NewPaymentDate { get; set; } = new DateTimeOffset(2023, 7, 1, 12, 6, 32,
                                 new TimeSpan(1, 0, 0));

        public List<Areas.Identity.Data.ConfigValue> ConfigValues { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal LateFees { get; set; }
        public decimal ProcessingFees { get; set; }
       
        [Display(Name = "Payment Preference")]
        public string PaymentPreference { get; set; }

        public string PaymentAmountMessage { get; set; }

        public const string PaymentAmountStr = "";
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PaymentAmount = await CalculatePayment(id);
            HttpContext.Session.SetString(PaymentAmountStr, PaymentAmount.ToString());

            PublishableKey = _apiFactory.GetPublishableKey();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string stripeToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if(!string.IsNullOrEmpty(HttpContext.Session.GetString(PaymentAmountStr)))
                {
                    Decimal payAmount = Convert.ToDecimal(HttpContext.Session.GetString(PaymentAmountStr));
                    if (payAmount != PaymentAmount)
                    {
                        PaymentAmountMessage = $"Your payment was not processed. Payment amount ${PaymentAmount} is not valid. The amount due is ${payAmount}.00";
                        PaymentAmount = await CalculatePayment(null);
                        HttpContext.Session.SetString(PaymentAmountStr, PaymentAmount.ToString());
                        return Page();
                    }
                }
                var userId = _userManager.GetUserId(User);
                var email = _userManager.GetUserName(User);
                PublishableKey = _apiFactory.GetPublishableKey();

                StringBuilder emailMsg = new StringBuilder();
                decimal paymentTotal = 0;

                StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
                var amount = PaymentAmount;

                var customerOptions = new CustomerCreateOptions
                {
                    Email = email,
                    Source = stripeToken,
                };

                var customerService = new CustomerService();
                Customer customer = customerService.Create(customerOptions);

                var chargeOptions = new ChargeCreateOptions
                {
                    Customer = customer.Id,
                    Description = "Education Fundation Income Share.",
                    Amount = Convert.ToInt64(amount) * 100,
                    Currency = "usd",
                };
                var chargeService = new ChargeService();
                Charge charge = chargeService.Create(chargeOptions);

                if (charge.Status == "succeeded")
                {
                    paymentTotal = await UpdatePayment(userId, emailMsg, paymentTotal);
                }

                //if (PaymentPreference == "0" || string.IsNullOrWhiteSpace(PaymentPreference))
                //{
                //    var chargeOptions = new ChargeCreateOptions
                //    {
                //        Customer = customer.Id,
                //        Description = "Education Fundation Income Share.",
                //        Amount = Convert.ToInt64(amount) * 100,
                //        Currency = "usd",
                //    };
                //    var chargeService = new ChargeService();
                //    Charge charge = chargeService.Create(chargeOptions);

                //    if (charge.Status == "succeeded")
                //    {
                //        paymentTotal = await UpdatePayment(userId, emailMsg, paymentTotal);
                //    }
                //}
                //else if(PaymentPreference == "1")
                //{
                //    var options = new PriceCreateOptions
                //    {
                //        Nickname = "Ed Fund Recurring Payment",
                //        Product = _config.GetValue<string>("Stripe:RecurringPaymentProduct"),
                //        Active = true,
                //        UnitAmount = Convert.ToInt64(amount) * 100,
                //        Currency = "usd",
                //        Recurring = new PriceRecurringOptions
                //        {
                //            Interval = "month",
                //            UsageType = "licensed",
                //        },
                //    };

                //    var payment = new PriceService();
                //    var price = payment.Create(options);

                //    StripeAutoPay stripeAutoPay = new StripeAutoPay();
                //    stripeAutoPay.Subscription1st(_config["Stripe:SecretKey"], "Payment for the 1st", email, price.Id);
                //    stripeAutoPay.Subscription15th(_config["Stripe:SecretKey"], "Payment for the 15th", email, price.Id);
                //}


            }
            catch(StripeException e) {
                switch (e.StripeError.Type)
                {
                    case "card_error":
                        Console.WriteLine("Code: " + e.StripeError.Code);
                        Console.WriteLine("Message: " + e.StripeError.Message);
                        break;
                    case "api_connection_error":
                        break;
                    case "api_error":
                        break;
                    case "authentication_error":
                        break;
                    case "invalid_request_error":
                        break;
                    case "rate_limit_error":
                        break;
                    case "validation_error":
                        break;
                    default:
                        // Unknown Error Type
                        break;
                }
            }
            return RedirectToPage("/contract/details");
        }

        private async Task<decimal> UpdatePayment(string userId, StringBuilder emailMsg, decimal paymentTotal)
        {
            var payments = await _context.Payments
            .Include(p => p.AcademyProgram)
            .Include(p => p.Contract).Where(p => p.Contract.UserId == userId && p.PaymentMethod == null && p.CompleteDate == null && p.TransactionDate == null).ToListAsync();

            var payment = _context.Payments
                .Include(p => p.Contract)
                .FirstOrDefault(p => p.Contract.UserId == userId && p.PaymentMethod == null && p.CompleteDate == null && p.TransactionDate == null);

            int? SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));
            if (SchoolId == 0)
                SchoolId = payment.Contract.SchoolId;
            ConfigValues = await _context.ConfigValues.Include(x => x.School).Where(x => x.SchoolId == SchoolId).ToListAsync();

            if (ConfigValues != null && ConfigValues.Count > 0)
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
                            pay.Status = "Succeeded";
                            //pay.Amount = Convert.ToInt64(pay.Amount * Convert.ToDecimal(1.115));
                            pay.PaymentMethod = "Card";
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
                    payment.Status = "Succeeded";
                    //payment.Amount = Convert.ToInt64(payment.Amount * Convert.ToDecimal(1.115));
                    payment.PaymentMethod = "Card";
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
            return paymentTotal;
        }

        private async Task<decimal> CalculatePayment(int? id)
        {
            decimal paymentAmount = 0;

            if (id == null)
            {
                var userId = _userManager.GetUserId(User);
                var email = _userManager.GetUserName(User);

                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.UserId == userId);
            }
            else
            {
                Contract = _context.Contracts.Include(a => a.AcademyProgram).FirstOrDefault(c => c.Id == id);
            }

            Payments = await _context.Payments
                .Include(p => p.AcademyProgram)
                .Include(p => p.Contract)
                .Where(p => p.Contract.Id == Contract.Id)
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
                .FirstOrDefaultAsync(p => p.Contract.Id == Contract.Id
                        && p.PaymentMethod == null
                        && p.CompleteDate == null
                        && p.TransactionDate == null);

            var SchoolId = Convert.ToInt32(HttpContext.Session.GetString("SchoolId"));

            if (SchoolId == 0)
                SchoolId = (int)Contract.SchoolId;


            if (string.IsNullOrEmpty(SchoolId.ToString()))
            {
                ConfigValues = await _context.ConfigValues.ToListAsync();
            }
            else
            {
                int parsedSchoolId = Convert.ToInt32(SchoolId);
                ConfigValues = await _context.ConfigValues.Include(x => x.School)
                                                            .Where(x => x.SchoolId == parsedSchoolId)
                                                            .ToListAsync();
            }


            if (ConfigValues != null && ConfigValues.Count > 0)
            {
                ProcessingFees = Convert.ToDecimal(ConfigValues.FirstOrDefault(c => c.KeyPair == "Processing Fees")?.ValuePair) / 100;
                LateFees = Convert.ToDecimal(ConfigValues.FirstOrDefault(c => c.KeyPair == "Late Fees")?.ValuePair) / 100;
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
                            paymentAmount += Convert.ToInt64(pay.Amount * fees);
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
                    paymentAmount = Convert.ToInt64(Payment.Amount * fees);
                    IsLate = false;
                }
            }

            return paymentAmount;
        }


    }
}
