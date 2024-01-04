using DatabaseLibrary.DatabaseConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Models;

namespace TheSuiteSpot.HotelDatabase.DatabaseConfiguration
{
    public class HotelContext : DbContext
    {
        public HotelContext()
        {

        }

        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {

        }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<SystemMessageType> MessageType { get; set; }
        public DbSet<SystemMessage> Message { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserInbox> UserInbox { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Voucher> Voucher { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => ur.RoleName)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasColumnType("decimal(11,2)"); // this means 10 precision points, 8 to the left 2 to the right

            modelBuilder.Entity<Room>()
                .Property(r => r.PricePerExtraBed)
                .HasColumnType("decimal(7,2)");

            modelBuilder.Entity<Room>()
                .Property(r => r.PricePerDay)
                .HasColumnType("decimal(9,2)");

            modelBuilder.Entity<Voucher>()
                .Property(uv => uv.DiscountPercentage)
                .HasColumnType("decimal(4,2)");

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Build.GetConnectionString());
        }
    }
}
