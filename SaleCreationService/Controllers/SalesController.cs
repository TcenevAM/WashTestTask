using System;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaleCreationService.Services;

namespace SaleCreationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly ISaleService _saleService;

        public SalesController(ILogger<SalesController> logger, ISaleService saleService)
        {
            _logger = logger;
            _saleService = saleService;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddSaleAsync([FromBody] SaleDTO saleDto)
        {
            _logger.LogInformation("Creating new sale.");
            
            var salePointResponse = new HttpResponseMessage();
            try
            {
                salePointResponse = await _saleService.GetSalesPointResponse(saleDto.SalesPointId);
                var salesPointDto =
                    JsonConvert.DeserializeObject<SalesPointDTO>(await salePointResponse.Content.ReadAsStringAsync());

                if (saleDto.CustomerId != null && !await _saleService.IsCustomerIdValid(saleDto.CustomerId.Value))
                    return NotFound($"Customer with id {saleDto.CustomerId} not found");

                await _saleService.ReduceProductAmountInSalesPoint(saleDto, salesPointDto);
                await _saleService.CreateSale(saleDto);

                return Ok();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Error while creating sale.");
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Error while creating sale.");
                var client = new HttpClient();
                await client.PutAsync($"/SalesPoints/{saleDto.SalesPointId}",
                    salePointResponse.Content);
                return BadRequest(e.Message);
            }
        }
    }
}