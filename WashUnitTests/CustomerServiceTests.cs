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
    public class CustomerServiceTests
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;

        public CustomerServiceTests()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new Context(_options);
            _context.AddRange(
                new Customer { Id = 1, Name = "Customer 1" },
                new Customer { Id = 2, Name = "Customer 2" }
            );
            _context.SaveChanges();
        }
        
        [Fact]
        public async Task GetAsync_ExistingCustomer_ReturnsCorrectCustomer()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new CustomerService(context);

                // Act
                var result = await service.GetAsync(1);

                // Assert
                Assert.Equal("Customer 1", result.Name);
                Assert.Equal(1, result.Id);
            }
        }

        [Fact]
        public async Task GetAsync_NonExistingCustomer_ReturnsNull()
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
        public void GetAll_CustomersInDatabase_ReturnsAllCustomers()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new CustomerService(context);

                // Act
                var result = service.GetAll();

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(result, c => c.Id == 1 && c.Name == "Customer 1");
                Assert.Contains(result, c => c.Id == 2 && c.Name == "Customer 2");
            }
        }
        
        [Fact]
        public void ToDto_CorrectCustomerEntity_CustomerDto()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var service = new CustomerService(context);
                var customer = new Customer { Id = 1, Name = "John Smith", Sales = new List<Sale> { new Sale {Id = 1} } };
                var expectedDto = new CustomerDTO { Id = 1, Name = "John Smith", SalesIds = new List<int> { 1 } };

                // Act
                var actualDto = service.ToDto(customer);

                // Assert
                Assert.Equal(expectedDto.Id, actualDto.Id);
                Assert.Equal(expectedDto.Name, actualDto.Name);
                Assert.Equal(expectedDto.SalesIds, actualDto.SalesIds);
            }
        }
        
        [Fact]
        public async Task AddAsync_Customer_SameCustomer()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var customer = new Customer
                {
                    Name = "John"
                };
                var service = new CustomerService(context);

                // Act
                var result = await service.AddAsync(customer);

                // Assert
                Assert.Equal(3, result.Id);
                Assert.Equal("John", result.Name);
            }
        }
        
        [Fact]
        public async Task PutAsync_ValidCustomer_UpdatesName()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var customer = await context.Customers.FindAsync(1);
                var customerDto = new CustomerDTO { Name = "Jane" };
                var customerService = new CustomerService(context);

                // Act
                await customerService.PutAsync(customer, customerDto);

                // Assert
                Assert.Equal("Jane", customer.Name);
            }
        }
        
        [Fact]
        public async void RemoveAsync_ValidCustomer_RemovesCustomerFromDbContext()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var customer = await context.Customers.FindAsync(1);
                var customerService = new CustomerService(context);

                // Act
                await customerService.RemoveAsync(customer);

                // Assert
                Assert.DoesNotContain(_context.Customers, c => c.Id == customer.Id);
            }
        }
    }
}