using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace AccountAPI.Entities
{
    public class AccountDbContext : DbContext
    {
        private string _connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=AccountDb9;Trusted_Connection=True;";

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired();

                  modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<User>()
              .Property(u => u.Password)
              .IsRequired();

            modelBuilder.Entity<User>()
              .Property(u => u.Pesel)
              .IsRequired();

            modelBuilder.Entity<User>()
              .Property(u => u.Email)
              .IsRequired();

            modelBuilder.Entity<User>()
              .Property(u => u.PhoneNumber)
              .IsRequired();

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

    }
}

