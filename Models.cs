// Models.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BillPaymentBackend.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UPIId { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed passwords
    }

    public class Transaction
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string BillType { get; set; }

        [Required]
        public string BillNumber { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public string PaymentStatus { get; set; } // e.g., Success, Failed

        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}