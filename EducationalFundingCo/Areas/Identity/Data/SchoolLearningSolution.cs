using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class SchoolLearningSolution
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public int? LearningSolutionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [ForeignKey(nameof(SchoolId))]
        public virtual School School { get; set; }

        [ForeignKey(nameof(LearningSolutionId))]
        public virtual LearningSolution LearningSolution { get; set; }
    }
}