﻿using Microsoft.EntityFrameworkCore;
using WebServer.ByTheCakeApp.Data.Models;

namespace WebServer.ByTheCakeApp.Data
{
    public class CakeDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("Server=.;Database=ByTheCakeDb;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            builder
                .Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            builder
                .Entity<Product>()
                .HasMany(p => p.Orders)
                .WithOne(op => op.Product)
                .HasForeignKey(op => op.ProductId);

            builder
                .Entity<Order>()
                .HasMany(o => o.Products)
                .WithOne(op => op.Order)
                .HasForeignKey(op => op.OrderId);

        }
    }
}
