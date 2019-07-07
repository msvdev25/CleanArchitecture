using Cart.Domain;
using Cart.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RS2.Core;
using System;
using System.Threading.Tasks;

namespace Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;
        private IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment env;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _productService.GetProducts());
        }

        // GET: api/Products/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product != null)
                    return Ok(product);
                else
                    throw new Exception("Not Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            var id = await _productService.SaveProduct(product);

            if (id > 0)
            {
                return Ok(id);
            }
            else
            {
                return StatusCode(500, "Unable to save the data");
            }

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {

            if( await _productService.DeleteProduct(id))
                return Ok();

            return StatusCode(500, "Unable to deleted the product");

        }
    }
}
