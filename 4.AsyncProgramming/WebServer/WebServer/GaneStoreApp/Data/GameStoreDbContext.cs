﻿namespace WebServer.GaneStoreApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using WebServer.GaneStoreApp.Data.Models;

    public class GameStoreDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder
                .UseSqlServer("Server=.;Database=GameStoreDb;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserGame>()
                .HasKey(ug => new { ug.GameId, ug.UserId });

            builder
                .Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
                .HasMany(u => u.Games)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId);

            builder.Entity<Game>()
                .HasMany(g => g.Users)
                .WithOne(u => u.Game)
                .HasForeignKey(u => u.GameId);
        }
    }
}
