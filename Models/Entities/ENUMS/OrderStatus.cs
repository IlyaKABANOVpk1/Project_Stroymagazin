using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models.Entities.ENUMS
{
    public enum OrderStatus
    {
        Pending = 1,    // В ожидании
        Received = 2,   // Получен
        Cancelled = 3   // Отменен
    }
}
