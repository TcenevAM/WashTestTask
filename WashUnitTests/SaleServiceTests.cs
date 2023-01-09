using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SaleServiceTests
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;

        public SaleServiceTests()
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
            _context.AddRange(
                new Sale
                {
                    Id = 1, 
                    Customer = new Customer
                    {
                        Id = 1,
                        Name = "Customer 1"
                    },
                    Date = DateTimeOffset.Now,
                    SalesPoint = _context.SalesPoints.Find(1),
                    SalesData = new List<SaleData>
                    {
                        new SaleData
                        {
                            Id = 1,
                            Product = _context.Products.Find(1),
                            ProductQuantity = 12
                        }
                    }
                }
            );
            _context.Customers.Add(new Customer { Id = 2, Name = "Customer 2" });
            _context.SaveChanges();
        }
        
        [Fact]
        public async Task GetAsync_ExistingSale_ReturnsCorrectSale()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);

                // Act
                var result = await service.GetAsync(1);

                // Assert
                Assert.Equal(1, result.Id);
                Assert.Equal(1, result.CustomerId);
            }
        }
        
        [Fact]
        public async Task GetAsync_NonExistingSale_ReturnsNull()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                const int nonExistentId = -1;
                var service = new CustomerService(context);

                // Act
                var result = await service.GetAsync(nonExistentId);

                // Assert
                Assert.Null(result);
            }
        }
        
        [Fact]
        public void GetAll_SalesInDatabase_ReturnsAllSales()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);

                // Act
                var result = service.GetAll();

                // Assert
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task ReduceProductAmountInSalesPoint_ValidSaleData_ReduceProductAmount()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var salesPoint = await _context.SalesPoints.FindAsync(1);
                var sale = new Sale
                {
                    SalesPoint = salesPoint,
                    SalesData = new List<SaleData> { new SaleData { ProductId = 1, ProductQuantity = 5 } }
                };
                var service = new SaleService(context);

                // Act
                await service.ReduceProductAmountInSalesPoint(sale);

                // Assert
                Assert.Equal(10, salesPoint.ProvidedProducts[0].Quantity);
            }
        }
        
        [Fact]
        public async Task ReduceProductAmountInSalesPoint_NotEnoughProduct_ThrowsArgumentException()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var salesPoint = await _context.SalesPoints.FindAsync(1);
                var sale = new Sale
                {
                    SalesPoint = salesPoint,
                    SalesData = new List<SaleData> { new SaleData { ProductId = 1, ProductQuantity = int.MaxValue } }
                };
                var service = new SaleService(context);

                // Act
                // Assert
                await Assert.ThrowsAsync<ArgumentException>(() => service.ReduceProductAmountInSalesPoint(sale));
            }
        }

        [Fact]
        public async void AddAsync_ValidSale_ReturnsSale()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);
                var sale = new Sale
                {
                    Date = DateTimeOffset.Now,
                    SalesPointId = 1,
                    CustomerId = 1,
                    SalesData = new List<SaleData>
                    {
                        new SaleData { ProductId = 1, ProductQuantity = 2 },
                        new SaleData { ProductId = 2, ProductQuantity = 3 }
                    }
                };

                // Act
                var result = await service.AddAsync(sale);

                // Assert
                Assert.Equal(sale, result);
            }
        }
        
        [Fact]
        public async void AddAsync_SalesPointDoesNotExist_ThrowsArgumentException()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);
                var sale = new Sale
                {
                    Date = DateTimeOffset.Now,
                    SalesPointId = -1,
                    CustomerId = 1,
                    SalesData = new List<SaleData>
                    {
                        new SaleData { ProductId = 1, ProductQuantity = 2 },
                        new SaleData { ProductId = 2, ProductQuantity = 3 }
                    }
                };
                
                // Act
                // Assert
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(sale));
            }
        }
        
        [Fact]
        public async void AddAsync_SalesPointDoesNotContainAllRequestedProducts_ThrowsArgumentException()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);
                var sale = new Sale
                {
                    Date = DateTimeOffset.Now,
                    SalesPointId = 1,
                    CustomerId = 1,
                    SalesData = new List<SaleData>
                    {
                        new SaleData { ProductId = 10, ProductQuantity = 2 },
                        new SaleData { ProductId = 2, ProductQuantity = 3 }
                    }
                };
                
                // Act
                // Assert
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(sale));
            }
        }
        
        [Fact]
        public async void AddAsync_InvalidCustomerId_ThrowsArgumentException()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);
                var sale = new Sale
                {
                    Date = DateTimeOffset.Now,
                    SalesPointId = 1,
                    CustomerId = -1,
                    SalesData = new List<SaleData>
                    {
                        new SaleData { ProductId = 1, ProductQuantity = 2 },
                        new SaleData { ProductId = 2, ProductQuantity = 3 }
                    }
                };
                
                // Act
                // Assert
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(sale));
            }
        }
        
        [Fact]
        public async Task PutAsync_ValidSale_UpdatesSale()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);
                var sale = await context.Sales.FindAsync(1);
                var dto = new SaleDTO { CustomerId = 2 };

                // Act
                await service.PutAsync(sale, dto);

                // Assert
                Assert.Equal("Customer 2", sale.Customer.Name);
            }
        }
        
        [Fact]
        public async void RemoveAsync_ValidSale_RemovesSaleFromDbContext()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new SaleService(context);
                var sale = await context.Sales.FindAsync(1);

                // Act
                await service.RemoveAsync(sale);

                // Assert
                Assert.DoesNotContain(_context.Sales, s => s.Id == 1);
            }
        }
    }
}