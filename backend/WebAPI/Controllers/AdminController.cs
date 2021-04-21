using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAddMoneyService _addMoneyService;
        private IAddProductService _addProductService;

        public AdminController(IAddMoneyService addMoneyService, IAddProductService addProductService)
        {
            _addMoneyService = addMoneyService;
            _addProductService = addProductService;
        }

        [HttpGet("getaddmoney")]
        public IActionResult GetAddMoney()
        {
            var result = _addMoneyService.GetApproved();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("confirmaddmoney")]
        public IActionResult ConfirmAddMoney(AddMoneyDto addMoneyDto)
        {
            var result = _addMoneyService.Confirm(addMoneyDto.AddMoneyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("rejectaddmoney")]
        public IActionResult RejectAddMoney(AddMoneyDto addMoneyDto)
        {
            var result = _addMoneyService.Reject(addMoneyDto.AddMoneyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("getaddproduct")]
        public IActionResult GetAddProduct()
        {
            var result = _addProductService.GetApproved();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("confirmaddproduct")]
        public IActionResult ConfirmAaddProduct(AddProductDto addProductDto)
        {
            var result = _addProductService.Confirm(addProductDto.AddProductId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("rejectaddproduct")]
        public IActionResult RejectAaddProduct(AddProductDto addProductDto)
        {
            var result = _addProductService.Reject(addProductDto.AddProductId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}