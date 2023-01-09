using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WashTestTask.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SalesPoint> SalesPoints { get; set; }
        public DbSet<SaleData> SaleDatas { get; set; }
        public DbSet<ProvidedProduct> ProvidedProducts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().Navigation(c => c.Sales).AutoInclude();
            modelBuilder.Entity<Sale>().Navigation(s => s.SalesPoint).AutoInclude();
            modelBuilder.Entity<Sale>().Navigation(s => s.SalesData).AutoInclude();
            modelBuilder.Entity<Sale>().Navigation(s => s.Customer).AutoInclude();
            modelBuilder.Entity<SaleData>().Navigation(sd => sd.Product).AutoInclude();
            modelBuilder.Entity<SalesPoint>().Navigation(sp => sp.ProvidedProducts).AutoInclude();
            modelBuilder.Entity<ProvidedProduct>().Navigation(p => p.Product).AutoInclude();

            modelBuilder.Entity<Customer>().HasData(new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    Name = "Customer 1",
                },
                new Customer
                {
                    Id = 2,
                    Name = "Customer 2"
                }
            });
            modelBuilder.Entity<Product>().HasData(new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Price = 1000,
                    Title = "Product 1"
                },
                new Product
                {
                    Id = 2,
                    Price = 2000,
                    Title = "Product 2"
                }
            });
            modelBuilder.Entity<SalesPoint>().HasData(new List<SalesPoint>
            {
                new SalesPoint
                {
                    Id = 1,
                    Name = "Sales point 1",
                },
                new SalesPoint
                {
                    Id = 2,
                    Name = "Sales point 2",
                }
            });
            modelBuilder.Entity<ProvidedProduct>().HasData(new List<ProvidedProduct>
            {
                new ProvidedProduct
                {
                    Id = 1,
                    ProductId = 1,
                    SalesPointId = 1,
                    Quantity = 10
                },
                new ProvidedProduct
                {
                    Id = 2,
                    ProductId = 2,
                    SalesPointId = 1,
                    Quantity = 0
                },
                new ProvidedProduct
                {
                    Id = 3,
                    ProductId = 2,
                    SalesPointId = 2,
                    Quantity = 5
                }
            });
            modelBuilder.Entity<Sale>().HasData(new List<Sale>
            {
                new Sale
                {
                    Id = 1,
                    Date = DateTimeOffset.Now,
                    CustomerId = 1,
                    SalesPointId = 1
                }
            });
            modelBuilder.Entity<SaleData>().HasData(new List<SaleData>
            {
                new SaleData
                {
                    Id = 1,
                    ProductId = 1,
                    SaleId = 1,
                    ProductQuantity = 3
                },
                new SaleData
                {
                    Id = 2,
                    ProductId = 2,
                    SaleId = 1,
                    ProductQuantity = 5
                }
            });
        }
    }
}