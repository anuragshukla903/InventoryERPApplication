using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryERPApp.DTO.Purchase;
using InventoryERPApp.Interfaces;

namespace InventoryERPApp.Controller;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseResponse>> CreatePurchase([FromBody] PurchaseCreateRequest request)
    {
        try
        {
            var result = await _purchaseService.CreatePurchaseAsync(request);
            return CreatedAtAction(nameof(GetPurchaseById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseResponse>> GetPurchaseById(int id)
    {
        var result = await _purchaseService.GetPurchaseByIdAsync(id);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<PurchaseResponse>>> GetAllPurchases()
    {
        var result = await _purchaseService.GetAllPurchasesAsync();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeletePurchase(int id)
    {
        try
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);
            
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
