using Microsoft.AspNetCore.Mvc;
using BillPaymentBackend.Models;
using BillPaymentBackend.Services;

namespace BillPaymentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillPaymentController : ControllerBase
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BillPaymentController> _logger;

        public BillPaymentController(
            IBillPaymentService billPaymentService,
            IConfiguration configuration,
            ILogger<BillPaymentController> logger)
        {
            _billPaymentService = billPaymentService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("fetch-bill")]
        public async Task<ActionResult<BillPaymentDetails>> FetchBill(
            [FromQuery] string consumerId,
            [FromQuery] string billType,
            [FromQuery] string provider)
        {
            try
            {
                var billDetails = await _billPaymentService.FetchBillDetailsAsync(
                    consumerId, billType, provider);
                return Ok(billDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill details");
                return StatusCode(500, new { message = "Failed to fetch bill details" });
            }
        }

        [HttpPost("initiate-payment")]
        public async Task<ActionResult<RazorpayOrder>> InitiatePayment(
            [FromBody] PaymentInitiationRequest request)
        {
            try
            {
                var order = await _billPaymentService.InitiatePaymentAsync(request);
                return Ok(new
                {
                    OrderId = order.OrderId,
                    Amount = order.FinalAmount,
                    DiscountApplied = order.DiscountAmount,
                    RazorpayKeyId = _configuration["Razorpay:KeyId"]
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment");
                return StatusCode(500, new { message = "Failed to initiate payment" });
            }
        }

        [HttpPost("verify-payment")]
        public async Task<ActionResult> VerifyPayment(
            [FromQuery] string paymentId,
            [FromQuery] string orderId,
            [FromQuery] string signature)
        {
            try
            {
                var isValid = await _billPaymentService.VerifyPaymentAsync(
                    paymentId, orderId, signature);

                if (!isValid)
                {
                    return BadRequest(new { message = "Invalid payment signature" });
                }

                return Ok(new { message = "Payment verified successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment");
                return StatusCode(500, new { message = "Failed to verify payment" });
            }
        }
    }
}

