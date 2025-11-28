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
    /// Логика взаимодействия для ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        private Product _product;

        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();
            _product = product;
            LoadCategories();

            if (_product != null)
            {
                NameBox.Text = _product.Name;
                SKUBox.Text = _product.SKU;
                BarcodeBox.Text = _product.Barcode;
                UnitBox.Text = _product.UnitOfMeasure;
                PriceBox.Text = _product.Price.ToString();

                CategoryCombo.SelectedValue = _product.CategoryId;
            }
        }

        private void LoadCategories()
        {
            using (var db = new AppDbContext())
            {
                CategoryCombo.ItemsSource = db.Categories.ToList();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(SKUBox.Text) || !decimal.TryParse(PriceBox.Text, out decimal price))
            {
                MessageBox.Show("Проверьте заполнение обязательных полей (Название, Артикул, Цена).", "Ошибка валидации");
                return;
            }

            using (var db = new AppDbContext())
            {
                Product entity;
                if (_product == null)
                {
                    entity = new Product();
                    db.Products.Add(entity);
                }
                else
                {
                    entity = db.Products.Find(_product.Id);
                }

                if (entity != null)
                {
                    entity.Name = NameBox.Text;
                    entity.SKU = SKUBox.Text;
                    entity.Barcode = BarcodeBox.Text;
                    entity.UnitOfMeasure = UnitBox.Text;
                    entity.Price = price;
                    entity.CategoryId = (CategoryCombo.SelectedItem as Category)?.Id;

                    db.SaveChanges();
                }
            }
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

