using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResLoanDto
    {
        public string Id { get; set; }
        public string BorrowerId { get; set; }
        public decimal Amount { get; set; }
        public decimal Interestrate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }

    }
}
