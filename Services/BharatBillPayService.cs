using BillPaymentBackend.Models;
using System.Net.Http.Json;

namespace BillPaymentBackend.Services
{
    public class BharatBillPayService : IBharatBillPayService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BharatBillPayService> _logger;

        public BharatBillPayService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<BharatBillPayService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_configuration["BharatBillPay:BaseUrl"]);
            _httpClient.DefaultRequestHeaders.Add("Authorization", 
                $"Bearer {_configuration["BharatBillPay:ApiKey"]}");
        }

        public async Task<BillPaymentDetails> FetchBillDetailsAsync(
            string consumerId, string billType, string provider)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"/api/bills/fetch?consumerId={consumerId}&billType={billType}&provider={provider}");
                
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<BillPaymentDetails>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill details from Bharat BillPay");
                throw;
            }
        }
    }
}

