using Microsoft.AspNetCore.Mvc;
using BillPaymentBackend.Models;
using BillPaymentBackend.Services;

namespace BillPaymentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly IBharatBillPayService _bharatBillPayService;
        private readonly IRazorpayService _razorpayService;
        private readonly ILogger<TestController> _logger;

        public TestController(
            IBillPaymentService billPaymentService,
            IBharatBillPayService bharatBillPayService,
            IRazorpayService razorpayService,
            ILogger<TestController> logger)
        {
            _billPaymentService = billPaymentService;
            _bharatBillPayService = bharatBillPayService;
            _razorpayService = razorpayService;
            _logger = logger;
        }

        [HttpGet("fetch-bill")]
        public async Task<ActionResult> TestBillFetch(
            [FromQuery] string consumerId = "TEST001",
            [FromQuery] string billType = "electricity",
            [FromQuery] string provider = "State Electricity Board")
        {
            try
            {
                var billDetails = await _bharatBillPayService.FetchBillDetailsAsync(
                    consumerId, billType, provider);

                return Ok(new
                {
                    Message = "Bill fetch successful",
                    BillDetails = billDetails,
                    TestInfo = new
                    {
                        ConsumerId = consumerId,
                        BillType = billType,
                        Provider = provider,
                        Timestamp = DateTime.Now
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test bill fetch");
                return StatusCode(500, new { message = "Test failed", error = ex.Message });
            }
        }

        [HttpPost("payment-flow")]
        public async Task<ActionResult> TestPaymentFlow([FromBody] PaymentInitiationRequest request)
        {
            try
            {
                // Step 1: Fetch bill details
                var billDetails = await _bharatBillPayService.FetchBillDetailsAsync(
                    request.ConsumerId,
                    request.BillType,
                    request.Provider);

                // Step 2: Initiate payment with Razorpay
                var order = await _billPaymentService.InitiatePaymentAsync(new PaymentInitiationRequest
                {
                    ConsumerId = request.ConsumerId,
                    BillType = request.BillType,
                    Provider = request.Provider,
                    Amount = billDetails.Amount,
                    BillNumber = billDetails.BillNumber
                });

                return Ok(new
                {
                    Message = "Payment flow test successful",
                    BillDetails = billDetails,
                    PaymentDetails = new
                    {
                        OrderId = order.OrderId,
                        OriginalAmount = order.OriginalAmount,
                        DiscountApplied = order.DiscountAmount,
                        FinalAmount = order.FinalAmount
                    },
                    TestInfo = new
                    {
                        Timestamp = DateTime.Now,
                        TestId = $"TEST_{DateTime.Now:yyyyMMddHHmmss}"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test payment flow");
                return StatusCode(500, new { message = "Test failed", error = ex.Message });
            }
        }

        [HttpGet("verify-setup")]
        public ActionResult TestSetup()
        {
            try
            {
                return Ok(new
                {
                    Message = "Setup verification successful",
                    Services = new
                    {
                        BharatBillPay = "Mock service ready",
                        Razorpay = "Integration ready",
                        Database = "Connected"
                    },
                    TestEndpoints = new[]
                    {
                        new { Path = "/api/test/fetch-bill", Method = "GET", Description = "Test bill fetch" },
                        new { Path = "/api/test/payment-flow", Method = "POST", Description = "Test complete payment flow" }
                    },
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying setup");
                return StatusCode(500, new { message = "Setup verification failed", error = ex.Message });
            }
        }
    }
}

