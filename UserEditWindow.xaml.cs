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
                PasswordBox.Text = _user.PasswordHash;
                RoleCombo.SelectedItem = _user.Role;
                IsActiveCheck.IsChecked = _user.IsActive;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text) || string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            using (var db = new AppDbContext())
            {
                if (_user == null)
                {
                    // Создание нового
                    var newUser = new User
                    {
                        FullName = FullNameBox.Text,
                        Username = UsernameBox.Text,
                        PasswordHash = PasswordBox.Text, // В реальности хешируем
                        Role = (RoleType)RoleCombo.SelectedItem,
                        IsActive = IsActiveCheck.IsChecked ?? true
                    };
                    db.Users.Add(newUser);
                }
                else
                {
                    // Обновление существующего (нужно приатачить к контексту)
                    var userToUpdate = db.Users.Find(_user.Id);
                    if (userToUpdate != null)
                    {
                        userToUpdate.FullName = FullNameBox.Text;
                        userToUpdate.Username = UsernameBox.Text;
                        userToUpdate.PasswordHash = PasswordBox.Text;
                        userToUpdate.Role = (RoleType)RoleCombo.SelectedItem;
                        userToUpdate.IsActive = IsActiveCheck.IsChecked ?? true;
                    }
                }

                db.SaveChanges();
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
