using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using WashTestTask.Services;

namespace WashTestTask.Consumers
{
    public class GetCustomerConsumer : IConsumer<GetCustomer>
    {
        private readonly CustomerService _customerService;

        public GetCustomerConsumer(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task Consume(ConsumeContext<GetCustomer> context)
        {
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