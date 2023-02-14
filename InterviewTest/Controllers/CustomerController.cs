using InterviewTest.API.Models;
using InterviewTest.API.Models.Customer;
using InterviewTest.DataLayer;
using InterviewTest.DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InterviewTest.API.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly InterviewDbContext _interviewDbContext;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(InterviewDbContext interviewDbContext, ILogger<CustomerController> logger)
        {
            _interviewDbContext = interviewDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Create customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<bool>>> CreateCustomerAsync([FromBody] CreateCustomerRequest request)
        {
            _logger.LogInformation($"{DateTime.UtcNow} CreateCustomerAsync input data: {JsonConvert.SerializeObject(request)}");
            var response = new ApiResponse<bool>();
            if (request != null)
            {
                try
                {
                    await _interviewDbContext.Customers.AddAsync(new Customer
                    {
                        Firstname = request.Firstname,
                        Surname = request.Surname
                    });

                    await _interviewDbContext.SaveChangesAsync(CancellationToken.None);

                    _logger.LogInformation($"{DateTime.UtcNow} New customer created");

                    response.Result = true;

                    return response;
                }
                catch (Exception e)
                {
                    // I could implement validation service instead of using try catch but it will take more time
                    _logger.LogError(e, "");

                    response.ErrorMessage = e.Message;

                    return response;
                }
              
            }

            return response;
        }


        /// <summary>
        /// Get customers by filtration
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("index")]
        public async Task<ActionResult<ApiResponse<GetCustomersResponse>>> GetCustomersAsync(
            [FromBody] GetCustomersRequest request)
        {
            // due to security recommendation and query limitations I prefer to use POST instead of GET because here I can get data from body instead on query
            var response = new ApiResponse<GetCustomersResponse>();
            try
            {
                _logger.LogInformation($"{DateTime.UtcNow} GetCustomersAsync request data: {JsonConvert.SerializeObject(request)}");

                var query = _interviewDbContext.Customers
                    .AsNoTracking()
                    .OrderBy(e => e.Surname)
                    .Where(e => string.IsNullOrEmpty(request.SearchValue) ? true :
                                 e.Firstname.Contains(request.SearchValue) ||
                                e.Surname.Contains(request.SearchValue));

                response.Result = new GetCustomersResponse
                {
                    TotalCount = await query.CountAsync(CancellationToken.None)
                };

                response.Result.Customers = await query
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .Select(e => new GetCustomerItem
                    {
                        Surname = e.Surname,
                        Firstname = e.Firstname,
                        Id = e.Id
                    }).ToListAsync(CancellationToken.None);

                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessage = e.Message;

                _logger.LogError(e, "");
            }

            return response;
        }

        /// <summary>
        /// Delete customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCustomerAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                _logger.LogInformation($"{DateTime.UtcNow} DeleteCustomerAsync request data: {id}");

                var existingCustomer =
                    await _interviewDbContext.Customers.FirstOrDefaultAsync(e => e.Id == id, CancellationToken.None);
                if (existingCustomer != null)
                {
                    _interviewDbContext.Remove(existingCustomer);

                    await _interviewDbContext.SaveChangesAsync(CancellationToken.None);

                    return new ApiResponse<bool>()
                    {
                        Result = true
                    };
                }
            }
            catch (Exception e)
            {
                response.ErrorMessage = e.Message;

                _logger.LogError(e, $"request data: {id}");
            }

            return response;
        }
    }
}
