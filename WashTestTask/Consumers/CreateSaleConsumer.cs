using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Consumers
{
    public class CreateSaleConsumer : IConsumer<CreateSale>
    {
        private readonly ISaleService _saleService;
        private readonly ILogger<CreateSaleConsumer> _logger;

        public CreateSaleConsumer(ISaleService saleService, ILogger<CreateSaleConsumer> logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateSale> context)
        {
            _logger.LogInformation($"Consuming sale creation");
            
            var sale = _saleService.ToEntity(context.Message.SaleDto);
            
            await _saleService.AddAsync(sale);
            await context.RespondAsync<CreateSaleResponse>(new { Result = _saleService.ToDto(sale) });
        }
    }
}