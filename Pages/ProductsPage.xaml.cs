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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Stroymagazin.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                var query = db.Products.Include(p => p.Category).AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    string term = SearchBox.Text.Trim().ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(term) || p.SKU.ToLower().Contains(term));
                }

                // Сортируем по названию для удобства
                ProductsGrid.ItemsSource = query.OrderBy(p => p.Name).ToList();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadData();

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductEditWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Product product) // Используем DataContext для надежности
            {
                var dialog = new ProductEditWindow(product);
                if (dialog.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                using (var db = new AppDbContext())
                {
                    // --- БЕЗОПАСНОЕ УДАЛЕНИЕ ---

                    
                    bool hasStock = db.Stocks.Any(s => s.ProductId == id && s.Quantity != 0);

                   
                    bool hasTransactions = db.InventoryTransactions.Any(t => t.ProductId == id);

                  
                    bool hasOrders = db.OrderItems.Any(oi => oi.ProductId == id);

                    if (hasStock || hasTransactions || hasOrders)
                    {
                        MessageBox.Show(
                            "Невозможно удалить товар!\n\n" +
                            "Этот товар уже участвовал в операциях, имеет остатки на складе или числится в заказах. " +
                            "Удаление приведет к нарушению целостности базы данных.\n\n" +
                            "Совет: Переименуйте товар в 'АРХИВ_' или измените его данные.",
                            "Защита данных",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Если проверок нет, спрашиваем подтверждение
                    if (MessageBox.Show("Вы уверены, что хотите безвозвратно удалить этот товар? Это действие нельзя отменить.",
                        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var p = db.Products.Find(id);
                        if (p != null)
                        {
                            db.Products.Remove(p);
                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Товар успешно удален.", "Готово");
                        }
                    }
                }
            }
        }
    }
}
