using Project_Stroymagazin.Models.Entities.ENUMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public RoleType Role { get; set; } 
        public bool IsActive { get; set; } = true;

        // Навигация
        public ICollection<PurchaseOrder> Orders { get; set; } = new List<PurchaseOrder>();
    }
}
