using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public PurchaseOrder? Order { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ReceivedQuantity { get; set; } = 0;
        public decimal TotalPrice { get; set; }
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal Discount { get; set; } = 0;
    }
}
