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
            // 1. Валидация на пустые поля
            string name = NewNameBox.Text.Trim();
            string address = NewAddressBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название склада!", "Валидация");
                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Введите адрес склада!", "Валидация");
                return;
            }

            using (var db = new AppDbContext())
            {
                // 2. Проверка на уникальность названия (чтобы не путаться)
                if (db.Warehouses.Any(w => w.Name.ToLower() == name.ToLower()))
                {
                    MessageBox.Show("Склад с таким названием уже существует!", "Ошибка");
                    return;
                }

                var warehouse = new Warehouse
                {
                    Name = name,
                    Address = address
                };

                db.Warehouses.Add(warehouse);
                db.SaveChanges();
            }

            NewNameBox.Clear();
            NewAddressBox.Clear();
            LoadData();
            MessageBox.Show("Склад успешно добавлен.");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                using (var db = new AppDbContext())
                {
                    // 3. Валидация перед удалением: есть ли на складе товары или операции?
                    bool hasStocks = db.Stocks.Any(s => s.WarehouseId == id && s.Quantity != 0);
                    bool hasTransactions = db.InventoryTransactions.Any(t => t.WarehouseId == id);

                    if (hasStocks || hasTransactions)
                    {
                        MessageBox.Show("Нельзя удалить склад, на котором есть товары или по которому проводились операции.\n" +
                                        "Сначала переместите товары или удалите связанные записи.", "Ошибка удаления");
                        return;
                    }

                    if (MessageBox.Show("Вы уверены, что хотите удалить этот склад?", "Подтверждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
