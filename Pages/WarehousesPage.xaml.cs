using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
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
    /// Логика взаимодействия для WarehousesPage.xaml
    /// </summary>
    public partial class WarehousesPage : Page
    {
        public WarehousesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                WarehousesGrid.ItemsSource = db.Warehouses.ToList();
            }
        }

        private void AddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewNameBox.Text))
            {
                MessageBox.Show("Введите название склада");
                return;
            }

            using (var db = new AppDbContext())
            {
                var warehouse = new Warehouse
                {
                    Name = NewNameBox.Text,
                    Address = NewAddressBox.Text
                };
                db.Warehouses.Add(warehouse);
                db.SaveChanges();
            }

            NewNameBox.Clear();
            NewAddressBox.Clear();
            LoadData();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                if (MessageBox.Show("Удалить склад? Это может повлиять на остатки.", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var w = db.Warehouses.Find(id);
                        if (w != null)
                        {
                            db.Warehouses.Remove(w);
                            db.SaveChanges();
                            LoadData();
                        }
                    }
                }
            }
        }
    }
}
