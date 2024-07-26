using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class Contract
    {
        public Contract()
        {
            Payments = new HashSet<Payment>();
            Communications = new HashSet<Communication>();
        }

        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Display(Name = "Program")]
        public int ProgramId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        public string Email { get; set; }

        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }

        //[Display(Name = "Social (SSN)")]
        //[Required(ErrorMessage = "SSN is Required")]
        //[RegularExpression(@"^\d{9}|\d{3}-\d{2}-\d{4}$", ErrorMessage = "Invalid Social Security Number")]
        public string SocialSecurityNum { get; set; }

        [Display(Name = "Accepted Date")]
        public DateTimeOffset? AcceptedDate { get; set; }

        [Display(Name = "Completed Date")]
        public DateTimeOffset? CompletedDate { get; set; }

        [Display(Name = "Est Completed Date")]
        public DateTimeOffset? EstCompletedDate { get; set; }

        [Display(Name = "Payment Fenquency")]
        public string PaymentFenquency { get; set; }

        [Display(Name = "Max Delinquency Age")]
        public string MaxDelinquencyAge { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }


        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [Display(Name = "Zip code")]
        public string Zipcode { get; set; }

        [Display(Name = "Is Signed")]
        public bool? IsSigned { get; set; }
        public string SignatureUrl { get; set; }
        public string PublicToken { get; set; }
        public string MetaDataArray { get; set; }
        public int? RecordStatus { get; set; }
        public DateTime? DateSigned { get; set; }

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<Communication> Communications { get; set; }

        [ForeignKey(nameof(ProgramId))]
        public virtual AcademyProgram AcademyProgram { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser IdentityUser { get; set; }

        public int? SchoolId { get; set; }
        [ForeignKey(nameof(SchoolId))]
        public virtual School School { get; set; }
    }
}
