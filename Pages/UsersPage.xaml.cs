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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Stroymagazin.Pages
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                // Загружаем всех пользователей, чтобы администратор видел и активных, и заблокированных
                // Сортируем по статусу (активные выше) и по имени
                UsersGrid.ItemsSource = db.Users
                    .OrderByDescending(u => u.IsActive)
                    .ThenBy(u => u.FullName)
                    .ToList();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserEditWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int userId)
            {
                // Для пользователей мы не используем слово "Удалить", а используем "Деактивировать"
                var result = MessageBox.Show(
                    "Физическое удаление пользователя невозможно, так как он связан с историей операций.\n\n" +
                    "Деактивировать этого пользователя? Он больше не сможет войти в систему.",
                    "Управление доступом",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find(userId);
                        if (user != null)
                        {
                            // 1. ПРОВЕРКА: Нельзя деактивировать самого себя (если это важно)
                            // Здесь можно добавить проверку через глобальный класс сессии

                            // 2. ВМЕСТО Remove просто меняем флаг
                            user.IsActive = false;

                            db.SaveChanges();
                            LoadData();

                            MessageBox.Show($"Пользователь {user.FullName} деактивирован.", "Успех");
                        }
                    }
                }
            }
        }

        // Дополнительный метод для редактирования (если нужен)
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is User user)
            {
                var dialog = new UserEditWindow(user);
                if (dialog.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }
    }
}
