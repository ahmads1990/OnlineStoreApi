using Microsoft.Data.Sqlite;
using OnlineStoreApi.Models;
using OnlineStoreApi.Services.Interfaces;
using OnlineStoreApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineStoreApi.Tests
{
    public class ProductServiceTests
    {
        AppDbContext _appDbContext;
        IProductService _productService;
        SqliteConnection _connection;
        static int seedDataCount = 5;
        static int nonExistingId = 1000;
        public List<Product> GetProductsSeedData()
        {
            return new List<Product>()
            {
                new Product { ProductId = 1, Name = "Product1", MinimumQuantity = 1, Price = 10, Category = "C1", DiscountRate = 15, ProductImagePath = "" },
                new Product { ProductId = 2, Name = "Product2", MinimumQuantity = 2, Price = 15, Category = "C2", DiscountRate = 25, ProductImagePath = "" },
                new Product { ProductId = 3, Name = "Product3", MinimumQuantity = 3, Price = 20, Category = "C3", DiscountRate = 50, ProductImagePath = "" },
                new Product { ProductId = 4, Name = "Product4", MinimumQuantity = 3, Price = 25, Category = "C4", DiscountRate = 75, ProductImagePath = "" },
                new Product { ProductId = 5, Name = "Product5", MinimumQuantity = 4, Price = 30, Category = "C5", DiscountRate = 0, ProductImagePath = "" }
            };
        }
        [SetUp]
        public void Setup()
        {
            // create and open new SQLite Connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            // configure options 
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite(_connection)
               .Options;
            // seeding new context 
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Products.AddRange(GetProductsSeedData());
                context.SaveChanges();
            }
            // testing context
            _appDbContext = new AppDbContext(options);
            // Arrange
            _productService = new ProductService(_appDbContext);
        }
        [TearDown]
        public void TearDown()
        {
            _appDbContext.Dispose();
            _connection.Close();
        }
        // naming = (MethodTesting)_Input State_ Expected Output
        // GetAllProductsAsync
        [Test]
        public async Task GetAllProductsAsync_ValidData_NotNull()
        {
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetAllProductsAsync_ValidData_CountEqualSeed()
        {
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(seedDataCount));
        }// GetProductByIdAsync
        [Test]
        public async Task GetProductByIdAsync_ValidExistingId_NotNull()
        {
            // Assert 
            int testId = 1;
            // Act
            var result = await _productService.GetProductByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetProductByIdAsync_ValidNonExistingId_Null()
        {
            // Assert 
            int testId = nonExistingId;
            // Act
            var result = await _productService.GetProductByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Null);
        }
        //[Test]
        //public void GetProductByIdAsync_InvalidId_Throw()
        //{
        //    // Assert 
        //    int testId = -1;
        //    // Act
        //    var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        //       await _productService.GetProductByIdAsync(testId));
        //    // Assert
        //    Assert.That(exception, Is.TypeOf<ArgumentException>());
        //}
        // AddNewProduct
        [Test]
        public async Task AddNewProduct_ValidProduct_ValidProduct()
        {
            // Arrange
            var newProduct = new Product { Name = "Product6", MinimumQuantity = 3, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "notEmpty" };
            // Act
            var result = await _productService.AddNewProduct(newProduct);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(newProduct.Name));
            Assert.That(result.ProductId, Is.EqualTo(6));
        }
        [Test]
        public void AddNewProduct_InValidId_Throw()
        {
            // Arrange
            var newProduct = new Product { ProductId = 6, Name = "Product6", MinimumQuantity = 3, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        [Test]
        public void AddNewProduct_InValidName_Throw()
        {
            // Arrange
            var newProduct = new Product { Name = "", MinimumQuantity = 3, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        [Test]
        public void AddNewProduct_InValidMinimumQuantity_Throw()
        {
            // Arrange
            var newProduct = new Product { Name = "Product6", MinimumQuantity = 0, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        [Test]
        public void AddNewProduct_InValidPrice_Throw()
        {
            // Arrange
            var newProduct = new Product { Name = "Product6", MinimumQuantity = 3, Price = 0, Category = "C6", DiscountRate = 15, ProductImagePath = "" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        [Test]
        public void AddNewProduct_InValidPriceNegative_Throw()
        {
            // Arrange
            var newProduct = new Product { Name = "Product6", MinimumQuantity = 3, Price = -10, Category = "C6", DiscountRate = 15, ProductImagePath = "" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        // UpdateProductAsync
        [Test]
        public async Task UpdateProductAsync_ValidProduct_ValidProduct()
        {
            // Arrange
            var updatedProduct = new Product { ProductId = 1, Name = "Product6", MinimumQuantity = 3, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "notEmpty" };
            // Act
            var result = await _productService.UpdateProductAsync(updatedProduct);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(updatedProduct.Name));
            Assert.That(result.ProductId, Is.EqualTo(updatedProduct.ProductId));
        }
        [Test]
        public void UpdateProductAsync_InvalidId_Throw()
        {
            // Arrange
            var updatedProduct = new Product { ProductId = -1, Name = "Product6", MinimumQuantity = 3, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "notEmpty" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.UpdateProductAsync(updatedProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        [Test]
        public void UpdateProductAsync_InvalidName_Throw()
        {
            // Arrange
            var updatedProduct = new Product { ProductId = 1, Name = "", MinimumQuantity = 3, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "notEmpty" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        [Test]
        public void UpdateProductAsync_InvalidMinimumQuantity_Throw()
        {
            // Arrange
            var updatedProduct = new Product { ProductId = 1, Name = "Product6", MinimumQuantity = 0, Price = 130, Category = "C6", DiscountRate = 15, ProductImagePath = "notEmpty" };
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProduct));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
        //[Test]
        //public void UpdateProductAsync_InvalidPrice_Throw()
        //{
        //    // Arrange
        //    var updatedProduct = new Product { ProductId = 1, Name = "Product6", MinimumQuantity = 3, Price = 0, Category = "C6", DiscountRate = 15, ProductImagePath = "notEmpty" };
        //    // Act
        //    var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        //    await _productService.UpdateProductAsync(updatedProduct));

        //    Assert.That(exception, Is.TypeOf<ArgumentException>());
        //}
        // DeleteProductAsync
        [Test]
        public async Task DeleteProductAsync_ValidId_NotNull()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = product.ProductId;
            // Act
            var result = await _productService.DeleteProductAsync(testId);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductId, Is.EqualTo(testId));
        }
        [Test]
        public async Task DeleteProductAsync_ValidNonExistingId_Null()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = 6;
            // Act
            var result = await _productService.DeleteProductAsync(testId);
            // Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public void DeleteProductAsync_InvalidId_Throw()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = 0;
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.DeleteProductAsync(testId));

            Assert.That(exception, Is.TypeOf<ArgumentException>());
        }
    }
}
