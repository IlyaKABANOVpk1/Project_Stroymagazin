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
                PriceBox.Text = _product.Price.ToString("F2"); // Формат с 2 знаками после запятой

                CategoryCombo.SelectedValue = _product.CategoryId;
            }
        }

        private void LoadCategories()
        {
            using (var db = new AppDbContext())
            {
                // Загружаем список категорий для выпадающего списка
                CategoryCombo.ItemsSource = db.Categories.ToList();
                CategoryCombo.DisplayMemberPath = "Name";
                CategoryCombo.SelectedValuePath = "Id";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Валидация пустых текстовых полей
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название товара!", "Валидация");
                return;
            }

            if (string.IsNullOrWhiteSpace(SKUBox.Text))
            {
                MessageBox.Show("Введите артикул (SKU)!", "Валидация");
                return;
            }

            // 2. Валидация цены (замена точки на запятую для корректного парсинга)
            string priceInput = PriceBox.Text.Replace(".", ",");
            if (!decimal.TryParse(priceInput, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)!", "Валидация");
                return;
            }

            // 3. Проверка выбора категории
            if (CategoryCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию товара!", "Валидация");
                return;
            }

            using (var db = new AppDbContext())
            {
                Product entity;
                if (_product == null) // Создание нового товара
                {
                    // Проверка на уникальность SKU (артикула)
                    if (db.Products.Any(p => p.SKU == SKUBox.Text.Trim()))
                    {
                        MessageBox.Show("Товар с таким артикулом уже существует!", "Ошибка");
                        return;
                    }

                    entity = new Product();
                    db.Products.Add(entity);
                }
                else // Редактирование существующего
                {
                    entity = db.Products.Find(_product.Id);
                }

                if (entity != null)
                {
                    // Заполняем данные с обрезкой лишних пробелов (Trim)
                    entity.Name = NameBox.Text.Trim();
                    entity.SKU = SKUBox.Text.Trim();
                    entity.Barcode = BarcodeBox.Text?.Trim();
                    entity.UnitOfMeasure = UnitBox.Text?.Trim();
                    entity.Price = price;
                    entity.CategoryId = (int)CategoryCombo.SelectedValue;

                    try
                    {
                        db.SaveChanges();
                        this.DialogResult = true; // Сообщаем списку, что данные обновились
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка БД");
                    }
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

