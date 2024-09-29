using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerLandingContext;
        public LoanServices(PeerlandingContext peerlandingContext)
        {
            _peerLandingContext = peerlandingContext;
        }
        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                //InterestRate = loan.InterestRate,
                //Duration = loan.Duration,
            };

            await _peerLandingContext.AddAsync(newLoan);
            await _peerLandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public async Task<List<ResListLoanByBorrowerDto>> GetLoanByBorrowerId(string borrowerId)
        {
            var loans = await _peerLandingContext.MstLoans
                .Where(l => l.BorrowerId == borrowerId || borrowerId == null)
                .OrderByDescending(l => l.CreatedAt)
                .Select(loan => new ResListLoanByBorrowerDto
                {
                    LoanId = loan.Id,
                    InterestRate = loan.InterestRate,
                    Amount = loan.Amount,
                    Status = loan.Status,
                    Duration = loan.Duration,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt
                }).ToListAsync();
            return loans;
        }

        public async Task<ResLoanDto> GetLoanById(string id)
        {
            var loan = await _peerLandingContext.MstLoans.SingleOrDefaultAsync(x => x.Id == id);
            if (loan == null)
            {
                throw new ResErrorDto
                {
                    Data = null,
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var result = new ResLoanDto
            {
                Id = id,
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                Interestrate = loan.InterestRate,
                Duration = loan.Duration,
                Status = loan.Status
            };
            return result;
        }

        public async Task<List<ResListLoanDto>> GetLoans(string status)
        {
            var loans = await _peerLandingContext.MstLoans
                .Include(l => l.User)
                .Where(l => l.Status == status || status == null)
                .OrderByDescending(l => l.CreatedAt)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    InterestRate = loan.InterestRate,
                    Amount = loan.Amount,
                    Status = loan.Status,
                    Duration = loan.Duration
                }).ToListAsync();
            return loans;
        }

        public async Task<List<ResListLoanDto>> LoanList()
        {
            var loans = await _peerLandingContext.MstLoans
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    BorrowerName = loan.User.Name,
                    InterestRate = loan.InterestRate,
                    Amount = loan.Amount,
                    Duration = loan.Duration,
                    Status = loan.Status
                }).ToListAsync();
            return loans;

        }

        public async Task<string> UpdateLoanById(string id, ReqUpdateLoanDto updateLoan)
        {
            var loan = await _peerLandingContext.MstLoans.SingleOrDefaultAsync(x => x.Id == id);
            if (loan == null)
            {
                throw new ResErrorDto
                {
                    Message = "Loan not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            loan.Status = updateLoan.Status;
            loan.UpdatedAt = DateTime.UtcNow;
            await _peerLandingContext.SaveChangesAsync();
            return loan.BorrowerId;
        }
    }
}
