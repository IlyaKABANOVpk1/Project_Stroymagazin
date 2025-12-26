using Microsoft.EntityFrameworkCore;
using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Models.Entities.ENUMS;
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
    /// Логика взаимодействия для InventoryHistoryPage.xaml
    /// </summary>
    public partial class InventoryHistoryPage : Page
    {
        public InventoryHistoryPage()
        {
            InitializeComponent();
            LoadFilterData();
            LoadHistory();
        }

        private void LoadFilterData()
        {
            using (var db = new AppDbContext())
            {
                TypeCombo.ItemsSource = Enum.GetValues(typeof(TransactionType));
                WarehouseCombo.ItemsSource = db.Warehouses.ToList();
                UserCombo.ItemsSource = db.Users.ToList();
            }
            TypeCombo.SelectedIndex = -1;
            WarehouseCombo.SelectedIndex = -1;
            UserCombo.SelectedIndex = -1;
        }

        private void LoadHistory()
        {
            using (var db = new AppDbContext())
            {
                var query = db.InventoryTransactions
                    .Include(t => t.Warehouse)
                    .Include(t => t.User)
                    .AsQueryable();

                if (TypeCombo.SelectedItem is TransactionType type)
                {
                    query = query.Where(t => t.Type == type);
                }
                if (WarehouseCombo.SelectedValue is int warehouseId)
                {
                    query = query.Where(t => t.WarehouseId == warehouseId);
                }
                if (UserCombo.SelectedValue is int userId)
                {
                    query = query.Where(t => t.UserId == userId);
                }

                HistoryGrid.ItemsSource = query.OrderByDescending(t => t.Date).ToList();
            }
        }

        private void ApplyFilters_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadHistory();

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            TypeCombo.SelectedIndex = -1;
            WarehouseCombo.SelectedIndex = -1;
            UserCombo.SelectedIndex = -1;
            LoadHistory();
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InventoryTransaction transaction)
            {
                // Создаем экземпляр окна деталей, передавая выбранную транзакцию
                TransactionDetailsWindow detailsWindow = new TransactionDetailsWindow(transaction);

                // Устанавливаем текущее окно как владельца (чтобы модальное окно было по центру)
                detailsWindow.Owner = Window.GetWindow(this);

                // Показываем окно как модальное
                detailsWindow.ShowDialog();
            }
        }
    }
}
