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
               
                UsersGrid.ItemsSource = db.Users.ToList();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {

            new UserEditWindow().ShowDialog();
            LoadData();
            MessageBox.Show("Функционал добавления будет реализован в следующем шаге (UserEditWindow)", "В разработке");
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
       
            if (sender is Button btn && btn.Tag is int userId)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find(userId);
                        if (user != null)
                        {
                            
                            // db.Users.Remove(user); 
                            user.IsActive = false; 
                            db.SaveChanges();
                            LoadData();
                        }
                    }
                }
            }
        }
    }
}
