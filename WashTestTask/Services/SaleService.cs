using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;
using WashTestTask.Database;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Services
{
    public class SaleService : ISaleService
    {
        private readonly Context _context;

        public SaleService(Context context)
        {
            _context = context;
        }

        public async Task<Sale> GetAsync(int productId)
        {
            var sale = await _context.Sales.FindAsync(productId);

            if (sale == null) return null;
            
            return sale;
        }

        public IEnumerable<Sale> GetAll()
        {
            var sales = _context.Sales;

            return sales;
        }

        public SaleDTO ToDto(Sale sale)
        {
            return new SaleDTO
            {
                Id = sale.Id, Date = sale.Date, CustomerId = sale.CustomerId, TotalAmount = sale.TotalAmount,
                SalesData = sale.SalesData.Select(ToSaleDataDto).ToList(), SalesPointId = sale.SalesPointId
            };
        }

        private SaleDataDTO ToSaleDataDto(SaleData data)
        {
            return new SaleDataDTO
            {
                ProductId = data.ProductId, ProductQuantity = data.ProductQuantity, ProductAmount = data.ProductAmount
            };
        }

        public Sale ToEntity(SaleDTO saleDto)
        {
            return new Sale
            {
                Date = saleDto.Date,
                SalesPointId = saleDto.SalesPointId,
                CustomerId = saleDto.CustomerId,
                SalesData = saleDto.SalesData.Select(sd => new SaleData
                {
                    ProductId = sd.ProductId,
                    ProductQuantity = sd.ProductQuantity,
                }).ToList()
            };
        }

        public async Task<List<string>> GetPropertiesWithInvalidData(SaleDTO saleDto)
        {
            var result = new List<string>();
            
            var salesPoint = await _context.SalesPoints.FindAsync(saleDto.SalesPointId);
            if (salesPoint == null)
            {
                result.Add("SalesPoint");
            }

            if (saleDto.CustomerId != null && await _context.Customers.FindAsync(saleDto.CustomerId) == null)
            {
                result.Add("CustomerId");
            }

            return result;
        }

        public async Task<Sale> AddAsync(Sale sale)
        {
            var salesPoint = await _context.SalesPoints.FindAsync(sale.SalesPointId);
            if (salesPoint == null)
            {
                throw new ArgumentException($"SalesPoint with id {sale.SalesPointId} does not exist");
            }

            var customer = await _context.Customers.FindAsync(sale.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with id {sale.CustomerId} does not exist");
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            
            return sale;
        }

        public async Task PutAsync(Sale sale, SaleDTO saleDto)
        {
            var customer = await _context.Customers.FindAsync(saleDto.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with id {sale.CustomerId} does not exist");
            }
            sale.Customer = customer;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Sale sale)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }
}