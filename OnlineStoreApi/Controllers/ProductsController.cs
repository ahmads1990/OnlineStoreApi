using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineStoreApi.Dtos;

namespace OnlineStoreApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        // Get All
        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Seri Log is Working");

            var allProducts = await _productService.GetAllProductsAsync();
            if (allProducts.IsNullOrEmpty())
                return Ok("No products for selling");

            _logger.LogInformation("Returned AllProducts");
            return Ok(allProducts);
        }
        // Get by id
        [HttpGet("GetById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product is null)
                return NotFound("EntityDoesntExist");

            _logger.LogInformation("Returned Product By Id");
            return Ok(product);
        }
        // Add new product
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(AddNewProductDto addNewProductDto)
        {
            try
            {
                // Map dto to model and add user(seller) id into the new model
                var newProduct = new Product
                {
                    Name = addNewProductDto.Name,
                    Price = addNewProductDto.Price,
                    MinimumQuantity = addNewProductDto.MinimumQuantity,
                    Category = addNewProductDto.Category,
                    DiscountRate = addNewProductDto.DiscountRate,
                };
                // Todo add image func
                var response = await _productService.AddNewProduct(newProduct);
                _logger.LogInformation($"New product added successfully");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid request");
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        // Update product
        [HttpPut]
        public async Task<IActionResult> UpdateProductById(UpdateProductDto updateProductDto)
        {
            try
            {
                var updatedProduct = new Product
                {
                    Name = updateProductDto.Name,
                    Price = updateProductDto.Price,
                    MinimumQuantity = updateProductDto.MinimumQuantity,
                    Category = updateProductDto.Category,
                    DiscountRate = updateProductDto.DiscountRate,
                };
                var response = await _productService.UpdateProductAsync(updatedProduct);
                if (response is null)
                {
                    _logger.LogInformation($"Product not found for update");
                    return NotFound("EntityDoesntExist");
                }

                _logger.LogInformation($"Product updated successfully");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid request");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized request");
                return Unauthorized($"Unauthorized request: {ex.Message}");
            }
        }
        // Delete product
        [HttpDelete]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            try
            {
                // send product and seller ids to product service to delete the product
                var response = await _productService.DeleteProductAsync(productId);
                if (response is null)
                {
                    _logger.LogInformation($"Product not found for removal");
                    return NotFound("EntityDoesntExist");
                }

                _logger.LogInformation($"Product removed successfully");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized request");
                return Unauthorized($"Unauthorized request: {ex.Message}");
            }
        }
    }
}
