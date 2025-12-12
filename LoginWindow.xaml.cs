using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Utils;
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
using System.Windows.Shapes;

namespace Project_Stroymagazin
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public User LoggedInUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginBox.Text.Trim();
            string password = PassBox.Password;

            // 1. Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Введите имя пользователя и пароль.";
                return;
            }

            using (var db = new AppDbContext())
            {
                // Ищем пользователя только по имени, чтобы получить его хеш
                var user = db.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    // 2. Использование PasswordHasher для сравнения введенного пароля с хешем
                    if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
                    {
                        if (!user.IsActive)
                        {
                            ErrorText.Text = "Доступ запрещен. Пользователь заблокирован.";
                            return;
                        }

                        // Вход успешен:
                        LoggedInUser = user;

                        // Передаем пользователя в главное окно
                        MainWindow main = new MainWindow(user);
                        main.Show();

                        this.Close();
                    }
                    else
                    {
                        // Пароль неверен
                        ErrorText.Text = "Неверное имя пользователя или пароль.";
                    }
                }
                else
                {
                    // Пользователь не найден
                    ErrorText.Text = "Неверное имя пользователя или пароль.";
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
