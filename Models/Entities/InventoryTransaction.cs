using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? TransactionTypeId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public int? OrderId { get; set; }
        public PurchaseOrder? Order { get; set; }
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string? Notes { get; set; }
    }
}
