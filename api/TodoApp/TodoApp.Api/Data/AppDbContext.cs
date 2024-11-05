﻿using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Models;

namespace TodoApp.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Description)
                      .HasMaxLength(1000);

                entity.Property(e => e.IsCompleted)
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
