using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Models.Entities.ENUMS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ShipmentPage.xaml
    /// </summary>
    public class ShipmentItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public partial class ShipmentPage : Page
    {
        private ObservableCollection<ShipmentItem> _shipmentItems = new ObservableCollection<ShipmentItem>();
        private Product _selectedProduct;

        public ShipmentPage()
        {
            InitializeComponent();
            ShipmentGrid.ItemsSource = _shipmentItems;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            using (var db = new AppDbContext())
            {
                WarehouseCombo.ItemsSource = db.Warehouses.ToList();
            }
        }

        private void PerformSearch()
        {
            string term = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(term)) return;

            using (var db = new AppDbContext())
            {
                // Ищем по точному совпадению SKU или частичному совпадению имени
                _selectedProduct = db.Products
                    .FirstOrDefault(p => p.SKU.ToLower() == term || p.Name.ToLower().Contains(term));
            }

            if (_selectedProduct != null)
            {
                SelectedProductText.Text = $"{_selectedProduct.Name} (SKU: {_selectedProduct.SKU})";
                // Подставляем цену продажи по умолчанию, но можно менять
                PriceBox.Text = _selectedProduct.Price.ToString("N2");
                SelectedProductText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A237E")); // Синий цвет
                QuantityBox.Focus();
            }
            else
            {
                SelectedProductText.Text = "❌ Товар не найден. Проверьте SKU или название.";
                SelectedProductText.Foreground = Brushes.Red;
                PriceBox.Text = "0.00";
                _selectedProduct = null;
            }
        }

        // Обнови существующий метод
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
        }

        // Добавь новый метод для кнопки
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void AddItemToShipment_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null || !decimal.TryParse(QuantityBox.Text, out decimal qty) || !decimal.TryParse(PriceBox.Text, out decimal price) || qty <= 0)
            {
                MessageBox.Show("Выберите товар и введите корректное количество/цену.");
                return;
            }

            // Проверка наличия на выбранном складе
            if (WarehouseCombo.SelectedValue is not int warehouseId)
            {
                MessageBox.Show("Выберите склад списания.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var stock = db.Stocks.FirstOrDefault(s => s.ProductId == _selectedProduct.Id && s.WarehouseId == warehouseId);
                if (stock == null || stock.Quantity < qty)
                {
                    MessageBox.Show($"На складе {warehouseId} недостаточно товара. Доступно: {stock?.Quantity ?? 0}");
                    return;
                }
            }

            // Добавление в локальную коллекцию
            var newItem = new ShipmentItem
            {
                ProductId = _selectedProduct.Id,
                Product = _selectedProduct,
                Quantity = qty,
                Price = price
            };

            _shipmentItems.Add(newItem);

            ShipmentGrid.Items.Refresh();
            SearchBox.Clear();
            SelectedProductText.Text = "-";
            PriceBox.Text = "0.00";
            QuantityBox.Text = "1";
            _selectedProduct = null;
            SearchBox.Focus();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ShipmentItem item)
            {
                _shipmentItems.Remove(item);
            }
        }

        private void FinalizeShipment_Click(object sender, RoutedEventArgs e)
        {
            if (_shipmentItems.Count == 0)
            {
                MessageBox.Show("Список списания пуст.");
                return;
            }

            if (WarehouseCombo.SelectedValue is not int warehouseId)
            {
                MessageBox.Show("Выберите склад для оформления списания.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ReasonBox.Text))
            {
                MessageBox.Show("Укажите причину списания (Отгрузка, Брак и т.п.).");
                return;
            }

            using (var db = new AppDbContext())
            {
                foreach (var item in _shipmentItems)
                {
                    // 1. Создание записи о движении (списание)
                    var transaction = new InventoryTransaction
                    {
                        Date = DateTime.Now,
                        Type = TransactionType.Sale, // Используем Sale как общее списание/отгрузку
                        ProductId = item.ProductId,
                        WarehouseId = warehouseId,
                        Quantity = -item.Quantity, // Отрицательное число (расход)
                        UserId = 1, // Текущий пользователь
                       
                    };
                    db.InventoryTransactions.Add(transaction);

                    // 2. Списание остатка (Stock)
                    var stock = db.Stocks
                        .FirstOrDefault(s => s.ProductId == item.ProductId && s.WarehouseId == warehouseId);

                    if (stock != null)
                    {
                        stock.Quantity -= item.Quantity;
                    }
                }

                db.SaveChanges();
            }

            MessageBox.Show($"Документ списания оформлен успешно. Списано {ShipmentGrid.Items.Count} позиций.");
            _shipmentItems.Clear();
            ReasonBox.Clear();
        }
    }
}
