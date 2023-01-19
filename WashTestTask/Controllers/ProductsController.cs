using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using Data.Dtos;
using Data.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<ProductDTO>> GetProducts()
        {
            _logger.LogInformation("Getting all products.");

            var products = _productService.GetAll().Select(_productService.ToDto);

            return Ok(products);
        }
        
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductByIdAsync(int id)
        {
            _logger.LogInformation($"Getting product with id {id}.");
            
            var product = await _productService.GetAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} not found.");
                return NotFound();
            }

            return Ok(_productService.ToDto(product));
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductDTO productDto)
        {
            _logger.LogInformation("Creating new product.");
            
            var product = new Product
            {
                Title = productDto.Name,
                Price = productDto.Price
            };
            product = await _productService.AddAsync(product);
            await _publishEndpoint.Publish<ProductCreated>(new
            {
                Id = product.Id, 
                Title = product.Title, 
                Price = product.Price
            });

            return CreatedAtAction("", new { id = product.Id }, productDto);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> PutProduct(int id, [FromBody] ProductDTO productDto)
        {
            _logger.LogInformation($"Updating product with id {id}.");
            
            var product = await _productService.GetAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} not found.");
                return NotFound();
            }
            await _productService.PutAsync(product, productDto);
            await _publishEndpoint.Publish<ProductUpdated>(new { Id = product.Id });

            return Ok(_productService.ToDto(product));
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation($"Deleting product with id {id}.");

            var product = await _productService.GetAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} not found.");
                return NotFound();
            }
            await _productService.RemoveAsync(product);

            return NoContent();
        }
    }
}