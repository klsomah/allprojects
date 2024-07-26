using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class OTPVerification
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [MaxLength(250)]
        public string Email { get; set; }
        
        public int OTPCode { get; set; } = 0;
        public DateTime? OTPGeneratedOn { get; set; }
        public DateTime? OTPValidatedOn { get; set; } = null;
    }
}