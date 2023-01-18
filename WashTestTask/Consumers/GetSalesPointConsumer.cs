using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using WashTestTask.Services;

namespace WashTestTask.Consumers
{
    public class GetSalesPointConsumer : IConsumer<GetSalesPoint>
    {
        private readonly SalesPointService _salesPointService;

        public GetSalesPointConsumer(SalesPointService salesPointService)
        {
            _salesPointService = salesPointService;
        }

        public async Task Consume(ConsumeContext<GetSalesPoint> context)
        {
            var salesPoint = await _salesPointService.GetAsync(context.Message.SalesPointId);
            
            await context.RespondAsync<GetSalesPointResponse>(new 
            {
                Id = salesPoint.Id,
                Name = salesPoint.Name,
                ProvidedProducts = salesPoint.ProvidedProducts.Select(p => new { p.Id, p.ProductId, p.Quantity })
            });
        }
    }
}