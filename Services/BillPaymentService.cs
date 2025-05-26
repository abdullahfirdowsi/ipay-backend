using BillPaymentBackend.Models;
using BillPaymentBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace BillPaymentBackend.Services
{
    public class BillPaymentService : IBillPaymentService
    {
        private readonly IBharatBillPayService _bharatBillPayService;
        private readonly IRazorpayService _razorpayService;
        private readonly AppDbContext _context;
        private readonly ILogger<BillPaymentService> _logger;

        public BillPaymentService(
            IBharatBillPayService bharatBillPayService,
            IRazorpayService razorpayService,
            AppDbContext context,
            ILogger<BillPaymentService> logger)
        {
            _bharatBillPayService = bharatBillPayService;
            _razorpayService = razorpayService;
            _context = context;
            _logger = logger;
        }

        public async Task<BillPaymentDetails> FetchBillDetailsAsync(string consumerId, string billType, string provider)
        {
            return await _bharatBillPayService.FetchBillDetailsAsync(consumerId, billType, provider);
        }

        public async Task<RazorpayOrder> InitiatePaymentAsync(PaymentInitiationRequest request)
        {
            var billDetails = await _bharatBillPayService.FetchBillDetailsAsync(
                request.ConsumerId,
                request.BillType,
                request.Provider);

            // Apply ₹5 discount
            var order = await _razorpayService.CreateOrderAsync(billDetails.Amount);

            // Save to database
            _context.RazorpayOrders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> VerifyPaymentAsync(string paymentId, string orderId, string signature)
        {
            if (_razorpayService.VerifyPayment(paymentId, orderId, signature))
            {
                var order = await _context.RazorpayOrders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order != null)
                {
                    // Create transaction record
                    var transaction = new Transaction
                    {
                        RazorpayPaymentId = paymentId,
                        RazorpayOrderId = orderId,
                        RazorpaySignature = signature,
                        Amount = order.OriginalAmount,
                        DiscountAmount = order.DiscountAmount,
                        FinalAmount = order.FinalAmount,
                        PaymentStatus = "Success",
                        TransactionDate = DateTime.Now
                    };

                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ProcessAdminVerificationAsync(int transactionId, string status, string comments)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
                return false;

            var verification = new AdminVerification
            {
                TransactionId = transactionId,
                Status = status,
                AdminComments = comments,
                VerificationDate = DateTime.Now
            };

            transaction.IsAdminVerified = true;
            transaction.AdminVerificationDate = DateTime.Now;

            _context.AdminVerifications.Add(verification);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Transaction>> GetPendingTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.BillPaymentDetails)
                .Include(t => t.RazorpayOrder)
                .Where(t => !t.IsAdminVerified && t.PaymentStatus == "Success")
                .ToListAsync();
        }
    }
}

