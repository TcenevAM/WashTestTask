using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Consumers
{
    public class GetCustomerConsumer : IConsumer<GetCustomer>
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<GetCustomerConsumer> _logger;

        public GetCustomerConsumer(ICustomerService customerService, ILogger<GetCustomerConsumer> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetCustomer> context)
        {
            _logger.LogInformation($"Consuming get customer request with customer id {context.Message.CustomerId}");
            var customer = await _customerService.GetAsync(context.Message.CustomerId);
            
            
            
            await context.RespondAsync<GetCustomerResponse>(new
            {
                Id = customer.Id,
                Name = customer.Name,
                SalesIds = customer.Sales.Select(s => s.Id).ToList()
            });
        }
    }
}