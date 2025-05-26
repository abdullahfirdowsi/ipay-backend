using BillPaymentBackend.Models;

namespace BillPaymentBackend.Services
{
    public interface IBillPaymentService
    {
        Task<BillPaymentDetails> FetchBillDetailsAsync(string consumerId, string billType, string provider);
        Task<RazorpayOrder> InitiatePaymentAsync(PaymentInitiationRequest request);
        Task<bool> VerifyPaymentAsync(string paymentId, string orderId, string signature);
        Task<bool> ProcessAdminVerificationAsync(int transactionId, string status, string comments);
        Task<IEnumerable<Transaction>> GetPendingTransactionsAsync();
    }
}

