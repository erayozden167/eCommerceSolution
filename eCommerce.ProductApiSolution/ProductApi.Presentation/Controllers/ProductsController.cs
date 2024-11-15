using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversion;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products detected.");
            var (_, list) = ProductConversion.FromEntity(null, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.GetAsync(id);
            if (product is null)
                return NotFound("Product not found.");
            var (_product, _) = ProductConversion.FromEntity(product, null);
            return _product is not null ? Ok(_product) : NotFound("Product not found");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var entity = ProductConversion.ToEntity(productDTO);
            var response = await productInterface.CreateAsync(entity);
            return response.Flag is true? Ok(response) : BadRequest(response);
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var entity = ProductConversion.ToEntity(productDTO);
            var response = await productInterface.UpdateAsync(entity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpDelete]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO productDTO)
        {
            var entity = ProductConversion.ToEntity(productDTO);
            var response = await productInterface.DeleteAsync(entity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
