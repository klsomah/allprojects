using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class Payment
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Display(Name = "Contract")]
        public int? ContractId { get; set; }

        [Display(Name = "Program")]
        public int? ProgramId { get; set; }
        public string Status { get; set; }

        [Display(Name = "Scheduled Date")]
        public DateTimeOffset? ScheduledDate { get; set; }
        public decimal? Income { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ProcessingFee { get; set; }
        public decimal? LateFees { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Complete Date")]
        public DateTimeOffset? CompleteDate { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTimeOffset? TransactionDate { get; set; }

        //public int? SchoolId { get; set; }
        //[ForeignKey(nameof(SchoolId))]
        //public virtual School School { get; set; }


        [ForeignKey(nameof(ContractId))]
        public virtual Contract Contract { get; set; }

        [ForeignKey(nameof(ProgramId))]
        public virtual AcademyProgram AcademyProgram { get; set; }
    }
}
