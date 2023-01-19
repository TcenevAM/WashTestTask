using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Contracts;
using Data.Dtos;
using Data.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WashTestTask.Services.Interfaces;

namespace WashTestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IPublishEndpoint _publishEndpoint;

        public CustomersController(ILogger<CustomersController> logger, ICustomerService customerService,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _customerService = customerService;
            _publishEndpoint = publishEndpoint;
        }

        // GET: api/Customers
        [HttpGet]
        public ActionResult<IEnumerable<CustomerDTO>> GetCustomers()
        {
            _logger.LogInformation("Getting all customers.");

            var customers = _customerService.GetAll();
            
            return Ok(customers.Select(_customerService.ToDto));
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerAsync(int id)
        {
            _logger.LogInformation($"Getting customer with id {id}.");
            
            var customer = await _customerService.GetAsync(id);
            if (customer == null)
            {
                _logger.LogError($"Customer with id {id} not found.");
                return NotFound();
            }

            return Ok(_customerService.ToDto(customer));
        }
        
        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerDTO customerDto)
        {
            _logger.LogInformation("Creating new customer.");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for customer creation.");
                return BadRequest(ModelState);
            }
         
            var customer = new Customer
            {
                Name = customerDto.Name
            };
            customer = await _customerService.AddAsync(customer);
            await _publishEndpoint.Publish<CustomerCreated>(new { Id = customer.Id, Name = customer.Name });
            
            return CreatedAtAction("", new { id = customer.Id }, customerDto);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> PutCustomerAsync(int id, [FromBody] CustomerDTO customerDto)
        {
            _logger.LogInformation($"Updating customer with id {id}.");
            
            var customer = await _customerService.GetAsync(id);
            if (customer == null)
            {
                _logger.LogWarning($"customer with id {id} not found.");
                return NotFound();
            }
            await _customerService.PutAsync(customer, customerDto);
            await _publishEndpoint.Publish<CustomerUpdated>(new { Id = customer.Id });
            
            return Ok(_customerService.ToDto(customer));
        }
        
        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            _logger.LogInformation($"Deleting customer with id {id}.");

            var customer = await _customerService.GetAsync(id);
            if (customer == null)
            {
                _logger.LogWarning($"customer with id {id} not found.");
                return NotFound();
            }
            await _customerService.RemoveAsync(customer);
            
            return NoContent();
        }
    }
}