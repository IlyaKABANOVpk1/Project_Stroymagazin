using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Project_Stroymagazin.Models.Entities.ENUMS;

namespace Project_Stroymagazin.Models.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public int ManagerId { get; set; } // Кто создал
        public User Manager { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
