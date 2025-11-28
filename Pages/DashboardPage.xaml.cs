using Microsoft.EntityFrameworkCore;
using Project_Stroymagazin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Stroymagazin.Pages
{
    /// <summary>
    /// Логика взаимодействия для DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            using (var db = new AppDbContext())
            {
                
                TotalProductsText.Text = db.Products.Count().ToString();
                TotalWarehousesText.Text = db.Warehouses.Count().ToString();
                TotalUsersText.Text = db.Users.Count(u => u.IsActive).ToString();

                
                var totalValue = db.Stocks
                    .Include(s => s.Product)
                    .Sum(s => s.Quantity * s.Product.Price);

                StockValueText.Text = $"{totalValue:N0} ₽";
            }
        }
    }
}
