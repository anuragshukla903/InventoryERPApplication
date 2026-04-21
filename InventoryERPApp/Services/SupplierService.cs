using InventoryERPApp.DTO.Supplier;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _context;

    public SupplierService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierResponse> CreateSupplierAsync(SupplierCreateRequest request)
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            GSTIN = request.GSTIN
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return await GetSupplierByIdAsync(supplier.Id) ?? throw new InvalidOperationException("Failed to retrieve created supplier");
    }

    public async Task<SupplierResponse?> GetSupplierByIdAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Purchases)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return null;

        return new SupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Address = supplier.Address,
            Phone = supplier.Phone,
            Email = supplier.Email,
            GSTIN = supplier.GSTIN,
            CreateAt = supplier.CreateAt,
            ModifiedAt = supplier.ModifiedAt,
            IsActive = supplier.IsActive,
            PurchaseCount = supplier.Purchases.Count
        };
    }

    public async Task<List<SupplierResponse>> GetAllSuppliersAsync()
    {
        var suppliers = await _context.Suppliers
            .Include(s => s.Purchases)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return suppliers.Select(supplier => new SupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Address = supplier.Address,
            Phone = supplier.Phone,
            Email = supplier.Email,
            GSTIN = supplier.GSTIN,
            CreateAt = supplier.CreateAt,
            ModifiedAt = supplier.ModifiedAt,
            IsActive = supplier.IsActive,
            PurchaseCount = supplier.Purchases.Count
        }).ToList();
    }

    public async Task<SupplierResponse?> UpdateSupplierAsync(SupplierUpdateRequest request)
    {
        var supplier = await _context.Suppliers.FindAsync(request.Id);
        
        if (supplier == null) return null;

        supplier.Name = request.Name;
        supplier.Address = request.Address;
        supplier.Phone = request.Phone;
        supplier.Email = request.Email;
        supplier.GSTIN = request.GSTIN;

        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();

        return await GetSupplierByIdAsync(supplier.Id);
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        
        if (supplier == null) return false;

        // Check if supplier has any purchases
        var hasPurchases = await _context.Purchases.AnyAsync(p => p.SupplierId == id);
        if (hasPurchases)
        {
            throw new InvalidOperationException("Cannot delete supplier with existing purchases. Consider deactivating instead.");
        }

        supplier.IsDelete = true;
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleSupplierStatusAsync(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        
        if (supplier == null) return false;

        supplier.IsActive = !supplier.IsActive;
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();

        return true;
    }
}
