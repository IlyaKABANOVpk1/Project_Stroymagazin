using Project_Stroymagazin.Models;
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
using Microsoft.EntityFrameworkCore;

namespace Project_Stroymagazin.Pages
{
    /// <summary>
    /// Логика взаимодействия для StockPage.xaml
    /// </summary>
    public partial class StockPage : Page
    {
        public StockPage()
        {
            InitializeComponent();
            LoadWarehouses();
            LoadStocks();
        }

        private void LoadWarehouses()
        {
            using (var db = new AppDbContext())
            {
                var warehouses = db.Warehouses.ToList();
                // Добавляем "Все склады" как фиктивный элемент (опционально),
                // но для простоты пока просто загрузим список.
                WarehouseCombo.ItemsSource = warehouses;

                // Выбираем первый по умолчанию, если есть
                if (warehouses.Any())
                    WarehouseCombo.SelectedIndex = 0;
            }
        }

        private void LoadStocks()
        {
            using (var db = new AppDbContext())
            {
                // Начинаем формировать запрос
                // Нам нужно "подтянуть" (Include) данные о Продукте, его Категории и Складе,
                // так как в таблице Stock хранятся только ID.
                var query = db.Stocks
                    .Include(s => s.Product)
                    .ThenInclude(p => p.Category)
                    .Include(s => s.Warehouse)
                    .AsQueryable();

                // 1. Фильтр по складу (если выбран)
                if (WarehouseCombo.SelectedValue is int warehouseId)
                {
                    query = query.Where(s => s.WarehouseId == warehouseId);
                }

                // 2. Фильтр по поиску (по имени товара или SKU)
                string search = SearchBox.Text.ToLower();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(s =>
                        s.Product.Name.ToLower().Contains(search) ||
                        s.Product.SKU.ToLower().Contains(search));
                }

                var result = query.ToList();
                StockGrid.ItemsSource = result;

                TotalStatsText.Text = $"Всего позиций: {result.Count} | Общий остаток: {result.Sum(x => x.Quantity):N0} ед.";
            }
        }

        private void WarehouseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStocks();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadStocks();
        }
    }
}
