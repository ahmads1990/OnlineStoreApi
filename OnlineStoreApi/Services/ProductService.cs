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
                string.IsNullOrEmpty(product.ProductImagePath) ||
                product.MinimumQuantity <= 0)
                return null;

            // Try to store product in database 
            var createdProduct = await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Return created product
            return createdProduct.Entity;
        }
        public Task<Product?> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
        public async Task<Product?> DeleteProductAsync(int productId)
        {
            // Check for valid product id (positive number) and seller id (non null/empty string)
            if (productId <= 0)
                return null;

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
