using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Consumers
{
    public class GetSalesPointConsumer : IConsumer<GetSalesPoint>
    {
        private readonly ISalesPointService _salesPointService;
        private readonly ILogger<GetSalesPointConsumer> _logger;

        public GetSalesPointConsumer(ISalesPointService salesPointService, ILogger<GetSalesPointConsumer> logger)
        {
            _salesPointService = salesPointService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetSalesPoint> context)
        {
            _logger.LogInformation($"Consuming get sales point with id: {context.Message.SalesPointId}");
            
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