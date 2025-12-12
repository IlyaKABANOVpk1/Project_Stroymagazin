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

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
               
                optionsBuilder.UseSqlServer("Server=HULK;Database=ProjectStroymagazinDB;Trusted_Connection=True;TrustServerCertificate=True");
            }
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


            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.PurchasePrice)
                .HasPrecision(18, 2);

            
            modelBuilder.Entity<InventoryTransaction>()
                .Property(t => t.Quantity)
                .HasPrecision(18, 4);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Quantity)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Stock>()
                .Property(s => s.Quantity)
                .HasPrecision(18, 4);
        }
    }
}
