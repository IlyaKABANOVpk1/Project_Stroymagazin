using Project_Stroymagazin.Models;
using Project_Stroymagazin.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для SupplierEditWindow.xaml
    /// </summary>
    public partial class SupplierEditWindow : Window
    {
        private Supplier _supplier;

        public SupplierEditWindow(Supplier supplier = null)
        {
            InitializeComponent();
            _supplier = supplier;

            if (_supplier != null)
            {
                NameBox.Text = _supplier.Name;
                PhoneBox.Text = _supplier.Phone;
                EmailBox.Text = _supplier.Email;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Валидация названия
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название компании.", "Ошибка валидации");
                return;
            }

            // 2. Валидация Email (если введен)
            if (!string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(EmailBox.Text, emailPattern))
                {
                    MessageBox.Show("Введите корректный Email (например: info@company.com).", "Ошибка валидации");
                    return;
                }
            }

            // 3. Валидация телефона (простая проверка на длину, можно усложнить)
            if (string.IsNullOrWhiteSpace(PhoneBox.Text) || PhoneBox.Text.Length < 6)
            {
                MessageBox.Show("Введите корректный номер телефона (минимум 6 цифр).", "Ошибка валидации");
                return;
            }

            using (var db = new AppDbContext())
            {
                Supplier entity;
                if (_supplier == null)
                {
                    entity = new Supplier();
                    db.Suppliers.Add(entity);
                }
                else
                {
                    entity = db.Suppliers.Find(_supplier.Id);
                }

                if (entity != null)
                {
                    entity.Name = NameBox.Text.Trim();
                    entity.Phone = PhoneBox.Text.Trim();
                    entity.Email = EmailBox.Text.Trim().ToLower(); // Почту лучше хранить в нижнем регистре

                    db.SaveChanges();
                    this.DialogResult = true; // Чтобы список обновился
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

