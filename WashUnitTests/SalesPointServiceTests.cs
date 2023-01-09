using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WashTestTask.Dtos;
using WashTestTask.Models;
using WashTestTask.Services;
using Xunit;

namespace WashUnitTests
{
    public class SalesPointServiceTests
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;

        public SalesPointServiceTests()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new Context(_options);
            _context.AddRange(
                new SalesPoint
                {
                    Id = 1,
                    Name = "Sale point 1",
                    ProvidedProducts = new List<ProvidedProduct>
                    {
                        new ProvidedProduct
                        {
                            Id = 1, Product = new Product { Id = 1, Price = 1000, Title = "product 1" }, Quantity = 15
                        },
                        new ProvidedProduct
                        {
                            Id = 2, Product = new Product { Id = 2, Price = 2000, Title = "product 2" }, Quantity = 5
                        }
                    }
                },
                new SalesPoint
                {
                    Id = 2,
                    Name = "Sale point 2",
                    ProvidedProducts = new List<ProvidedProduct>
                    {
                        new ProvidedProduct
                        {
                            Id = 3, Product = new Product { Id = 3, Price = 3000, Title = "product 3" }, Quantity = 0
                        },
                        new ProvidedProduct
                        {
                            Id = 4, Product = new Product { Id = 4, Price = 4000, Title = "product 4" }, Quantity = 50
                        }
                    }
                }
            );
            _context.SaveChanges();
        }
        
        [Fact]
        public async Task GetAsync_ExistingSalePoint_ReturnsCorrectSalePoint()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SalesPointService(context);

                // Act
                var result = await service.GetAsync(1);

                // Assert
                Assert.Equal(1, result.Id);
                Assert.Equal("Sale point 1", result.Name);
            }
        }
        
        [Fact]
        public async Task GetAsync_NonExistingSalePoint_ReturnsNull()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                const int nonExistentId = -1;
                var service = new SalesPointService(context);

                // Act
                var result = await service.GetAsync(nonExistentId);

                // Assert
                Assert.Null(result);
            }
        }
        
        [Fact]
        public void GetAll_SalePointInDatabase_ReturnsAllSalePoint()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SalesPointService(context);

                // Act
                var result = service.GetAll();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }
        
        [Fact]
        public async void AddAsync_ValidSalePoint_ReturnsSalePoint()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SalesPointService(context);
                var salePoint = new SalesPoint()
                {
                    Id = 3,
                    Name = "Sales point 3",
                    ProvidedProducts = new List<ProvidedProduct>()
                };

                // Act
                var result = await service.AddAsync(salePoint);

                // Assert
                Assert.Equal(salePoint, result);
            }
        }

        [Fact]
        public async Task PutAsync_ValidSalesPoint_UpdatesSalesPoint()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SalesPointService(context);
                var salesPoint = await context.SalesPoints.FindAsync(1);
                var dto = new SalesPointDTO { Name = "New sales point name"};

                // Act
                await service.PutAsync(salesPoint, dto);

                // Assert
                Assert.Equal("New sales point name", salesPoint.Name);
            }
        }

        [Fact]
        public async void RemoveAsync_ValidSalesPoint_RemovesSalesPointFromDbContext()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SalesPointService(context);
                var sale = await context.SalesPoints.FindAsync(1);

                // Act
                await service.RemoveAsync(sale);

                // Assert
                Assert.DoesNotContain(_context.Sales, s => s.Id == 1);
            }
        }
    }
}