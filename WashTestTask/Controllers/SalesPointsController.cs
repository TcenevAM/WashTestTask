using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Dtos;
using Data.Models;
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

        public SalesPointsController(ISalesPointService salesPointService, ILogger<SalesPointsController> logger)
        {
            _salesPointService = salesPointService;
            _logger = logger;
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