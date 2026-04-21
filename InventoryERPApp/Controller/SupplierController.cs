using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryERPApp.DTO.Supplier;
using InventoryERPApp.Interfaces;

namespace InventoryERPApp.Controller;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> CreateSupplier([FromBody] SupplierCreateRequest request)
    {
        try
        {
            var result = await _supplierService.CreateSupplierAsync(request);
            return CreatedAtAction(nameof(GetSupplierById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierResponse>> GetSupplierById(int id)
    {
        var result = await _supplierService.GetSupplierByIdAsync(id);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SupplierResponse>>> GetAllSuppliers()
    {
        var result = await _supplierService.GetAllSuppliersAsync();
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<SupplierResponse>> UpdateSupplier([FromBody] SupplierUpdateRequest request)
    {
        try
        {
            var result = await _supplierService.UpdateSupplierAsync(request);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteSupplier(int id)
    {
        try
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            
            if (!result)
                return NotFound();

            return Ok(true);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult<bool>> ToggleSupplierStatus(int id)
    {
        try
        {
            var result = await _supplierService.ToggleSupplierStatusAsync(id);
            
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
