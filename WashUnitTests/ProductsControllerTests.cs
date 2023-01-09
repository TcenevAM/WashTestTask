using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WashTestTask.Controllers;
using WashTestTask.Dtos;
using WashTestTask.Models;
using WashTestTask.Services;
using Xunit;

namespace WashUnitTests
{
    public class ProductControllerTests
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly ILogger<ProductsController> _mockLogger = new Mock<ILogger<ProductsController>>().Object;

        public ProductControllerTests()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new Context(_options);
            _context.AddRange(
                new Product { Id = 1, Title = "Product 1", Price = 10.99m },
                new Product { Id = 2, Title = "Product 2", Price = 15.99m },
                new Product { Id = 3, Title = "Product 3", Price = 20.99m }
            );
            _context.SaveChanges();
        }

        [Fact]
        public void GetAll_ValidRequest_ReturnsAllProducts()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var controller = new ProductsController(new ProductService(context), _mockLogger);
    
                // Act
                var result = controller.GetProducts().Result as OkObjectResult;
    
                // Assert
                var products = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(result.Value);
                Assert.Equal(3, products.Count());
            }
        }
        
        [Fact]
        public async void GetById_InvalidId_ReturnsNotFound()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var productId = 4;

                // Act
                var controller = new ProductsController(new ProductService(context), _mockLogger);
                var result = await controller.GetProductByIdAsync(productId);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetById_ValidId_ReturnsCorrectProduct(int id)
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var controller = new ProductsController(new ProductService(context), _mockLogger);

                // Act
                var response = await controller.GetProductByIdAsync(id);
                var result = (OkObjectResult)response.Result;
                var product = (ProductDTO)result.Value;
            
                // Assert
                Assert.NotNull(product);
                Assert.Equal(id, product.Id);
            }
        }
        
        [Fact]
        public async Task Post_ValidProduct_ReturnsProduct()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var controller = new ProductsController(new ProductService(context), _mockLogger);

                // Act
                var result = await controller.CreateProductAsync(new ProductDTO { Name = "Product2", Price = 20 });

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                var product = Assert.IsType<ProductDTO>(createdResult.Value);
                Assert.Equal("Product2", product.Name);
                Assert.Equal(20, product.Price);
                Assert.Equal(4, context.Products.Count());
            }
        }
        
        [Fact]
        public async void PutProduct_ValidProduct_UpdatesProduct()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var productDto = new ProductDTO { Name = "UpdatedProduct", Price = 100 };
                const int productId = 1;

                // Act
                var controller = new ProductsController(new ProductService(context), _mockLogger);
                var response = await controller.PutProduct(productId, productDto);

                // Assert
                var product = await context.Products.FindAsync(productId);
                Assert.IsType<OkObjectResult>(response.Result);
                Assert.Equal("UpdatedProduct", product.Title);
                Assert.Equal(100, product.Price);
            }
        }
        
        [Fact]
        public async void PutProduct_InvalidId_ReturnsNotFound()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var productDto = new ProductDTO();
                const int productId = -1;

                // Act
                var controller = new ProductsController(new ProductService(context), _mockLogger);
                var response = await controller.PutProduct(productId, productDto);

                // Assert
                Assert.IsType<NotFoundResult>(response.Result);
            }
        }
        
        [Fact]
        public async void DeleteProduct_ValidId_RemovesProduct()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var productId = 1;

                // Act
                var controller = new ProductsController(new ProductService(context), _mockLogger);
                await controller.DeleteProduct(productId);

                // Assert
                Assert.Equal(2, context.Products.Count());
            }
        }

        [Fact]
        public async void DeleteProduct_InvalidId_ReturnsNotFound()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var productId = 4;

                // Act
                var controller = new ProductsController(new ProductService(context), _mockLogger);
                var response = await controller.DeleteProduct(productId);

                // Assert
                Assert.IsType<NotFoundResult>(response);
            }
        }
    }
}