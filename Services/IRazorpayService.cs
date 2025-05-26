using BillPaymentBackend.Models;

namespace BillPaymentBackend.Services
{
    public interface IRazorpayService
    {
        Task<RazorpayOrder> CreateOrderAsync(decimal amount);
        bool VerifyPayment(string paymentId, string orderId, string signature);
    }
}

