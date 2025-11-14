using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using

namespace Project_Stroymagazin.Models.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public int? StatusId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? ExpectedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public string? Notes { get; set; }
        public decimal Discount { get; set; } = 0;
        public int? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
