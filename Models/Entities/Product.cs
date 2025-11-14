using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string SKU { get; set; } = null!;
        public int? CategoryId { get; set; }
        public ProductCategory? Category { get; set; }
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RetailPrice { get; set; }
        public string UnitOfMeasure { get; set; } = null!;
        public decimal MinStockLevel { get; set; } = 0;
        public decimal MaxStockLevel { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Barcode { get; set; }

        public StockLevel? StockLevel { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
