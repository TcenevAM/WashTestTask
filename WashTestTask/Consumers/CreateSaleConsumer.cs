using System.Threading.Tasks;
using Data.Contracts;
using MassTransit;
using WashTestTask.Services;

namespace WashTestTask.Consumers
{
    public class CreateSaleConsumer : IConsumer<CreateSale>
    {
        private readonly SaleService _saleService;

        public CreateSaleConsumer(SaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task Consume(ConsumeContext<CreateSale> context)
        {
            var sale = _saleService.ToEntity(context.Message.SaleDto);
            
            await _saleService.AddAsync(sale);
        }
    }
}