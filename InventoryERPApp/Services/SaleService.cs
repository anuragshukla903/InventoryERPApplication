using InventoryERPApp.DTO.Sale;
using InventoryERPApp.Interfaces;
using InventoryERPApp.Model;
using InventoryERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryERPApp.Services;

public class SaleService : ISaleService
{
    private readonly ApplicationDbContext _context;

    public SaleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SaleResponse> CreateSaleAsync(SaleCreateRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Validate request
            //ValidateSaleRequest(request);

            // Validate stock availability before proceeding
            await ValidateStockAvailabilityAsync(request.Items);

            // Get or create customer
            var customerId = await GetOrCreateCustomerAsync(request);

            // Create Sale
            var sale = new Sale
            {
                CustomerId = customerId,
                Date = request.Date,
                TotalAmount = request.Items.Sum(item => item.Quantity * item.Rate)
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Create Sale Items and process stock
            var saleItems = new List<SaleItem>();
            
            foreach (var itemRequest in request.Items)
            {
                var saleItem = new SaleItem
                {
                    SaleId = sale.Id,
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    Rate = itemRequest.Rate
                };

                _context.SaleItems.Add(saleItem);
                saleItems.Add(saleItem);
            }

            await _context.SaveChangesAsync();

            // Process stock updates for each item
            foreach (var item in saleItems)
            {
                await ProcessStockForSaleItemAsync(item, sale.Date, sale.Id);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetSaleByIdAsync(sale.Id) ?? throw new InvalidOperationException("Failed to retrieve created sale");
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task ValidateStockAvailabilityAsync(List<SaleItemRequest> items)
    {
        foreach (var item in items)
        {
            var currentStock = await _context.CurrentStocks
                .FirstOrDefaultAsync(cs => cs.ProductId == item.ProductId);

            if (currentStock == null || currentStock.Quantity < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for Product ID {item.ProductId}. Available: {currentStock?.Quantity ?? 0}, Required: {item.Quantity}");
            }
        }
    }

    private async Task ProcessStockForSaleItemAsync(SaleItem saleItem, DateTime saleDate, int saleId)
    {
        var remainingQuantity = saleItem.Quantity;
        
        // Get available stock batches ordered by expiry date (FIFO - First In First Out)
        var availableBatches = await _context.StockBatches
            .Where(sb => sb.ProductId == saleItem.ProductId && sb.RemainingQty > 0)
            .OrderBy(sb => sb.ExpiryDate ?? DateTime.MaxValue)
            .ThenBy(sb => sb.CreatedDate)
            .ToListAsync();

        if (availableBatches.Sum(sb => sb.RemainingQty) < saleItem.Quantity)
        {
            throw new InvalidOperationException($"Insufficient stock in batches for Product ID {saleItem.ProductId}");
        }

        // Process each batch until the required quantity is fulfilled
        foreach (var batch in availableBatches)
        {
            if (remainingQuantity <= 0) break;

            var quantityToUse = Math.Min(remainingQuantity, batch.RemainingQty);
            
            // Update StockBatch
            batch.RemainingQty -= quantityToUse;
            
            // Create Stock Ledger Entry (OUT)
            var stockLedger = new StockLedger
            {
                ProductId = saleItem.ProductId,
                Date = saleDate,
                Type = "OUT",
                Quantity = quantityToUse,
                ReferenceType = "Sale",
                ReferenceId = saleId,
                BatchId = batch.Id
            };

            _context.StockLedgers.Add(stockLedger);
            remainingQuantity -= quantityToUse;
        }

        // Update Current Stock
        var currentStock = await _context.CurrentStocks
            .FirstOrDefaultAsync(cs => cs.ProductId == saleItem.ProductId);

        if (currentStock != null)
        {
            currentStock.Quantity -= saleItem.Quantity;
            if (currentStock.Quantity <= 0)
            {
                _context.CurrentStocks.Remove(currentStock);
            }
        }
    }

    public async Task<SaleResponse?> GetSaleByIdAsync(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null) return null;

        return new SaleResponse
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            CustomerName = sale.Customer.Name,
            Date = sale.Date,
            TotalAmount = sale.TotalAmount,
            CreateAt = sale.CreateAt,
            Items = sale.SaleItems.Select(item => new SaleItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                Rate = item.Rate,
                Amount = item.Quantity * item.Rate
            }).ToList()
        };
    }

    public async Task<List<SaleResponse>> GetAllSalesAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.Date)
            .ToListAsync();

        return sales.Select(sale => new SaleResponse
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            CustomerName = sale.Customer.Name,
            Date = sale.Date,
            TotalAmount = sale.TotalAmount,
            CreateAt = sale.CreateAt,
            Items = sale.SaleItems.Select(item => new SaleItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                Rate = item.Rate,
                Amount = item.Quantity * item.Rate
            }).ToList()
        }).ToList();
    }

    public async Task<bool> DeleteSaleAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return false;

            // Get all related items to reverse stock entries
            var saleItems = sale.SaleItems.ToList();

            // Reverse stock entries for each item
            foreach (var item in saleItems)
            {
                await ReverseStockForSaleItemAsync(item, sale.Date, sale.Id);
            }

            // Remove Sale Items
            _context.SaleItems.RemoveRange(saleItems);

            // Remove Sale
            _context.Sales.Remove(sale);

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

    private async Task ReverseStockForSaleItemAsync(SaleItem saleItem, DateTime saleDate, int saleId)
    {
        // Get all stock ledger entries for this sale item
        var stockLedgerEntries = await _context.StockLedgers
            .Where(sl => sl.ReferenceType == "Sale" && sl.ReferenceId == saleId && sl.ProductId == saleItem.ProductId)
            .ToListAsync();

        // Reverse each batch entry
        foreach (var ledgerEntry in stockLedgerEntries)
        {
            if (ledgerEntry.BatchId.HasValue)
            {
                var batch = await _context.StockBatches.FindAsync(ledgerEntry.BatchId.Value);
                if (batch != null)
                {
                    batch.RemainingQty += ledgerEntry.Quantity;
                }
            }

            // Remove the stock ledger entry
            _context.StockLedgers.Remove(ledgerEntry);
        }

        // Update Current Stock (add back the quantity)
        var currentStock = await _context.CurrentStocks
            .FirstOrDefaultAsync(cs => cs.ProductId == saleItem.ProductId);

        if (currentStock != null)
        {
            currentStock.Quantity += saleItem.Quantity;
        }
        else
        {
            currentStock = new CurrentStock
            {
                ProductId = saleItem.ProductId,
                Quantity = saleItem.Quantity
            };
            _context.CurrentStocks.Add(currentStock);
        }
    }

    

    private async Task<int> GetOrCreateCustomerAsync(SaleCreateRequest request)
    {
        if (request.CustomerId.HasValue)
        {
            // Validate that the customer exists
            var customer = await _context.Customers.FindAsync(request.CustomerId.Value);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {request.CustomerId.Value} not found.");
            }
            return request.CustomerId.Value;
        }

        // If customer name and mobile are provided, create new customer
        if (!string.IsNullOrWhiteSpace(request.CustomerName) && !string.IsNullOrWhiteSpace(request.Mobile))
        {
            // Create new customer
            var newCustomer = new Customer
            {
                Name = request.CustomerName,
                Mobile = request.Mobile
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return newCustomer.Id;
        }

        // Default to walk-in customer with ID=1
        return 1;
    }
}
