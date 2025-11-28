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
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название компании.", "Ошибка");
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
                    entity.Name = NameBox.Text;
                    entity.Phone = PhoneBox.Text;
                    entity.Email = EmailBox.Text;

                    db.SaveChanges();
                }
            }
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

