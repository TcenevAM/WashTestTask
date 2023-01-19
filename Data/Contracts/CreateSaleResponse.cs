using Data.Dtos;
using Data.Models;

namespace Data.Contracts
{
    public interface CreateSaleResponse
    {
        public SaleDTO Result { get; set; }
    }
}