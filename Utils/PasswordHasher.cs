using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Utils
{
    public static class PasswordHasher
    {
        // Метод для хеширования пароля
        public static string HashPassword(string password)
        {
            
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        // Метод для проверки пароля
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
