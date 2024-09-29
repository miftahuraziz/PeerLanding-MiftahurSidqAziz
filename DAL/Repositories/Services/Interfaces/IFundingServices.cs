using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IFundingServices
    {
        Task<string> AddFunding(ReqAddFundingDto reqAddFundingDto);
        Task<string> FundingLoan(ReqFundingLoanDto reqFundingLoanDto);
        Task<List<ResFundingDto>> GetFundings();
    }
}
