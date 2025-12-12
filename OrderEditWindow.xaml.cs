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
using System.Windows.Shapes;

namespace Project_Stroymagazin
{
    /// <summary>
    /// Логика взаимодействия для OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
        private ObservableCollection<OrderItem> _items = new ObservableCollection<OrderItem>();
        private User _currentUser;

        public OrderEditWindow()
        {
            InitializeComponent();
            ItemsGrid.ItemsSource = _items;
            LoadData();

          
            ManagerNameText.Text = "Администратор";
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                SupplierCombo.ItemsSource = db.Suppliers.ToList();
                ProductCombo.ItemsSource = db.Products.ToList();
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProductCombo.SelectedItem is Product product &&
                decimal.TryParse(QuantityBox.Text, out decimal qty) &&
                decimal.TryParse(PriceBox.Text, out decimal price))
            {
                var item = new OrderItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = qty,
                    PurchasePrice = price
                };

                _items.Add(item);
                UpdateTotal();

                QuantityBox.Clear();
                PriceBox.Clear();
            }
            else
            {
                MessageBox.Show("Проверьте данные товара.");
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderItem item)
            {
                _items.Remove(item);
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            decimal total = _items.Sum(i => i.Quantity * i.PurchasePrice);
            TotalAmountText.Text = $"{total:C2}";
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierCombo.SelectedItem is not Supplier supplier)
            {
                MessageBox.Show("Выберите поставщика!");
                return;
            }

            if (_items.Count == 0)
            {
                MessageBox.Show("Добавьте товары в заказ.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var order = new PurchaseOrder
                {
                    Date = DateTime.Now,
                    Status = OrderStatus.Pending,
                    SupplierId = supplier.Id,
                    ManagerId = 1 
                };

                db.PurchaseOrders.Add(order);
                db.SaveChanges(); 

                foreach (var item in _items)
                {
                    item.OrderId = order.Id;
                    item.Product = null;
                    db.OrderItems.Add(item);
                }

                db.SaveChanges();
            }

            this.Close();
        }
    }
}
