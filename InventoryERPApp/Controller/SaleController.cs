using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryERPApp.DTO.Sale;
using InventoryERPApp.Interfaces;

namespace InventoryERPApp.Controller;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public async Task<ActionResult<SaleResponse>> CreateSale([FromBody] SaleCreateRequest request)
    {
        try
        {
            var result = await _saleService.CreateSaleAsync(request);
            return CreatedAtAction(nameof(GetSaleById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleResponse>> GetSaleById(int id)
    {
        var result = await _saleService.GetSaleByIdAsync(id);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SaleResponse>>> GetAllSales()
    {
        var result = await _saleService.GetAllSalesAsync();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteSale(int id)
    {
        try
        {
            var result = await _saleService.DeleteSaleAsync(id);
            
            if (!result)
                return NotFound();

            return Ok(true);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
