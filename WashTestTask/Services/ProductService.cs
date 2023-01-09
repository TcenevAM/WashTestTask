using System.Collections.Generic;
using System.Threading.Tasks;
using WashTestTask.Dtos;
using WashTestTask.Models;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Services
{
    public class ProductService : IProductService
    {
        private readonly Context _context;

        public ProductService(Context context)
        {
            _context = context;
        }

        public async Task<Product> GetAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null) return null;
            
            return product;
        }
        
        public IEnumerable<Product> GetAll()
        {
            var products = _context.Products;

            return products;
        }

        public ProductDTO ToDto(Product product)
        {
            return new ProductDTO { Id = product.Id, Name = product.Title, Price = product.Price};
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task PutAsync(Product product, ProductDTO productDto)
        {
            product.Title = productDto.Name;
            product.Price = productDto.Price;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}