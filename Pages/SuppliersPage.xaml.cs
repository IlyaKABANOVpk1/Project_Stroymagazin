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
    /// Логика взаимодействия для SuppliersPage.xaml
    /// </summary>
    public partial class SuppliersPage : Page
    {
        public SuppliersPage()
        {
            InitializeComponent();
            LoadData(null);
        }

        private void LoadData(string filter)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Suppliers.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    string lowerFilter = filter.Trim().ToLower(); // Добавили Trim()
                    query = query.Where(s =>
                        s.Name.ToLower().Contains(lowerFilter) ||
                        (s.Phone != null && s.Phone.ToLower().Contains(lowerFilter)) ||
                        (s.Email != null && s.Email.ToLower().Contains(lowerFilter))
                    );
                }

                SuppliersGrid.ItemsSource = query.OrderBy(s => s.Name).ToList();
            }
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SupplierEditWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadData(SearchBox.Text);
            }
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            // Используем DataContext для получения объекта из строки
            if (sender is Button btn && btn.DataContext is Supplier supplier)
            {
                var dialog = new SupplierEditWindow(supplier);
                if (dialog.ShowDialog() == true)
                {
                    LoadData(SearchBox.Text);
                }
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                using (var db = new AppDbContext())
                {
                    // --- БЕЗОПАСНОЕ УДАЛЕНИЕ ---

                    // 1. Проверяем, есть ли у поставщика заказы (PurchaseOrders)
                    // Если в базе есть хотя бы один заказ, связанный с этим ID, удалять нельзя
                    bool hasOrders = db.PurchaseOrders.Any(o => o.SupplierId == id);

                    if (hasOrders)
                    {
                        MessageBox.Show(
                            "Невозможно удалить поставщика!\n\n" +
                            "С этим поставщиком связаны документы закупа (заказы). " +
                            "Удаление приведет к потере истории закупок.\n\n" +
                            "Рекомендация: если вы больше не работаете с ним, просто не создавайте новые заказы.",
                            "Защита целостности данных",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // 2. Если заказов нет, запрашиваем подтверждение
                    if (MessageBox.Show("Вы уверены, что хотите удалить этого поставщика? Это действие нельзя отменить.",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var supplierToDelete = db.Suppliers.Find(id);
                        if (supplierToDelete != null)
                        {
                            db.Suppliers.Remove(supplierToDelete);
                            db.SaveChanges();
                            LoadData(SearchBox.Text);
                            MessageBox.Show("Поставщик успешно удален.");
                        }
                    }
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(SearchBox.Text);
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            LoadData(SearchBox.Text);
        }
    }
}

