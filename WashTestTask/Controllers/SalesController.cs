using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly ISaleService _saleService;
        private readonly IRequestClient<HandleSaleCreationRequest> _saleCreationClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public SalesController(ILogger<SalesController> logger, ISaleService saleService,
            IRequestClient<HandleSaleCreationRequest> saleCreationClient, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _saleService = saleService;
            _saleCreationClient = saleCreationClient;
            _publishEndpoint = publishEndpoint;
        }
        
        // GET: api/Sales
        [HttpGet]
        public ActionResult<IEnumerable<SaleDTO>> GetSales()
        {
            _logger.LogInformation("Getting all sales.");

            var sales = _saleService.GetAll();
        
            return Ok(sales.Select(_saleService.ToDto));
        }
        
        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDTO>> GetSaleAsync(int id)
        {
            _logger.LogInformation($"Getting sale with id {id}.");
            
            var sale = await _saleService.GetAsync(id);
            if (sale == null)
            {
                _logger.LogError($"Sale with id {id} not found.");
                return NotFound();
            }

            return Ok(_saleService.ToDto(sale));
        }
        
        [HttpPost]
        public async Task<IActionResult> AddSaleAsync([FromBody] SaleDTO saleDto)
        {
            _logger.LogInformation("Creating new sale.");

            try
            {
                var invalidProperties = await _saleService.GetPropertiesWithInvalidData(saleDto);
                if (invalidProperties.Any())
                {
                    return BadRequest($"Dto have properties with invalid data: {string.Join(',', invalidProperties)}");
                }
                
                var response = await _saleCreationClient.GetResponse<CreateSaleResponse>(new { SaleDto = saleDto });
                await _publishEndpoint.Publish<SaleCreated>(new
                {
                    Id = response.Message.Result.Id,
                    CustomerId = response.Message.Result.CustomerId,
                    SalesPointId = response.Message.Result.SalesPointId,
                    Date = response.Message.Result.Date,
                    TotalAmount = response.Message.Result.TotalAmount
                });

                return CreatedAtAction("", new { id = response.Message.Result.Id }, saleDto);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Error while creating sale.");
                return BadRequest(e.Message);
            }
        }
        
        // PUT: api/Sales/5
        [HttpPut("{id}/customer")]
        public async Task<ActionResult<Sale>> PutSaleAsync(int id, [FromBody] SaleDTO saleDto)
        {
            _logger.LogInformation($"Updating sale with id {id}.");
    
            var sale = await _saleService.GetAsync(id);
            if (sale == null)
            {
                _logger.LogWarning($"Sale with id {id} not found.");
                return NotFound();
            }
            await _saleService.PutAsync(sale, saleDto);
            await _publishEndpoint.Publish<SaleUpdated>(new { Id = sale.Id });
    
            return Ok(sale);
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleAsync(int id)
        {
            _logger.LogInformation($"Deleting sale with id {id}.");

            var sale = await _saleService.GetAsync(id);
            if (sale == null)
            {
                _logger.LogWarning($"Sale with id {id} not found.");
                return NotFound();
            }
            await _saleService.RemoveAsync(sale);
    
            return NoContent();
        }
    }
}