using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducationalFundingCo.Areas.Identity.Data
{
   
    public class ApplicationUser   
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        [MaxLength(450)]
        public string IdentityUserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [ForeignKey(nameof(SchoolId))]
        public virtual School School { get; set; }

        [ForeignKey(nameof(IdentityUserId))]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}
