using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Data.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SaleCreationService.Consumers
{
    public class CreateSaleConsumer : IConsumer<SaleDTO>
    {
        private readonly ILogger<CreateSaleConsumer> _logger;
        private readonly IRequestClient<GetSalesPoint> _salesPointClient;
        private readonly IRequestClient<GetCustomer> _customerClient;

        public CreateSaleConsumer(ILogger<CreateSaleConsumer> logger, IRequestClient<GetSalesPoint> salesPointClient,
            IRequestClient<GetCustomer> customerClient)
        {
            _logger = logger;
            _salesPointClient = salesPointClient;
            _customerClient = customerClient;
        }

        public async Task Consume(ConsumeContext<SaleDTO> context)
        {
            _logger.LogInformation("Creating new sale.");

            try
            {
                var salePointResponse =
                    await _salesPointClient.GetResponse<GetSalesPointResponse>(new
                        { SalesPointId = context.Message.SalesPointId });
                var customer =
                    await _customerClient.GetResponse<GetCustomerResponse>(new
                        { SalesPointId = context.Message.CustomerId });

                if (customer.Message.Id == 1)
                    Console.WriteLine("exception"); //return NotFound($"Customer with id {saleDto.CustomerId} not found");
                if (salePointResponse.Message.Name.Contains("q"))
                    Console.WriteLine("Exception");

                await context.Publish<ReduceProductAmountInSalesPoint>(context.Message);
                await context.Publish<CreateSale>(context.Message);

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            /*catch (ArgumentException e)
            {
                _logger.LogError(e, "Error while creating sale.");
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Error while creating sale.");
                var client = new HttpClient();
                await client.PutAsync($"/SalesPoints/{saleDto.SalesPointId}",
                    salePointResponse.Content);
                return BadRequest(e.Message);
            }8*/
        }
    }
}