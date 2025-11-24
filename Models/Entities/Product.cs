using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string SKU { get; set; } = null!; // Артикул
        public string? Barcode { get; set; }
        public string UnitOfMeasure { get; set; } = "шт";
        public decimal Price { get; set; } // Розничная цена

        // Навигация
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>(); // Остатки на разных складах
    }
}
