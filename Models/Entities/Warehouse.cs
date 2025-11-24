using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string? Address { get; set; }

        // Навигация
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
