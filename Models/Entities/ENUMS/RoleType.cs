using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities.ENUMS
{
    public enum RoleType
    {
        Administrator = 1,
        WarehouseManager = 2, // Кладовщик
        Cashier = 3,          // Кассир
        PurchaseManager = 4   // Менеджер закупок
    }
}
