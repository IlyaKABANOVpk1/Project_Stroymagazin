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
                _selectedProduct = db.Products
                    .FirstOrDefault(p => p.SKU.ToLower() == term || p.Name.ToLower().Contains(term));
            }

            if (_selectedProduct != null)
            {
                SelectedProductText.Text = $"{_selectedProduct.Name} (SKU: {_selectedProduct.SKU})";
                PriceBox.Text = _selectedProduct.Price.ToString("N2");
                SelectedProductText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A237E"));
                QuantityBox.Focus();
            }
            else
            {
                SelectedProductText.Text = "❌ Товар не найден.";
                SelectedProductText.Foreground = Brushes.Red;
                PriceBox.Text = "0.00";
                _selectedProduct = null;
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) PerformSearch();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void AddItemToShipment_Click(object sender, RoutedEventArgs e)
        {
         
            if (_selectedProduct == null)
            {
                MessageBox.Show("Сначала найдите и выберите товар.", "Валидация");
                return;
            }

            if (!decimal.TryParse(QuantityBox.Text.Replace(".", ","), out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество (положительное число).", "Валидация");
                return;
            }

            
            if (!decimal.TryParse(PriceBox.Text.Replace(".", ","), out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену.", "Валидация");
                return;
            }

        
            if (WarehouseCombo.SelectedValue is not int warehouseId)
            {
                MessageBox.Show("Выберите склад списания.", "Валидация");
                return;
            }

           
            using (var db = new AppDbContext())
            {
                var stock = db.Stocks.FirstOrDefault(s => s.ProductId == _selectedProduct.Id && s.WarehouseId == warehouseId);

               
                decimal alreadyInList = _shipmentItems
                    .Where(i => i.ProductId == _selectedProduct.Id)
                    .Sum(i => i.Quantity);

                if (stock == null || stock.Quantity < (qty + alreadyInList))
                {
                    MessageBox.Show($"Недостаточно товара. Доступно на складе: {stock?.Quantity ?? 0}. В списке уже: {alreadyInList}", "Ошибка остатков");
                    return;
                }
            }

            _shipmentItems.Add(new ShipmentItem
            {
                ProductId = _selectedProduct.Id,
                Product = _selectedProduct,
                Quantity = qty,
                Price = price
            });

            
            SearchBox.Clear();
            SelectedProductText.Text = "-";
            PriceBox.Text = "0.00";
            QuantityBox.Text = "1";
            _selectedProduct = null;
            SearchBox.Focus();
        }

        private void FinalizeShipment_Click(object sender, RoutedEventArgs e)
        {
            
            if (_shipmentItems.Count == 0)
            {
                MessageBox.Show("Список списания пуст.", "Валидация");
                return;
            }

            
            if (WarehouseCombo.SelectedValue is not int warehouseId)
            {
                MessageBox.Show("Выберите склад для оформления.", "Валидация");
                return;
            }

            
            if (string.IsNullOrWhiteSpace(ReasonBox.Text))
            {
                MessageBox.Show("Укажите причину списания.", "Валидация");
                return;
            }

            using (var db = new AppDbContext())
            {
                foreach (var item in _shipmentItems)
                {
                    // 1. Транзакция (Расход)
                    db.InventoryTransactions.Add(new InventoryTransaction
                    {
                        Date = DateTime.Now,
                        Type = TransactionType.Sale,
                        ProductId = item.ProductId,
                        WarehouseId = warehouseId,
                        Quantity = -item.Quantity, // Отрицательное значение
                        UserId = 1 // Жестко прописано
                    });

                    // 2. Обновление остатка
                    var stock = db.Stocks.FirstOrDefault(s => s.ProductId == item.ProductId && s.WarehouseId == warehouseId);
                    if (stock != null)
                    {
                        stock.Quantity -= item.Quantity;
                    }
                }

                db.SaveChanges();
            }

            MessageBox.Show($"Списание оформлено успешно.", "Успех");
            _shipmentItems.Clear();
            ReasonBox.Clear();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ShipmentItem item)
            {
                _shipmentItems.Remove(item);
            }
        }
    }
}
