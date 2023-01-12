using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WashTestTask.Controllers;
using WashTestTask.Database;
using WashTestTask.Services;
using WashTestTask.Services.Interfaces;
using Xunit;

namespace WashUnitTests
{
    public class CustomerControllerTests
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly ILogger<CustomersController> _mockLogger = new Mock<ILogger<CustomersController>>().Object;

        public CustomerControllerTests()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new Context(_options);
            _context.Database.EnsureDeleted();
            _context.AddRange(
                new Customer { Id = 1, Name = "Customer 1" },
                new Customer { Id = 2, Name = "Customer 2" }
            );
            _context.SaveChanges();
        }
        
        [Fact]
        public void GetCustomers_CustomersInDatabase_ReturnsCustomers()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var response = controller.GetCustomers();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(response.Result);
                var returnValue = Assert.IsAssignableFrom<IEnumerable<CustomerDTO>>(okResult.Value);
                Assert.Equal(context.Customers.Count(), returnValue.Count());
            }
        }

        [Fact]
        public async Task GetCustomer_NotFound_ReturnsNotFound()
        {
            // Arrange
            const int nonExistentId = -1;
            var customerService = new Mock<ICustomerService>();
            customerService.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync((Customer)null);
            var controller = new CustomersController(_mockLogger, customerService.Object);

            // Act
            var response = await controller.GetCustomerAsync(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }
        
        [Fact]
        public async Task GetCustomer_ValidId_ReturnsCustomer()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var response = await controller.GetCustomerAsync(1);

                // Assert
                Assert.IsType<OkObjectResult>(response.Result);
            }
        }
        
        [Fact]
        public async Task PostCustomer_ValidCustomer_ReturnsCreatedResponse()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var customer = new CustomerDTO()
                {
                    Name = "John"
                };
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var result = await controller.CreateCustomerAsync(customer);

                // Assert
                Assert.IsType<CreatedAtActionResult>(result);
            }
        }

        [Fact]
        public async Task PostCustomer_InvalidCustomer_ReturnsBadRequest()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var customer = new CustomerDTO();
                var controller = new CustomersController(_mockLogger, new CustomerService(context));
                controller.ModelState.AddModelError("Name", "Required");

                // Act
                var response = await controller.CreateCustomerAsync(customer);

                // Assert
                Assert.IsType<BadRequestObjectResult>(response);
            }
        }
        
        [Fact]
        public async Task PutCustomer_ExistingCustomer_UpdatesCustomer()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var customerDto = new CustomerDTO { Name = "New Name" };
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var response = await controller.PutCustomerAsync(1, customerDto);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(response.Result);
                var customer = (CustomerDTO)okResult.Value;
                Assert.Equal("New Name", customer.Name);
            }
        }
        
        [Fact]
        public async Task PutCustomer_NotFound_ReturnsNotFound()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                const int nonExistentId = -1;
                var customerDto = new CustomerDTO { Name = "New Customer" };
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var response = await controller.PutCustomerAsync(nonExistentId, customerDto);

                // Assert
                Assert.IsType<NotFoundResult>(response.Result);
            }
        }
        
        [Fact]
        public async Task DeleteCustomer_ExistingIdPassed_DeletesCustomerAndReturnsNoContent()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var result = await controller.DeleteCustomerAsync(1);

                // Assert
                Assert.IsType<NoContentResult>(result);
                Assert.DoesNotContain(_context.Customers, c => c.Id == 1);
            }
        }
        
        [Fact]
        public async Task DeleteCustomer_NonExistingIdPassed_ReturnsNotFound()
        {
            using (var context = new Context(_options))
            {
                // Arrange
                const int nonExistentId = -1;
                var controller = new CustomersController(_mockLogger, new CustomerService(context));

                // Act
                var response = await controller.DeleteCustomerAsync(nonExistentId);

                // Assert
                Assert.IsType<NotFoundResult>(response);
            }
        }
    }
}