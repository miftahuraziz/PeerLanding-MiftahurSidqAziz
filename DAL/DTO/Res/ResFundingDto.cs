using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResFundingDto
    {
        public string BorrowerName { get; set; }
        public string LenderName { get; set; }
        public decimal Amount { get; set; }
        public DateTime FundedAt { get; set; }
        public string Status { get; set; }
    }
}
