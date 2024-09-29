using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/funding/[action]")]
    [ApiController]
    public class FundingController : ControllerBase
    {
        public readonly IFundingServices _fundingServices;

        public FundingController(IFundingServices fundingServices)
        {
            _fundingServices = fundingServices;
        }

        [HttpPost]
        public async Task<IActionResult> FundingLoan(ReqFundingLoanDto funding)
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
                var res = await _fundingServices.FundingLoan(funding);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan funded successfully",
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
        [Route("{lenderId}")]
        public async Task<IActionResult> GetAllFundings(string lenderId)
        {
            try
            {
                var res = await _fundingServices.GetFundings(lenderId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Succes load fundings",
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
    }
}
