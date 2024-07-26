using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class USState
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}