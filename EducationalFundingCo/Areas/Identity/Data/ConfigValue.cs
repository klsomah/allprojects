using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class ConfigValue
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        public string KeyPair { get; set; }
        public string ValuePair { get; set; }
        public int? SchoolId { get; set; }
        [ForeignKey(nameof(SchoolId))]
        public virtual School School { get; set; }

    }
}
