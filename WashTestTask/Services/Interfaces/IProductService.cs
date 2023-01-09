using System.Collections.Generic;
using System.Threading.Tasks;
using WashTestTask.Dtos;
using WashTestTask.Models;

namespace WashTestTask.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetAsync(int productId);
        IEnumerable<Product> GetAll();
        ProductDTO ToDto(Product product);
        Task<Product> AddAsync(Product product);
        Task PutAsync(Product product, ProductDTO productDto);
        Task RemoveAsync(Product product);
    }
}