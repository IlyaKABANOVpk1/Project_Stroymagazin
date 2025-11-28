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
                    string lowerFilter = filter.ToLower();
                    query = query.Where(s =>
                        s.Name.ToLower().Contains(lowerFilter) ||
                        (s.Phone != null && s.Phone.ToLower().Contains(lowerFilter)) ||
                        (s.Email != null && s.Email.ToLower().Contains(lowerFilter))
                    );
                }

                SuppliersGrid.ItemsSource = query.ToList();
            }
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            new SupplierEditWindow().ShowDialog();
            LoadData(SearchBox.Text);
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Supplier supplier)
            {
                new SupplierEditWindow(supplier).ShowDialog();
                LoadData(SearchBox.Text);
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                if (MessageBox.Show("Удалить этого поставщика?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var supplierToDelete = db.Suppliers.Find(id);
                        if (supplierToDelete != null)
                        {
                            db.Suppliers.Remove(supplierToDelete);
                            db.SaveChanges();
                            LoadData(SearchBox.Text);
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

