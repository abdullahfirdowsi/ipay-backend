// AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using BillPaymentBackend.Models;
using System.Collections.Generic;

namespace BillPaymentBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BillPaymentDetails> BillPaymentDetails { get; set; }
        public DbSet<RazorpayOrder> RazorpayOrders { get; set; }
        public DbSet<AdminVerification> AdminVerifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.BillPaymentDetails)
                .WithOne(b => b.Transaction)
                .HasForeignKey<BillPaymentDetails>(b => b.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.RazorpayOrder)
                .WithOne(r => r.Transaction)
                .HasForeignKey<RazorpayOrder>(r => r.TransactionId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.AdminVerification)
                .WithOne(a => a.Transaction)
                .HasForeignKey<AdminVerification>(a => a.TransactionId);

            // Configure indexes
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.RazorpayPaymentId)
                .IsUnique()
                .HasFilter("[RazorpayPaymentId] IS NOT NULL");

            modelBuilder.Entity<RazorpayOrder>()
                .HasIndex(r => r.OrderId)
                .IsUnique();
            
            // Configure decimal precision for BillPaymentDetails
            modelBuilder.Entity<BillPaymentDetails>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            // Configure decimal precision for RazorpayOrder
            modelBuilder.Entity<RazorpayOrder>()
                .Property(r => r.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RazorpayOrder>()
                .Property(r => r.FinalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RazorpayOrder>()
                .Property(r => r.OriginalAmount)
                .HasColumnType("decimal(18,2)");

            // Configure decimal precision for Transaction
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.FinalAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}