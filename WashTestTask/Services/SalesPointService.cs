using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using WashTestTask.Database;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Services
{
    public class SalesPointService : ISalesPointService
    {
        private readonly Context _context;

        public SalesPointService(Context context)
        {
            _context = context;
        }
        
        public async Task<SalesPoint> GetAsync(int productId)
        {
            var product = await _context.SalesPoints.FindAsync(productId);
            
            if (product == null) return null;
                        
            return product;
        }

        public IEnumerable<SalesPoint> GetAll()
        {
            var salesPoint = _context.SalesPoints;

            return salesPoint;
        }

        public SalesPointDTO ToDto(SalesPoint salesPoint)
        {
            return new SalesPointDTO
            {
                Id = salesPoint.Id,
                Name = salesPoint.Name,
                ProvidedProducts = salesPoint.ProvidedProducts.Select(p => new ProvidedProductDTO
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };
        }

        public SalesPoint ToEntity(SalesPointDTO salesPointDto)
        {
            return new SalesPoint
            {
                Name = salesPointDto.Name,
                ProvidedProducts = salesPointDto.ProvidedProducts.Select(p => new ProvidedProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };
        }

        public async Task<SalesPoint> AddAsync(SalesPoint salesPoint)
        {
            _context.SalesPoints.Add(salesPoint);
            await _context.SaveChangesAsync();
            return salesPoint;
        }

        public async Task PutAsync(SalesPoint salesPoint, SalesPointDTO salesPointDto)
        {
            salesPoint.Name = salesPointDto.Name;
            salesPoint.ProvidedProducts = salesPointDto.ProvidedProducts.Select(p => new ProvidedProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList();
            
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(SalesPoint salesPoint)
        {
            _context.SalesPoints.Remove(salesPoint);
            await _context.SaveChangesAsync();
        }
    }
}