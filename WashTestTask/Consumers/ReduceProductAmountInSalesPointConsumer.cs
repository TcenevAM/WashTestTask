using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Consumers
{
    public class ReduceProductAmountInSalesPointConsumer : IConsumer<ReduceProductAmountInSalesPoint>
    {
        private readonly ISalesPointService _salesPointService;
        private readonly ILogger<ReduceProductAmountInSalesPoint> _logger;

        public ReduceProductAmountInSalesPointConsumer(ISalesPointService salesPointService,
            ILogger<ReduceProductAmountInSalesPoint> logger)
        {
            _salesPointService = salesPointService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReduceProductAmountInSalesPoint> context)
        {
            _logger.LogInformation($"Reducing product amount in sales point");
            
            var salesPoint = await _salesPointService.GetAsync(context.Message.SaleDto.SalesPointId);
            foreach (var saleDataDto in context.Message.SaleDto.SalesData)
            {
                var productInfo =
                    salesPoint.ProvidedProducts.First(p => p.ProductId == saleDataDto.ProductId);

                if (productInfo.Quantity < saleDataDto.ProductQuantity)
                    throw new ArgumentException(
                        $"Sales point with id {salesPoint.Id} does not contain enough product with id {productInfo.ProductId}");
                
                productInfo.Quantity -= saleDataDto.ProductQuantity;
            }

            await _salesPointService.UpdateAsync(salesPoint);
        }
    }
}