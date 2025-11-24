using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities.ENUMS
{
    public enum TransactionType
    {
        Purchase = 1,   // Закупка (приход)
        Sale = 2,       // Продажа (расход)
        Adjustment = 3, // Корректировка (инвентаризация)
        Transfer = 4    // Перемещение между складами
    }
}
