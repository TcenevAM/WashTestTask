using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WashTestTask.Dtos;
using WashTestTask.Models;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly Context _context;

        public CustomerService(Context context)
        {
            _context = context;
        }

        public async Task<Customer> GetAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null) return null;
            
            return customer;
        }
        
        public List<Customer> GetAll()
        {
            var customers = _context.Customers;

            return customers.ToList();
        }

        public CustomerDTO ToDto(Customer customer)
        {
            return new CustomerDTO { Id = customer.Id, Name = customer.Name, SalesIds = customer.SalesIds };
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task PutAsync(Customer customer, CustomerDTO customerDto)
        {
            customer.Name = customerDto.Name;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}