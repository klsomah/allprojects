using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Areas.Identity.Data
{
    public class Communication
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Display(Name = "Contract")]
        public int ContractId { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string Message { get; set; }

        [Display(Name = "Message Type")]
        public string MessageType { get; set; }
        public string Meduim { get; set; }

        [Display(Name = "Read Status")]
        public bool ReadStatus { get; set; }

        [ForeignKey(nameof(ContractId))]
        public virtual Contract Contract { get; set; }
        public int? SchoolId { get; set; }
        [ForeignKey(nameof(SchoolId))]
        public virtual School School { get; set; }

    }
}
