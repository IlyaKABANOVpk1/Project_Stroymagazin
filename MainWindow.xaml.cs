using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Models.Entities.ENUMS;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Project_Stroymagazin.Pages;

namespace Project_Stroymagazin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

           
            UserFullNameText.Text = _currentUser.FullName;

          
            UserRoleText.Text = _currentUser.Role.ToString();

            GenerateMenu();

            MainFrame.Navigate(new Pages.DashboardPage());
            PageTitleText.Text = "Общая статистика";
        }

        private void GenerateMenu()
        {
            MenuPanel.Children.Clear();

            
            CreateMenuButton("Главная статистика", "Home", () =>
            {
                PageTitleText.Text = "Общая статистика";
                MainFrame.Navigate(new DashboardPage());
            });

            
            if (_currentUser.Role == RoleType.Administrator)
            {
                CreateSectionHeader("Настройки системы");

                CreateMenuButton("Сотрудники", "AccountMultiple", () =>
                {
                    PageTitleText.Text = "Управление персоналом";
                    MainFrame.Navigate(new UsersPage());
                });

                CreateMenuButton("Список складов", "Warehouse", () =>
                {
                    PageTitleText.Text = "Адреса и склады";
                    MainFrame.Navigate(new WarehousesPage());
                    // Здесь Админ создает Склад как сущность, но не трогает товар
                });

                CreateSectionHeader("Справочники (Мастер-данные)");

                CreateMenuButton("Каталог товаров", "PackageVariantClosed", () =>
                {
                    PageTitleText.Text = "Редактирование каталога";
                    MainFrame.Navigate(new ProductsPage());
                    // Админ создает карточки товаров (названия, SKU), но не кол-во
                });

                CreateMenuButton("База поставщиков", "Domain", () =>
                {
                    PageTitleText.Text = "Контрагенты";
                    MainFrame.Navigate(new SuppliersPage());
                });
            }

            
            else if (_currentUser.Role == RoleType.WarehouseManager)
            {
                CreateSectionHeader("Операции");

                CreateMenuButton("Приемка товара", "Download", () =>
                {
                    PageTitleText.Text = "Оформление поступления";
                    MainFrame.Navigate(new ArrivalPage());
                });

                CreateMenuButton("Отгрузка / Списание", "Upload", () =>
                {
                    PageTitleText.Text = "Расход товара";
                    MainFrame.Navigate(new ShipmentPage());
                });

                CreateSectionHeader("Складской учет");

                CreateMenuButton("Текущие остатки", "Box", () =>
                {
                    PageTitleText.Text = "Наличие на складах";
                    MainFrame.Navigate(new StockPage());
                });

                CreateMenuButton("История движений", "History", () =>
                {
                    PageTitleText.Text = "Журнал операций";
                    MainFrame.Navigate(new InventoryHistoryPage());
                });

                CreateSectionHeader("Закупки");

                CreateMenuButton("Заказы поставщикам", "Truck", () =>
                {
                    PageTitleText.Text = "Управление заказами";
                    MainFrame.Navigate(new OrdersPage());
                });
            }
        }


        private void CreateMenuButton(string text, string iconName, Action onClick)
        {
            Button btn = new Button
            {
                Content = text,
                Height = 45,
                Margin = new Thickness(0, 2, 0, 2),
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(Color.FromRgb(159, 168, 218)),
                BorderThickness = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(20, 0, 0, 0),
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            btn.Click += (s, e) => onClick();

            
            btn.MouseEnter += (s, e) => btn.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            btn.MouseLeave += (s, e) => btn.Background = Brushes.Transparent;

            MenuPanel.Children.Add(btn);
        }

        private void CreateSectionHeader(string title)
        {
            TextBlock tb = new TextBlock
            {
                Text = title.ToUpper(),
                Foreground = new SolidColorBrush(Color.FromRgb(159, 168, 218)),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(15, 15, 0, 5)
            };
            MenuPanel.Children.Add(tb);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}