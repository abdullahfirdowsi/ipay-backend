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
    }
}