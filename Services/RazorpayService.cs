using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using BillPaymentBackend.Models;

namespace BillPaymentBackend.Services
{
    public class RazorpayService : IRazorpayService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RazorpayService> _logger;
        private readonly string _keyId;
        private readonly string _keySecret;

        public RazorpayService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<RazorpayService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _keyId = configuration["Razorpay:KeyId"] ?? 
                throw new InvalidOperationException("Razorpay:KeyId is not configured");
            _keySecret = configuration["Razorpay:KeySecret"] ?? 
                throw new InvalidOperationException("Razorpay:KeySecret is not configured");

            // Configure HttpClient for Razorpay API
            _httpClient.BaseAddress = new Uri("https://api.razorpay.com/v1/");
            var authString = $"{_keyId}:{_keySecret}";
            var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);
        }

        public async Task<RazorpayOrder> CreateOrderAsync(decimal amount)
        {
            try
            {
                var discountAmount = 5.00M;
                var finalAmount = amount - discountAmount;

                // Razorpay expects amount in smallest currency unit (paise)
                var amountInPaise = (int)(finalAmount * 100);

                var orderRequest = new
                {
                    amount = amountInPaise,
                    currency = "INR",
                    payment_capture = 1
                };

                var response = await _httpClient.PostAsJsonAsync("orders", orderRequest);
                response.EnsureSuccessStatusCode();

                var orderResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                var orderId = orderResponse.GetProperty("id").GetString();

                return new RazorpayOrder
                {
                    OrderId = orderId,
                    OriginalAmount = amount,
                    DiscountAmount = discountAmount,
                    FinalAmount = finalAmount,
                    Status = "Created",
                    CreatedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay order");
                throw;
            }
        }

        public bool VerifyPayment(string paymentId, string orderId, string signature)
        {
            try
            {
                // Generate signature
                var text = $"{orderId}|{paymentId}";
                var key = Encoding.UTF8.GetBytes(_keySecret);
                var payload = Encoding.UTF8.GetBytes(text);

                using var hmac = new HMACSHA256(key);
                var computedHash = hmac.ComputeHash(payload);
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

                return signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment signature verification failed");
                return false;
            }
        }
    }
}
