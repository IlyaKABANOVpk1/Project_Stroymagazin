using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class StockLevel
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal CurrentStock { get; set; } = 0;
        public decimal ReservedStock { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
    }
}
