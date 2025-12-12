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

            
            CreateMenuButton("Главная", "Home", () =>
            {
                PageTitleText.Text = "Общая статистика";
                MainFrame.Navigate(new DashboardPage());
            });

      
            if (_currentUser.Role == RoleType.Administrator)
            {
                CreateSectionHeader("Администрирование");

                CreateMenuButton("Сотрудники", "AccountMultiple", () =>
                {
                    PageTitleText.Text = "Управление персоналом";
                    MainFrame.Navigate(new UsersPage());
                });

                CreateMenuButton("Склады", "Warehouse", () =>
                {
                    PageTitleText.Text = "Управление складами";
                    MainFrame.Navigate(new WarehousesPage());
                });

               
                CreateMenuButton("Каталог товаров", "PackageVariantClosed", () =>
                {
                    PageTitleText.Text = "Управление каталогом";
                    MainFrame.Navigate(new ProductsPage());
                });
            }

           
            if (_currentUser.Role == RoleType.PurchaseManager || _currentUser.Role == RoleType.Administrator)
            {
                CreateSectionHeader("Закупки");

             
                CreateMenuButton("Заказы поставщикам", "Truck", () =>
                {
                    PageTitleText.Text = "Заказы";
                    MainFrame.Navigate(new OrdersPage());
                });

                CreateMenuButton("Поставщики", "Domain", () =>
                {
                    PageTitleText.Text = "База поставщиков";
                    MainFrame.Navigate(new SuppliersPage());
                });
            }

            
            if (_currentUser.Role == RoleType.WarehouseManager || _currentUser.Role == RoleType.Administrator)
            {
                CreateSectionHeader("Склад и логистика");

                CreateMenuButton("Остатки товара", "Box", () =>
                {
                    PageTitleText.Text = "Текущие остатки";
                    MainFrame.Navigate(new StockPage());
                });

              
                CreateMenuButton("Приемка товара", "Download", () =>
                {
                    PageTitleText.Text = "Оформление прихода";
                    MainFrame.Navigate(new ArrivalPage());
                });

               
                CreateMenuButton("Отгрузка/Списание", "Upload", () =>
                {
                    PageTitleText.Text = "Оформление расхода";
                    MainFrame.Navigate(new ShipmentPage());
                });

               
                CreateMenuButton("История операций", "History", () =>
                {
                    PageTitleText.Text = "Движение товаров";
                    MainFrame.Navigate(new InventoryHistoryPage());
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