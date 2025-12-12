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
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                OrdersGrid.ItemsSource = db.PurchaseOrders
                    .Include(o => o.Supplier)
                    .Include(o => o.Manager)
                    .OrderByDescending(o => o.Date)
                    .ToList();
            }
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderEditWindow().ShowDialog();
            LoadData();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadData();

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                if (MessageBox.Show("Удалить заказ?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var order = db.PurchaseOrders.Find(id);
                        if (order != null)
                        {
                            db.PurchaseOrders.Remove(order);
                            db.SaveChanges();
                            LoadData();
                        }
                    }
                }
            }
        }
    }
}
