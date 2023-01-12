using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;

namespace WashTestTask.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetAsync(int customerId);
        List<Customer> GetAll();
        CustomerDTO ToDto(Customer customer);
        Task<Customer> AddAsync(Customer customer);
        Task PutAsync(Customer customer, CustomerDTO customerDto);
        Task RemoveAsync(Customer customer);
    }
}