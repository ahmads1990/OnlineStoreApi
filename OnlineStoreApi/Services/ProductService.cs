using Microsoft.EntityFrameworkCore;
using OnlineStoreApi.Models;
using OnlineStoreApi.Services.Interfaces;

namespace OnlineStoreApi.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        }
        public async Task<Product> AddNewProduct(Product product)
        {
            // Validation
            // Check if entry is null or product has null values
            if (product is null ||
                string.IsNullOrEmpty(product.Name) ||
                string.IsNullOrEmpty(product.ProductImagePath))
                throw new ArgumentException("Invalid/Empty entity data");
            if (product.MinimumQuantity <= 0)
                throw new ArgumentException("Invalid entity data, Minimum Quantity >= 1");

            // Try to store product in database 
            var createdProduct = await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Return created product
            return createdProduct.Entity;
        }
        public async Task<Product?> UpdateProductAsync(Product product)
        {
            // Check if entryDto is null or seller id is null or product has null values then throw exception 
            if (product is null || product.ProductId <= 0 || string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("Invalid/Empty entity data");
            if (product.MinimumQuantity <= 0)
                throw new ArgumentException("Invalid entity data, Minimum Quantity >= 1");

            // Get product to be Updated
            var productToBeUpdated = await GetProductByIdAsync(product.ProductId);
            // if couldn't find return null
            if (productToBeUpdated is null) return null;

            // map new values to the product entity
            productToBeUpdated.Name = product.Name;
            productToBeUpdated.MinimumQuantity = product.MinimumQuantity;
            productToBeUpdated.Price = productToBeUpdated.Price;
            if (!string.IsNullOrEmpty(product.ProductImagePath))
                productToBeUpdated.ProductImagePath = product.ProductImagePath;

            // update product
            var updateResult = _dbContext.Products.Update(productToBeUpdated);
            await _dbContext.SaveChangesAsync();

            // Return updated product
            return updateResult.Entity;
        }
        public async Task<Product?> DeleteProductAsync(int productId)
        {
            // Check for valid product id (positive number) and seller id (non null/empty string)
            if (productId <= 0)
                throw new ArgumentException("Invalid/Empty entity id");

            // Chech the product exists in database
            var productToBeDeleted = await GetProductByIdAsync(productId);
            if (productToBeDeleted is null) return null;

            // Delete product the save changes
            _dbContext.Products.Remove(productToBeDeleted);
            await _dbContext.SaveChangesAsync();

            // Return product if necessary
            return productToBeDeleted;
        }
    }
}
