using Castle.Core.Resource;
using InterviewTest.API.Controllers;
using InterviewTest.API.Models;
using InterviewTest.API.Models.Customer;
using InterviewTest.DataLayer;
using InterviewTest.DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InterviewTest.Tests.Controllers
{
    public class CustomerControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;

        public CustomerControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public async Task CreateCustomerAsync_WithValidRequest_ReturnsApiResponseWithTrueResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CustomerController>>();
            using var mockDbContext = new InterviewDbContext(_databaseFixture.Options);
            var controller = new CustomerController(mockDbContext, mockLogger.Object);

            var request = new CreateCustomerRequest
            {
                Firstname = "John",
                Surname = "Doe"
            };

            // Act
            var result = await controller.CreateCustomerAsync(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(result.Value);
            Assert.True(apiResponse.Result);
        }

        [Fact]
        public async Task CreateCustomerAsync_WithNullRequest_ReturnsApiResponseWithFalseResultAndErrorMessage()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CustomerController>>();
            using var mockDbContext = new InterviewDbContext(_databaseFixture.Options);
            var controller = new CustomerController(mockDbContext, mockLogger.Object);

            // Act
            var result = await controller.CreateCustomerAsync(null);

            // Assert
            Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(result.Value);
            Assert.False(apiResponse.Result);
        }

        [Fact]
        public async Task GetCustomersAsync_WithValidRequest_ReturnsApiResponseWithGetCustomersResponse()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CustomerController>>();
            using var mockDbContext = new InterviewDbContext(_databaseFixture.Options);

            var existingCustomers = await mockDbContext.Customers.ToListAsync();
            mockDbContext.Customers.RemoveRange(existingCustomers);
            await mockDbContext.SaveChangesAsync();

            var controller = new CustomerController(mockDbContext, mockLogger.Object);

            var request = new GetCustomersRequest
            {
                SearchValue = "John",
                Page = 0,
                Count = 10
            };

            var customers = new List<Customer>
            {
                new Customer { Firstname = "John", Surname = "Doe", Id = Guid.NewGuid() },
                new Customer { Firstname = "Jane", Surname = "Doe", Id = Guid.NewGuid() }
            };
            mockDbContext.AddRange(customers);
            await mockDbContext.SaveChangesAsync(CancellationToken.None);
            
            // Act
            var result = await controller.GetCustomersAsync(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse<GetCustomersResponse>>>(result);
            var apiResponse = Assert.IsType<ApiResponse<GetCustomersResponse>>(result.Value);
            Assert.NotEqual(customers.Count, apiResponse.Result.TotalCount);
            Assert.NotEqual(customers.Count, apiResponse.Result.Customers.Count());
        }

        [Fact]
        public async Task DeleteCustomerAsync_WithValidId_ReturnsApiResponseWithTrueResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CustomerController>>();
            using var mockDbContext = new InterviewDbContext(_databaseFixture.Options);
            var controller = new CustomerController(mockDbContext, mockLogger.Object);

            var id = Guid.NewGuid();
            var customer = new Customer { Firstname = "John", Surname = "Doe", Id = id };
            mockDbContext.AddRange(customer);
            await mockDbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await controller.DeleteCustomerAsync(id);

            // Assert
            Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
            Assert.True(result.Value.Result);
        }

        [Fact]
        public async Task DeleteCustomerAsync_WithInvalidId_ReturnsApiResponseWithFalseResult()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CustomerController>>();
            using var mockDbContext = new InterviewDbContext(_databaseFixture.Options);
            var controller = new CustomerController(mockDbContext, mockLogger.Object);

            var id = Guid.NewGuid();

            // Act
            var result = await controller.DeleteCustomerAsync(id);

            // Assert
            Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
            Assert.False(result.Value.Result);
        }
    }
}