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
        private User _user; 

        public UserEditWindow(User user = null)
        {
            InitializeComponent();
            _user = user;

          
            RoleCombo.ItemsSource = Enum.GetValues(typeof(RoleType));
            RoleCombo.SelectedIndex = 0;

            

            if (_user != null)
            {
                FullNameBox.Text = _user.FullName;
                UsernameBox.Text = _user.Username;

              
                PasswordBox.Password = ""; 

                RoleCombo.SelectedItem = _user.Role;
                IsActiveCheck.IsChecked = _user.IsActive;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация пустых полей
            if (string.IsNullOrWhiteSpace(UsernameBox.Text) || string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля: Логин и ФИО!");
                return;
            }

            string newPassword = PasswordBox.Password;

            // Валидация пароля
            if (_user == null) // Создание нового
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    MessageBox.Show("Для нового сотрудника пароль обязателен!");
                    return;
                }
                if (newPassword.Length < 4)
                {
                    MessageBox.Show("Пароль должен быть не короче 4 символов!");
                    return;
                }
            }
            else // Редактирование старого
            {
                if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < 4)
                {
                    MessageBox.Show("Новый пароль должен быть не короче 4 символов!");
                    return;
                }
            }

            using (var db = new AppDbContext())
            {
                if (_user == null)
                {
                    // Проверка на уникальность логина
                    if (db.Users.Any(u => u.Username == UsernameBox.Text))
                    {
                        MessageBox.Show("Этот логин уже занят!");
                        return;
                    }

                    var newUser = new User
                    {
                        FullName = FullNameBox.Text.Trim(),
                        Username = UsernameBox.Text.Trim(),
                        PasswordHash = PasswordHasher.HashPassword(newPassword),
                        Role = (RoleType)RoleCombo.SelectedItem,
                        IsActive = IsActiveCheck.IsChecked ?? true
                    };
                    db.Users.Add(newUser);
                }
                else
                {
                    var userToUpdate = db.Users.Find(_user.Id);
                    if (userToUpdate != null)
                    {
                        userToUpdate.FullName = FullNameBox.Text.Trim();
                        userToUpdate.Username = UsernameBox.Text.Trim();

                        if (!string.IsNullOrWhiteSpace(newPassword))
                        {
                            userToUpdate.PasswordHash = PasswordHasher.HashPassword(newPassword);
                        }

                        userToUpdate.Role = (RoleType)RoleCombo.SelectedItem;
                        userToUpdate.IsActive = IsActiveCheck.IsChecked ?? true;
                    }
                }
                db.SaveChanges();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
