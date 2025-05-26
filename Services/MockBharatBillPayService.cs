using BillPaymentBackend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BillPaymentBackend.Services
{
    public class MockBharatBillPayService : IBharatBillPayService
    {
        private readonly ILogger<MockBharatBillPayService> _logger;
        private readonly string _baseUrl;

        public MockBharatBillPayService(
            IConfiguration configuration,
            ILogger<MockBharatBillPayService> logger)
        {
            _logger = logger;
            _baseUrl = configuration["BharatBillPay:BaseUrl"] ?? 
                throw new InvalidOperationException("BharatBillPay:BaseUrl is not configured");
        }

        public async Task<BillPaymentDetails> FetchBillDetailsAsync(
            string consumerId, string billType, string provider)
        {
            // Simulate API delay
            await Task.Delay(1000);

            // Mock response based on bill type
            var billDetails = new BillPaymentDetails
            {
                ConsumerId = consumerId,
                BillType = billType,
                Provider = provider,
                Amount = GetMockAmount(billType),
                DueDate = DateTime.Now.AddDays(15),
                BillNumber = GenerateMockBillNumber(),
                Status = "Pending"
            };

            _logger.LogInformation($"Generated mock bill details for consumer {consumerId}");
            return billDetails;
        }

        private decimal GetMockAmount(string billType)
        {
            // Return realistic mock amounts based on bill type
            return billType.ToLower() switch
            {
                "electricity" => Random.Shared.Next(500, 3000),
                "gas" => Random.Shared.Next(300, 1000),
                "water" => Random.Shared.Next(200, 800),
                _ => Random.Shared.Next(100, 1000)
            };
        }

        private string GenerateMockBillNumber()
        {
            return $"BILL{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
        }
    }
}
