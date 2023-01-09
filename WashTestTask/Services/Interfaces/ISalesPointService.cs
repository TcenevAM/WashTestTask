using System.Collections.Generic;
using System.Threading.Tasks;
using WashTestTask.Dtos;
using WashTestTask.Models;

namespace WashTestTask.Services.Interfaces
{
    public interface ISalesPointService
    {
        Task<SalesPoint> GetAsync(int productId);
        IEnumerable<SalesPoint> GetAll();
        SalesPointDTO ToDto(SalesPoint product);
        SalesPoint ToEntity(SalesPointDTO salesPointDto);
        Task<SalesPoint> AddAsync(SalesPoint product);
        Task PutAsync(SalesPoint product, SalesPointDTO productDto);
        Task RemoveAsync(SalesPoint sale);
    }
}