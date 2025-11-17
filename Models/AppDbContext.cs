using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_Stroymagazin.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Stroymagazin.Models
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext()
        {
        }

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<TransactionType> TransactionTypes => Set<TransactionType>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<StockLevel> StockLevels => Set<StockLevel>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration?.GetConnectionString("DefaultConnection")
                    ?? "Server=(localdb)\\MSSQLLocalDB;Database=Stroylandia;Trusted_Connection=True;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserRole — составной ключ
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // StockLevel — PK = ProductId
            modelBuilder.Entity<StockLevel>()
                .HasKey(sl => sl.ProductId);

            // Уникальные поля
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name).IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU).IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.INN).IsUnique()
                .HasFilter("[INN] IS NOT NULL");

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.KPP).IsUnique()
                .HasFilter("[KPP] IS NOT NULL");

            // Связи
            modelBuilder.Entity<StockLevel>()
                .HasOne(sl => sl.Product)
                .WithOne(p => p.StockLevel)
                .HasForeignKey<StockLevel>(sl => sl.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
