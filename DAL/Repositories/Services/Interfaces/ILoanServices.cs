using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan);
        Task<ResLoanDto> GetUserById(string id);
        Task<string> UpdateLoanById(string id, ReqUpdateLoanDto updateLoan);
        Task<List<ResListLoanDto>> LoanList(string status);
    }
}
