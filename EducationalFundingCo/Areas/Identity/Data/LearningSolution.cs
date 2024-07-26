using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class LearningSolution
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}