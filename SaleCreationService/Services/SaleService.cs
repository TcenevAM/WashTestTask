using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SaleCreationService.Services
{
    public class SaleService : ISaleService
    {
        private readonly IConfiguration _configuration;
        private readonly string _dalApiBaseUrl;

        public SaleService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dalApiBaseUrl = _configuration["DALApiBaseUrl"];
        }

        public async Task<HttpResponseMessage> GetSalesPointResponse(int salesPointId)
        {
            var client = new HttpClient();
            
            var salePointResponse = await client.GetAsync($"{_dalApiBaseUrl}/SalesPoints/{salesPointId}");
            if (!salePointResponse.IsSuccessStatusCode)
                throw new ArgumentException($"Sale point with id {salesPointId} not found");
            return salePointResponse;
        }

        public async Task<bool> IsCustomerIdValid(int customerId)
        {
            var client = new HttpClient();
            
            var customerResponse = await client.GetAsync($"{_dalApiBaseUrl}/Customers/{customerId}");
            return customerResponse.IsSuccessStatusCode;
        }
        
        public async Task ReduceProductAmountInSalesPoint(SaleDTO saleDto, SalesPointDTO salesPointDto)
        {
            var client = new HttpClient();
            
            foreach (var saleDataDto in saleDto.SalesData)
            {
                var productInfo =
                    salesPointDto.ProvidedProducts.FirstOrDefault(p => p.ProductId == saleDataDto.ProductId);
                
                if (productInfo == null)
                    throw new ArgumentException(
                        $"Sale point with id {saleDto.SalesPointId} does not contain product with id {saleDataDto.ProductId}");
                
                if (productInfo.Quantity < saleDataDto.ProductQuantity)
                    throw new ArgumentException(
                        $"Not enough product quantity with id {saleDataDto.ProductId} on sale point with id {productInfo.Id}");
                
                productInfo.Quantity -= saleDataDto.ProductQuantity;
            }

            await client.PutAsync($"https://localhost:5001/api/SalesPoints/{saleDto.SalesPointId}",
                new StringContent(JsonConvert.SerializeObject(salesPointDto), Encoding.UTF8, "application/json"));
        }

        public async Task CreateSale(SaleDTO saleDto)
        {
            var client = new HttpClient();
            
            var saleResponse = await client.PostAsync($"{_dalApiBaseUrl}/Sales",
                new StringContent(JsonConvert.SerializeObject(saleDto), Encoding.UTF8, "application/json"));
            if (!saleResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to create sale");
            }
        }
    }
}