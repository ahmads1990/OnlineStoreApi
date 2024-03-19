using OnlineStoreApi.Models;

namespace OnlineStoreApi.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product> AddNewProduct(Product product);
        Task<Product?> UpdateProductAsync(Product product);
        Task<Product?> DeleteProductAsync(int productId);
    }
}
