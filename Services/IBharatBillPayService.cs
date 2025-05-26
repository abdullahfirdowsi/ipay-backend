using BillPaymentBackend.Models;

namespace BillPaymentBackend.Services
{
    public interface IBharatBillPayService
    {
        Task<BillPaymentDetails> FetchBillDetailsAsync(string consumerId, string billType, string provider);
    }
}

