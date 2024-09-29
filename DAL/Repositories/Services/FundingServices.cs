using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class FundingServices : IFundingServices
    {
        private readonly PeerlandingContext _peerLandingContext;
        private readonly IUserServices _userServices;
        private readonly ILoanServices _loanServices;

        public FundingServices(PeerlandingContext peerLandingContext, IUserServices userServices, ILoanServices loanServices)
        {
            _peerLandingContext = peerLandingContext;
            _userServices = userServices;
            _loanServices = loanServices;
        }

        public async Task<string> AddFunding(ReqAddFundingDto reqAddFundingDto)
        {
            var newFunding = new TrnFunding
            {
                LoanId = reqAddFundingDto.LoanId,
                LenderId = reqAddFundingDto.LenderId,
                Amount = reqAddFundingDto.Amount
            };

            await _peerLandingContext.AddAsync(newFunding);
            await _peerLandingContext.SaveChangesAsync();

            return newFunding.Id;
        }

        public async Task<string> FundingLoan(ReqFundingLoanDto reqFundingLoanDto)
        {
            var lender = await _userServices.GetUserById(reqFundingLoanDto.LenderId) ?? throw new Exception("Lender not found");
            var loan = await _loanServices.GetLoanById(reqFundingLoanDto.LoanId) ?? throw new Exception("Loan not found");

            if (loan.Amount > lender.Balance)
            {
                throw new Exception("Lender balance insufficient");
            }
            try
            {
                await _loanServices.UpdateLoanById(loan.Id, new ReqUpdateLoanDto
                {
                    Status = "funded"
                });

                var lenderBalance = lender.Balance - loan.Amount;
                await _userServices.UpdateUserById(lender.Id, new ReqUpdateUserDto
                {
                    Name = lender.Name,
                    Role = lender.Role,
                    Balance = lenderBalance
                });

                var borrower = await _userServices.GetUserById(loan.BorrowerId) ?? throw new Exception("Borrower not found");
                var borrowerBalance = borrower.Balance + loan.Amount;
                await _userServices.UpdateUserById(loan.BorrowerId, new ReqUpdateUserDto
                {
                    Name = borrower.Name,
                    Role = borrower.Role,
                    Balance = borrowerBalance
                });

                await AddFunding(new ReqAddFundingDto
                {
                    LenderId = lender.Id,
                    LoanId = loan.Id,
                    Amount = loan.Amount,
                });

                await _peerLandingContext.SaveChangesAsync();
                return loan.Id;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred: {ex.Message}");
            }
        }

        public async Task<List<ResFundingDto>> GetFundings(string lenderId)
        {
            var fundings = await _peerLandingContext.TrnFunding
                .Include(f => f.User)
                .Include(f => f.Loan)
                .Where(f => f.User.Id == lenderId || lenderId == null)
                .OrderByDescending(f => f.FundedAt)
                .Select(funding => new ResFundingDto
                {
                    BorrowerName = funding.Loan.User.Name,
                    LenderName = funding.User.Name,
                    Amount = funding.Amount,
                    FundedAt = funding.FundedAt,
                    Status = funding.Loan.Status
                }).ToListAsync();
            return fundings;
        }
    }
}
