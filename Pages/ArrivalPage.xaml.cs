using Microsoft.EntityFrameworkCore;
using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Models.Entities.ENUMS;
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
    /// Логика взаимодействия для ArrivalPage.xaml
    /// </summary>
    public partial class ArrivalPage : Page
    {
        public ArrivalPage()
        {
            InitializeComponent();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            using (var db = new AppDbContext())
            {
                var pendingOrders = db.PurchaseOrders
                    .Include(o => o.Supplier)
                    .Where(o => o.Status == OrderStatus.Pending)
                    .ToList()
                    .Select(o => new { Id = o.Id, DisplayInfo = $"Заказ №{o.Id} от {o.Date.ToShortDateString()} ({o.Supplier.Name})" })
                    .ToList();

                OrderCombo.ItemsSource = pendingOrders;
                WarehouseCombo.ItemsSource = db.Warehouses.ToList();
            }
        }

        private void OrderCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderCombo.SelectedValue is int orderId)
            {
                using (var db = new AppDbContext())
                {
                    ItemsGrid.ItemsSource = db.OrderItems
                        .Include(oi => oi.Product)
                        .Where(oi => oi.OrderId == orderId)
                        .ToList();
                }
            }
            else
            {
                ItemsGrid.ItemsSource = null;
            }
        }

        private void ConfirmArrival_Click(object sender, RoutedEventArgs e)
        {
            if (OrderCombo.SelectedValue is not int orderId || WarehouseCombo.SelectedValue is not int warehouseId)
            {
                MessageBox.Show("Выберите заказ и склад.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var order = db.PurchaseOrders.Find(orderId);
                if (order == null || order.Status != OrderStatus.Received)
                {
                    // Продолжаем выполнение, если статус Pending. Опечатка в логике проверки выше исправлена логикой ниже.
                }

                if (order.Status != OrderStatus.Pending) return;

                var orderItems = db.OrderItems.Where(oi => oi.OrderId == orderId).ToList();

                foreach (var item in orderItems)
                {
                    // 1. Создаем запись о движении для каждого товара отдельно
                    var transaction = new InventoryTransaction
                    {
                        Date = DateTime.Now,
                        Type = TransactionType.Purchase,
                        WarehouseId = warehouseId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity, // Положительное число (приход)
                        UserId = 1
                    };
                    db.InventoryTransactions.Add(transaction);

                    // 2. Обновляем или создаем остаток (Stock)
                    var stock = db.Stocks
                        .FirstOrDefault(s => s.ProductId == item.ProductId && s.WarehouseId == warehouseId);

                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            ProductId = item.ProductId,
                            WarehouseId = warehouseId,
                            Quantity = item.Quantity
                        };
                        db.Stocks.Add(stock);
                    }
                    else
                    {
                        stock.Quantity += item.Quantity;
                    }
                }

                // 3. Закрываем заказ
                order.Status = OrderStatus.Received;

                db.SaveChanges();
            }

            MessageBox.Show("Приемка оформлена успешно!");
            LoadInitialData();
            ItemsGrid.ItemsSource = null;
        }
    }
}

