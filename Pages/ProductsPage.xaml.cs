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
                    string term = SearchBox.Text.ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(term) || p.SKU.ToLower().Contains(term));
                }

                ProductsGrid.ItemsSource = query.ToList();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadData();

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            new ProductEditWindow().ShowDialog();
            LoadData();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Product product)
            {
                new ProductEditWindow(product).ShowDialog();
                LoadData();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var p = db.Products.Find(id);
                        if (p != null)
                        {
                            db.Products.Remove(p);
                            db.SaveChanges();
                            LoadData();
                        }
                    }
                }
            }
        }
    }
}
