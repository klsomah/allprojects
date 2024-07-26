using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class EmploymentQuestionnaire
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Display(Name = "Contract")]
        public int? ContractId { get; set; }

        [Display(Name = "Employer Name")]
        public string EmployerName { get; set; }

        public decimal Income { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [Display(Name = "Zip code")]
        public string Zipcode { get; set; }

        [Display(Name = "Employment Start Date ")]
        public DateTime? EmploymentStartDate { get; set; }

        [Display(Name = "HR Contact Person")]
        public string HRContactPerson { get; set; }

        [Display(Name = "HR Contact Number")]
        public string HRContactNumber { get; set; }

        [Display(Name = "Upload Offer Letter")]
        public string OfferLetterLink { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(ContractId))]
        public virtual Contract Contract { get; set; }
    }
}
