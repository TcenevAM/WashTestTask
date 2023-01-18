using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using WashTestTask.Services;

namespace WashTestTask.Consumers
{
    public class ReduceProductAmountInSalesPointConsumer : IConsumer<ReduceProductAmountInSalesPoint>
    {
        private readonly SalesPointService _salesPointService;

        public ReduceProductAmountInSalesPointConsumer(SalesPointService salesPointService)
        {
            _salesPointService = salesPointService;
        }

        public async Task Consume(ConsumeContext<ReduceProductAmountInSalesPoint> context)
        {
            var salesPoint = await _salesPointService.GetAsync(context.Message.SaleDto.SalesPointId);
            foreach (var saleDataDto in context.Message.SaleDto.SalesData)
            {
                var productInfo =
                    salesPoint.ProvidedProducts.First(p => p.ProductId == saleDataDto.ProductId);
                
                productInfo.Quantity -= saleDataDto.ProductQuantity;
            }

            await _salesPointService.UpdateAsync(salesPoint);
        }
    }
}