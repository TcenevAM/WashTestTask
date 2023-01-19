using Data.Dtos;

namespace Data.Contracts
{
    public interface HandleSaleCreationRequest
    {
        public SaleDTO SaleDto { get; set; }
    }
}