using InventoryERPApp.DTO.Purchase;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class PurchaseService : IPurchaseService
{
    private readonly ApplicationDbContext _context;

    public PurchaseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseResponse> CreatePurchaseAsync(PurchaseCreateRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Create Purchase
            var purchase = new Purchase
            {
                SupplierId = request.SupplierId,
                InvoiceNo = request.InvoiceNo,
                Date = request.Date,
                TotalAmount = request.Items.Sum(item => item.Quantity * item.Rate)
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Create Purchase Items and related entities
            var purchaseItems = new List<PurchaseItem>();
            
            foreach (var itemRequest in request.Items)
            {
                var purchaseItem = new PurchaseItem
                {
                    PurchaseId = purchase.Id,
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    Rate = itemRequest.Rate,
                    BatchNo = itemRequest.BatchNo,
                    ExpiryDate = itemRequest.ExpiryDate
                };

                _context.PurchaseItems.Add(purchaseItem);
                purchaseItems.Add(purchaseItem);
            }

            await _context.SaveChangesAsync();

            // Create Stock Batches, Stock Ledger, and Update Current Stock
            foreach (var item in purchaseItems)
            {
                // Create Stock Batch
                var stockBatch = new StockBatch
                {
                    ProductId = item.ProductId,
                    BatchNo = item.BatchNo ?? $"BATCH-{DateTime.Now:yyyyMMdd}-{item.Id}",
                    PurchaseItemId = item.Id,
                    RemainingQty = item.Quantity,
                    Rate = item.Rate,
                    ExpiryDate = item.ExpiryDate,
                    CreatedDate = DateTime.UtcNow
                };

                _context.StockBatches.Add(stockBatch);
                await _context.SaveChangesAsync();

                // Create Stock Ledger Entry (IN)
                var stockLedger = new StockLedger
                {
                    ProductId = item.ProductId,
                    Date = purchase.Date,
                    Type = "IN",
                    Quantity = item.Quantity,
                    ReferenceType = "Purchase",
                    ReferenceId = purchase.Id,
                    BatchId = stockBatch.Id
                };

                _context.StockLedgers.Add(stockLedger);

                // Update or Create Current Stock
                var currentStock = await _context.CurrentStocks
                    .FirstOrDefaultAsync(cs => cs.ProductId == item.ProductId);

                if (currentStock != null)
                {
                    currentStock.Quantity += item.Quantity;
                }
                else
                {
                    currentStock = new CurrentStock
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    _context.CurrentStocks.Add(currentStock);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Return response
            return await GetPurchaseByIdAsync(purchase.Id) ?? throw new InvalidOperationException("Failed to retrieve created purchase");
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PurchaseResponse?> GetPurchaseByIdAsync(int id)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (purchase == null) return null;

        return new PurchaseResponse
        {
            Id = purchase.Id,
            SupplierId = purchase.SupplierId,
            SupplierName = purchase.Supplier.Name,
            InvoiceNo = purchase.InvoiceNo,
            Date = purchase.Date,
            TotalAmount = purchase.TotalAmount,
            CreateAt = purchase.CreateAt,
            Items = purchase.PurchaseItems.Select(item => new PurchaseItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                Rate = item.Rate,
                Amount = item.Quantity * item.Rate,
                BatchNo = item.BatchNo,
                ExpiryDate = item.ExpiryDate
            }).ToList()
        };
    }

    public async Task<List<PurchaseResponse>> GetAllPurchasesAsync()
    {
        var purchases = await _context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        return purchases.Select(purchase => new PurchaseResponse
        {
            Id = purchase.Id,
            SupplierId = purchase.SupplierId,
            SupplierName = purchase.Supplier.Name,
            InvoiceNo = purchase.InvoiceNo,
            Date = purchase.Date,
            TotalAmount = purchase.TotalAmount,
            CreateAt = purchase.CreateAt,
            Items = purchase.PurchaseItems.Select(item => new PurchaseItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                Rate = item.Rate,
                Amount = item.Quantity * item.Rate,
                BatchNo = item.BatchNo,
                ExpiryDate = item.ExpiryDate
            }).ToList()
        }).ToList();
    }

    public async Task<bool> DeletePurchaseAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null) return false;

            // Get all related items to reverse stock entries
            var purchaseItems = purchase.PurchaseItems.ToList();

            // Remove Stock Ledger entries
            var stockLedgerEntries = await _context.StockLedgers
                .Where(sl => sl.ReferenceType == "Purchase" && sl.ReferenceId == id)
                .ToListAsync();

            _context.StockLedgers.RemoveRange(stockLedgerEntries);

            // Remove Stock Batches
            var stockBatches = await _context.StockBatches
                .Where(sb => purchaseItems.Select(pi => pi.Id).Contains(sb.PurchaseItemId))
                .ToListAsync();

            _context.StockBatches.RemoveRange(stockBatches);

            // Update Current Stock (reverse)
            foreach (var item in purchaseItems)
            {
                var currentStock = await _context.CurrentStocks
                    .FirstOrDefaultAsync(cs => cs.ProductId == item.ProductId);

                if (currentStock != null)
                {
                    currentStock.Quantity -= item.Quantity;
                    if (currentStock.Quantity <= 0)
                    {
                        _context.CurrentStocks.Remove(currentStock);
                    }
                }
            }

            // Remove Purchase Items
            _context.PurchaseItems.RemoveRange(purchaseItems);

            // Remove Purchase
            _context.Purchases.Remove(purchase);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
