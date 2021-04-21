using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private IWalletService _walletService;
        private IAddMoneyService _addMoneyService;

        public WalletController(IWalletService walletService, IAddMoneyService addMoneyService)
        {
            _walletService = walletService;
            _addMoneyService = addMoneyService;
        }

        [HttpPost("add")]
        public IActionResult Add(AddMoney addMoney)
        {
            var result = _addMoneyService.Add(addMoney);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        [HttpGet("getbyuserId")]
        public IActionResult GetByUserId(int userId)
        {
            var result = _walletService.GetByUserId(userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}