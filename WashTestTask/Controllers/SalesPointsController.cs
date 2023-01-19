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
    public class SalesPointsController : ControllerBase
    {
        private readonly ISalesPointService _salesPointService;
        private readonly ILogger<SalesPointsController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public SalesPointsController(ISalesPointService salesPointService, ILogger<SalesPointsController> logger, 
            IPublishEndpoint publishEndpoint)
        {
            _salesPointService = salesPointService;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        // GET: api/SalesPoints
        [HttpGet]
        public ActionResult<IEnumerable<SalesPointDTO>> GetSalesPoints()
        {
            _logger.LogInformation("Getting all sales points.");
            
            var salesPoints = _salesPointService.GetAll();
            
            return Ok(salesPoints.Select(_salesPointService.ToDto));
        }

        // GET: api/SalesPoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesPoint>> GetSalesPoint(int id)
        {
            _logger.LogInformation($"Getting sales point with id {id}.");
            
            var salesPoint = await _salesPointService.GetAsync(id);
            if (salesPoint == null)
            {
                _logger.LogWarning($"Sales point with id {id} not found.");
                return NotFound();
            }

            return Ok(_salesPointService.ToDto(salesPoint));
        }

        // POST: api/SalesPoints
        [HttpPost]
        public async Task<ActionResult<SalesPoint>> PostSalesPoint([FromBody] SalesPointDTO salesPointDto)
        {
            _logger.LogInformation("Creating new sales point.");
            
            var salesPoint = _salesPointService.ToEntity(salesPointDto);
            await _salesPointService.AddAsync(salesPoint);
            await _publishEndpoint.Publish<SalesPointCreated>(new
            {
                Id = salesPoint.Id,
                Name = salesPoint.Name
            });
            
            return CreatedAtAction("", new { id = salesPoint.Id }, salesPoint);
        }

        // PUT: api/SalesPoints/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesPoint(int id, [FromBody] SalesPointDTO salesPointDto)
        {
            _logger.LogInformation($"Updating sales point with id {id}.");
    
            var salesPoint = await _salesPointService.GetAsync(id);
            if (salesPoint == null)
            {
                _logger.LogWarning($"Sales point with id {id} not found.");
                return NotFound();
            }
            await _salesPointService.PutAsync(salesPoint, salesPointDto);
            await _publishEndpoint.Publish<SalesPointUpdated>(new { Id = salesPoint.Id });
    
            return Ok(salesPoint);
        }
        
        // DELETE: api/SalesPoints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleAsync(int id)
        {
            _logger.LogInformation($"Deleting sales point with id {id}.");

            var sale = await _salesPointService.GetAsync(id);
            if (sale == null)
            {
                _logger.LogWarning($"Sales point with id {id} not found.");
                return NotFound();
            }
            await _salesPointService.RemoveAsync(sale);
    
            return NoContent();
        }
    }
}