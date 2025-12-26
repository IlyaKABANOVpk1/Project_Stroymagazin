using Microsoft.EntityFrameworkCore;
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
using System.Windows.Shapes;

namespace Project_Stroymagazin
{
    /// <summary>
    /// Логика взаимодействия для TransactionDetailsWindow.xaml
    /// </summary>
    public partial class TransactionDetailsWindow : Window
    {
        public TransactionDetailsWindow(InventoryTransaction selectedTransaction)
        {
            InitializeComponent();
            LoadDetails(selectedTransaction);
        }

        private void LoadDetails(InventoryTransaction selected)
        {
            HeaderTitle.Text = $"Операция: {selected.Type} от {selected.Date:dd.MM.yyyy HH:mm}";

            using (var db = new AppDbContext())
            {
                // Находим все записи, которые были созданы одновременно (в пределах 1 секунды)
                // Это позволяет сгруппировать товары из одной отгрузки/приемки
                var relatedItems = db.InventoryTransactions
                    .Include(t => t.Product)
                    .Where(t => t.Date == selected.Date && t.Type == selected.Type && t.WarehouseId == selected.WarehouseId)
                    .ToList();

                DetailsGrid.ItemsSource = relatedItems;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
