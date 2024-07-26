using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class School
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime? ModifiedOn { get; set; }
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [MaxLength(250)]
        public string Email { get; set; }
        [MaxLength(200)]
        [Display(Name = "School Name")]
        public string Name { get; set; }
        public bool IsBasedInUS { get; set; }
        [MaxLength(250)]
        public string Address1 { get; set; }
        [MaxLength(250)]
        [AllowNull]
        public string Address2 { get; set; }
        [MaxLength(150)]
        public string City { get; set; }
        [MaxLength(150)]
        [AllowNull]
        public string State { get; set; } = null;
        [AllowNull]
        public int? USStateId { get; set; }
        [AllowNull]
        public string ZipCode { get; set; }
        public bool IsProspectiveStudent { get; set; } = false;
        [MaxLength(100)]
        [AllowNull]
        [Display(Name = "CPS First Name")]
        public string CPSFirstName { get; set; } = null;
        [MaxLength(100)]
        [AllowNull]
        [Display(Name = "CPS Last Name")]
        public string CPSLastName { get; set; } = null;
        [MaxLength(250)]
        [AllowNull]
        [Display(Name = "CPS Email")]
        public string CPSEmail { get; set; } = null;
        public bool IsCPSBasedInUS { get; set; } = false;
        [MaxLength(200)]
        [AllowNull]
        [Display(Name = "CPS Name")]
        public string CPSName { get; set; } = null;
        [MaxLength(200)]
        [AllowNull]
        [Display(Name = "CPS Program")]
        public string CPSProgram { get; set; } = null;
        public int RecordStatus { get; set; }
        [MaxLength(150)]
        public string Status { get; set; }
        public string Program { get; set; } = null;

        //public bool IsAccredited { get; set; }
        //public string Accreditation { get; set; }
        //public string PrimaryContact { get; set; }


        [ForeignKey(nameof(USStateId))]
        public virtual USState USState { get; set; }

    }
}