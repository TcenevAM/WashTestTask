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
    public class SaleCreationHandler : IConsumer<HandleSaleCreationRequest>
    {
        private readonly ILogger<SaleCreationHandler> _logger;
        private readonly IRequestClient<CreateSale> _saleClient;
        private readonly IRequestClient<ReduceProductAmountInSalesPoint> _reduceClient;

        public SaleCreationHandler(ILogger<SaleCreationHandler> logger, IRequestClient<CreateSale> saleClient, 
            IRequestClient<ReduceProductAmountInSalesPoint> reduceClient)
        {
            _logger = logger;
            _saleClient = saleClient;
            _reduceClient = reduceClient;
        }

        public async Task Consume(ConsumeContext<HandleSaleCreationRequest> context)
        {
            _logger.LogInformation("Creating new sale in consumer.");

            var dto = context.Message.SaleDto;

            _logger.LogInformation($"Publishing event to reduce product amount in sales point with id {dto.SalesPointId}");
            await _reduceClient.GetResponse<ReduceProductAmountInSalesPoint>(new { SaleDto = dto });
            
            _logger.LogInformation($"Creating sale with {dto}");
            var response = await _saleClient.GetResponse<CreateSaleResponse>(new { SaleDto = dto });
            await context.RespondAsync<CreateSaleResponse>(new { Result = response.Message.Result });
        }
    }
}