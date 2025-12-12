using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Models.Entities.ENUMS;
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
    /// Логика взаимодействия для UserEditWindow.xaml
    /// </summary>
    public partial class UserEditWindow : Window
    {
        private User _user; // Если null - создание, иначе редактирование

        public UserEditWindow(User user = null)
        {
            InitializeComponent();
            _user = user;

            // Заполняем ComboBox значениями из Enum
            RoleCombo.ItemsSource = Enum.GetValues(typeof(RoleType));
            RoleCombo.SelectedIndex = 0;

            

            if (_user != null)
            {
                // Режим редактирования: заполняем поля
                FullNameBox.Text = _user.FullName;
                UsernameBox.Text = _user.Username;

                // УДАЛЕНО: PasswordBox.Text = _user.PasswordHash; 
                PasswordBox.Password = ""; // Оставляем поле пустым

                RoleCombo.SelectedItem = _user.Role;
                IsActiveCheck.IsChecked = _user.IsActive;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(UsernameBox.Text) || string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            string newPassword = PasswordBox.Password;

            using (var db = new AppDbContext())
            {
                if (_user == null)
                {
                    // --- РЕЖИМ СОЗДАНИЯ НОВОГО ПОЛЬЗОВАТЕЛЯ ---

                    // В режиме создания пароль обязателен
                    if (string.IsNullOrWhiteSpace(newPassword))
                    {
                        MessageBox.Show("Для нового пользователя пароль обязателен!");
                        return;
                    }

                    // Проверка на уникальность Username (Опционально, но рекомендуется)
                    if (db.Users.Any(u => u.Username == UsernameBox.Text))
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует!");
                        return;
                    }

                    var newUser = new User
                    {
                        FullName = FullNameBox.Text,
                        Username = UsernameBox.Text,
                        // !!! ХЕШИРОВАНИЕ !!!
                        PasswordHash = PasswordHasher.HashPassword(newPassword),
                        Role = (RoleType)RoleCombo.SelectedItem,
                        IsActive = IsActiveCheck.IsChecked ?? true
                    };
                    db.Users.Add(newUser);
                }
                else
                {
                    // --- РЕЖИМ РЕДАКТИРОВАНИЯ ---
                    var userToUpdate = db.Users.Find(_user.Id);

                    if (userToUpdate != null)
                    {
                        userToUpdate.FullName = FullNameBox.Text;
                        userToUpdate.Username = UsernameBox.Text;

                        // !!! ХЕШИРОВАНИЕ (ТОЛЬКО ЕСЛИ ПАРОЛЬ БЫЛ ИЗМЕНЕН) !!!
                        if (!string.IsNullOrWhiteSpace(newPassword))
                        {
                            userToUpdate.PasswordHash = PasswordHasher.HashPassword(newPassword);
                        }
                        // Если поле пароля пустое, старый хеш остается.

                        userToUpdate.Role = (RoleType)RoleCombo.SelectedItem;
                        userToUpdate.IsActive = IsActiveCheck.IsChecked ?? true;
                    }
                }

                db.SaveChanges();
            }

            // Установка DialogResult = true, если окно открыто как Dialog
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
