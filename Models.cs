// Models.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BillPaymentBackend.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UPIId { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Store hashed passwords
    }

    public class Transaction
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public string BillType { get; set; } = string.Empty;

        [Required]
        public string BillNumber { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public string PaymentStatus { get; set; } = "Pending"; // e.g., Success, Failed

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        // Add Razorpay-related properties
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpayOrderId { get; set; }
        public string? RazorpaySignature { get; set; }

        // Admin verification properties
        public bool IsAdminVerified { get; set; }
        public DateTime? AdminVerificationDate { get; set; }

        // Navigation properties
        public BillPaymentDetails? BillPaymentDetails { get; set; }
        public RazorpayOrder? RazorpayOrder { get; set; }
        public AdminVerification? AdminVerification { get; set; }
    }

    public class BillPaymentDetails
    {
        public int Id { get; set; }
        
        [Required]
        public string ConsumerId { get; set; } = string.Empty;
        
        [Required]
        public string BillType { get; set; } = string.Empty;  // Electricity, Gas
        
        public string Provider { get; set; } = string.Empty;
        
        public decimal Amount { get; set; }
        
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(15);
        
        public string BillNumber { get; set; } = string.Empty;
        
        public string Status { get; set; } = "Pending";
        
        // Reference to Transaction if payment is made
        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }

    public class PaymentInitiationRequest
    {
        [Required]
        public string ConsumerId { get; set; }
        
        [Required]
        public string BillType { get; set; }
        
        public string Provider { get; set; }
        
        public decimal Amount { get; set; }
        
        public string BillNumber { get; set; }
    }

    public class RazorpayOrder
    {
        public int Id { get; set; }
        
        [Required]
        public string OrderId { get; set; } = string.Empty;  // Razorpay order ID
        
        public decimal OriginalAmount { get; set; }
        
        public decimal DiscountAmount { get; set; } = 5.00M;  // Fixed ₹5 discount
        
        public decimal FinalAmount { get; set; }
        
        public string Status { get; set; } = "Created";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Reference to Transaction
        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }

    public class AdminVerification
    {
        public int Id { get; set; }
        
        public int TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        
        [Required]
        public string Status { get; set; } = "Pending";  // Pending, Verified, Rejected
        
        public string AdminComments { get; set; } = string.Empty;
        
        public DateTime VerificationDate { get; set; } = DateTime.Now;
    }

    public class AdminVerificationRequest
    {
        [Required]
        public string Status { get; set; }  // "Verified" or "Rejected"

        public string Comments { get; set; }
    }
}
