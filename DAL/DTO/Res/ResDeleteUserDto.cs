using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResDeleteUserDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string DeletedUserId { get; set; }
    }
}
