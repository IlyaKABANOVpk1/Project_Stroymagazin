using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Stroymagazin.Models.Entities;
using Project_Stroymagazin.Models.Entities.ENUMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        public AppDbContext()
        {
            
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=StroyMagazinV2;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

          
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<int>();

         
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "admin",
                    FullName = "Системный Администратор",
                    Role = RoleType.Administrator,
                    IsActive = true
                }
            );

           
            modelBuilder.Entity<Warehouse>().HasData(
                new Warehouse { Id = 1, Name = "Центральный склад", Address = "ул. Ленина 1" }
            );
        }
    }
}
