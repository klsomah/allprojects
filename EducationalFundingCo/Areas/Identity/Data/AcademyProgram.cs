using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class AcademyProgram
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Display(Name = "Program Title")]
        public string ProgramName { get; set; }

        [Display(Name = "Application Link")]
        public string ApplicationLink { get; set; }

        [Display(Name = "Income %")]
        public decimal IncomePercentage { get; set; }
        public string Duration { get; set; }

        [Display(Name = "Payment Cap")]
        public decimal Cap { get; set; }

        [Display(Name = "Min Income")]
        public decimal MinIncome { get; set; }
        public string Approval { get; set; }

        [Display(Name = "Max Deferment Period")]
        public string MaxDefermentPeriod { get; set; }

        [Display(Name = "Start Collecting")]
        public string StartCollecting { get; set; }

        public int? SchoolId { get; set; }
        [ForeignKey(nameof(SchoolId))]
        public virtual School School { get; set; }
    }
}
