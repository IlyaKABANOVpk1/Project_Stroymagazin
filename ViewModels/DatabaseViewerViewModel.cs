using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Project_Stroymagazin.Models;
using System.Windows;

namespace Project_Stroymagazin.ViewModels
{
    public partial class DatabaseViewerViewModel : ObservableObject
    {
        // Ссылка на контекст БД (замени AppDbContext на имя твоего класса контекста)
        private readonly AppDbContext _context;

        // Свойства для привязки (Binding)
        public ObservableCollection<string> TableNames { get; set; }

        private string _selectedTable;
        public string SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
                LoadTableData(value); // Загружаем данные при выборе
            }
        }

        private object _tableData;
        public object TableData
        {
            get => _tableData;
            set
            {
                _tableData = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public DatabaseViewerViewModel()
        {
            // Инициализация контекста
            _context = new AppDbContext();

            // Заполняем список таблиц (согласно твоему SQL скрипту)
            TableNames = new ObservableCollection<string>
            {
                "Roles",
                "Users",
                "UserRoles",
                "ProductCategories",
                "OrderStatuses",
                "TransactionTypes",
                "Suppliers",
                "Products",
                "PurchaseOrders",
                "OrderItems",
                "StockLevels",
                "InventoryTransactions"
            };

            StatusMessage = "Выберите таблицу для просмотра данных.";
        }

        private void LoadTableData(string tableName)
        {
            try
            {
                StatusMessage = $"Загрузка данных из {tableName}...";

                // Используем AsNoTracking() для режима "только чтение", это быстрее
                switch (tableName)
                {
                    case "Roles":
                        TableData = _context.Roles.AsNoTracking().ToList();
                        break;
                    case "Users":
                        TableData = _context.Users.AsNoTracking().ToList();
                        break;
                    case "UserRoles":
                        TableData = _context.UserRoles.AsNoTracking().ToList();
                        break;
                    case "ProductCategories":
                        TableData = _context.ProductCategories.AsNoTracking().ToList();
                        break;
                    case "OrderStatuses":
                        TableData = _context.OrderStatuses.AsNoTracking().ToList();
                        break;
                    case "TransactionTypes":
                        TableData = _context.TransactionTypes.AsNoTracking().ToList();
                        break;
                    case "Suppliers":
                        TableData = _context.Suppliers.AsNoTracking().ToList();
                        break;
                    case "Products":
                        // Для продуктов можно подгрузить связанные категории и поставщиков через Include, если нужно
                        TableData = _context.Products.AsNoTracking().ToList();
                        break;
                    case "PurchaseOrders":
                        TableData = _context.PurchaseOrders.AsNoTracking().ToList();
                        break;
                    case "OrderItems":
                        TableData = _context.OrderItems.AsNoTracking().ToList();
                        break;
                    case "StockLevels":
                        TableData = _context.StockLevels.AsNoTracking().ToList();
                        break;
                    case "InventoryTransactions":
                        TableData = _context.InventoryTransactions.AsNoTracking().ToList();
                        break;
                    default:
                        TableData = null;
                        break;
                }

                var count = (TableData as System.Collections.ICollection)?.Count ?? 0;
                StatusMessage = $"Загружено записей: {count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                TableData = null;
            }
        }
    }
}
