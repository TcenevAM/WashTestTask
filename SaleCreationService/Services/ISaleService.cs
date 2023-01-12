using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;

namespace SaleCreationService.Services
{
    public interface ISaleService
    {
        Task<HttpResponseMessage> GetSalesPointResponse(int salesPointId);
        Task<bool> IsCustomerIdValid(int customerId);
        Task ReduceProductAmountInSalesPoint(SaleDTO saleDto, SalesPointDTO salesPointDto);
        Task CreateSale(SaleDTO saleDto);
    }
}