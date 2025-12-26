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
            // ВАЛИДАЦИЯ: Выбор заказа и склада
            if (OrderCombo.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ для приемки.", "Валидация");
                return;
            }

            if (WarehouseCombo.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите склад, на который прибудет товар.", "Валидация");
                return;
            }

            int orderId = (int)OrderCombo.SelectedValue;
            int warehouseId = (int)WarehouseCombo.SelectedValue;

            using (var db = new AppDbContext())
            {
                var order = db.PurchaseOrders.Find(orderId);

                // ВАЛИДАЦИЯ: Проверка статуса в БД (защита от повторного нажатия)
                if (order == null || order.Status != OrderStatus.Pending)
                {
                    MessageBox.Show("Этот заказ уже был принят или отменен.", "Ошибка");
                    return;
                }

                var orderItems = db.OrderItems.Where(oi => oi.OrderId == orderId).ToList();

                // ВАЛИДАЦИЯ: Проверка наличия позиций в заказе
                if (!orderItems.Any())
                {
                    MessageBox.Show("В выбранном заказе нет товаров.", "Ошибка");
                    return;
                }

                foreach (var item in orderItems)
                {
                    // 1. Создаем запись о движении
                    var transaction = new InventoryTransaction
                    {
                        Date = DateTime.Now,
                        Type = TransactionType.Purchase,
                        WarehouseId = warehouseId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UserId = 1 // Жестко прописано по требованию
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

            MessageBox.Show("Приемка оформлена успешно!", "Успех");
            LoadInitialData();
            ItemsGrid.ItemsSource = null;
        }
    }
}

