using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;
        private IAddProductService _addProductService;

        public ProductsController(IProductService productService, IAddProductService addProductService)
        {
            _productService = productService;
            _addProductService = addProductService;
        }

        [HttpGet("getallproductdetails")]
        public IActionResult GetAllProductDetails()
        {
            var result = _productService.GetAllProductsDetail();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("add")]
        public IActionResult Add(AddProduct addProduct)
        {
            var result = _addProductService.Add(addProduct);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}