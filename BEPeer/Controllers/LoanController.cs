using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        public readonly ILoanServices _loanServices;
        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation errors occured");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                var res = await _loanServices.CreateLoan(loan);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan created successfully",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var loan = await _loanServices.GetLoanById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Loan fetced successfully",
                    Data = loan
                });
            }
            catch (ResErrorDto ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }

        }


        //[Authorize(Roles = "borrower")]
        [HttpGet]
        [Route("{borrowerId}")]
        public async Task<IActionResult> GetLoanByBorrowerId(string borrowerId)
        {
            try
            {
                var loan = await _loanServices.GetLoanByBorrowerId(borrowerId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Loan fetced successfully",
                    Data = loan
                });
            }
            catch (ResErrorDto ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        //[Authorize(Roles = "lender")]
        [HttpGet]
        public async Task<IActionResult> GetAllLoans()
        {
            try
            {
                var res = await _loanServices.LoanList();
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Succes load loan",
                    Data = res
                });
            }
            catch (ResErrorDto ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLoans([FromQuery] string status = null)
        {
            try
            {
                var res = await _loanServices.GetLoans(status);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Succes load loan",
                    Data = res
                });
            }
            catch (ResErrorDto ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateLoanById(string id, ReqUpdateLoanDto updateLoan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();

                    var errorMessage = new StringBuilder("Validation error occured!");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                await _loanServices.GetLoanById(id);
                var user = await _loanServices.UpdateLoanById(id, updateLoan);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Loan updated successfully",
                    Data = user
                });
            }
            catch (ResErrorDto ex)
            {
                return StatusCode(ex.StatusCode, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
