
using ExpenseTrackerAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ExpenseTrackerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Expense>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Expense>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<User>()
                         .HasMany(e => e.Expenses)
                         .WithOne(e => e.User)
                         .HasForeignKey(e => e.UserId)
                         .IsRequired();

        }
    }
}
