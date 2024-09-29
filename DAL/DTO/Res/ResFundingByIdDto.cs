using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResFundingByIdDto
    {
        public string BorrowerName {  get; set; }
        public decimal Amount { get; set; }
    }
}
