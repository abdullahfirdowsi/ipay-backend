using BillPaymentBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillPaymentBackend.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // Ensure database is created and apply migrations
                await context.Database.MigrateAsync();

                // Check if we already have users
                if (await context.Users.AnyAsync())
                {
                    return; // DB has been seeded
                }

                // Add test users
                var users = new[]
                {
                    new User
                    {
                        Name = "Test User",
                        Email = "user@test.com",
                        UPIId = "user@bharatpay",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
                    },
                    new User
                    {
                        Name = "Admin User",
                        Email = "admin@test.com",
                        UPIId = "admin@bharatpay",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                // Add sample transactions
                var transactions = new[]
                {
                    new Transaction
                    {
                        UserId = users[0].UserId,
                        BillType = "Electricity",
                        BillNumber = "BILL202505001",
                        Amount = 1500.00M,
                        DiscountAmount = 5.00M,
                        FinalAmount = 1495.00M,
                        PaymentStatus = "Success",
                        TransactionDate = DateTime.Now.AddDays(-5),
                        IsAdminVerified = false
                    },
                    new Transaction
                    {
                        UserId = users[0].UserId,
                        BillType = "Gas",
                        BillNumber = "BILL202505002",
                        Amount = 800.00M,
                        DiscountAmount = 5.00M,
                        FinalAmount = 795.00M,
                        PaymentStatus = "Pending",
                        TransactionDate = DateTime.Now.AddDays(-2),
                        IsAdminVerified = false
                    }
                };

                await context.Transactions.AddRangeAsync(transactions);

                // Add sample bill payment details
                var billPaymentDetails = new[]
                {
                    new BillPaymentDetails
                    {
                        ConsumerId = "ELEC001",
                        BillType = "Electricity",
                        Provider = "State Electricity Board",
                        Amount = 1500.00M,
                        DueDate = DateTime.Now.AddDays(10),
                        BillNumber = "BILL202505001",
                        Status = "Pending",
                        TransactionId = 1
                    },
                    new BillPaymentDetails
                    {
                        ConsumerId = "GAS001",
                        BillType = "Gas",
                        Provider = "City Gas Distribution",
                        Amount = 800.00M,
                        DueDate = DateTime.Now.AddDays(15),
                        BillNumber = "BILL202505002",
                        Status = "Pending",
                        TransactionId = 2
                    }
                };

                await context.BillPaymentDetails.AddRangeAsync(billPaymentDetails);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}

