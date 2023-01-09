using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WashTestTask.Dtos;
using WashTestTask.Models;
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

        public async Task ReduceProductAmountInSalesPoint(Sale sale)
        {
            foreach (var saleData in sale.SalesData)
            {
                var salesPoint = sale.SalesPoint;
                var providedProduct = salesPoint.ProvidedProducts.First(p => p.ProductId == saleData.ProductId);
                if (providedProduct.Quantity < saleData.ProductQuantity)
                    throw new ArgumentException(
                        $"Sales point with id {sale.SalesPointId} does not contain enough product with id {providedProduct.ProductId}");
                providedProduct.Quantity -= saleData.ProductQuantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Sale> AddAsync(Sale sale)
        {
            var salesPoint = await _context.SalesPoints.FindAsync(sale.SalesPointId);
            if (salesPoint == null)
            {
                throw new ArgumentException($"SalesPoint with id {sale.SalesPointId} does not exist");
            }

            var availableProductsIds = salesPoint.ProvidedProducts.Select(p => p.ProductId).ToList();
            var requestedProductsIds = sale.SalesData.Select(sd => sd.ProductId).ToList();
            if (availableProductsIds.Except(requestedProductsIds).Any())
            {
                throw new ArgumentException(
                    $"SalesPoint with id {sale.SalesPointId} does not contain the products with ids {requestedProductsIds.Except(availableProductsIds)}");
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