using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;

namespace WashTestTask.Services.Interfaces
{
    public interface ISaleService
    {
        Task<Sale> GetAsync(int productId);
        IEnumerable<Sale> GetAll();
        SaleDTO ToDto(Sale product);
        Task<Sale> AddAsync(Sale product);
        Task PutAsync(Sale product, SaleDTO productDto);
        Task RemoveAsync(Sale sale);
        Sale ToEntity(SaleDTO saleDto);
        Task<List<string>> GetPropertiesWithInvalidData(SaleDTO saleDto);
    }
}