using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BillPaymentBackend.Models;
using BillPaymentBackend.Services;

namespace BillPaymentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Requires authentication and admin role
    public class AdminController : ControllerBase
    {
        private readonly IBillPaymentService _billPaymentService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IBillPaymentService billPaymentService,
            ILogger<AdminController> logger)
        {
            _billPaymentService = billPaymentService;
            _logger = logger;
        }

        [HttpGet("pending-transactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetPendingTransactions()
        {
            try
            {
                var transactions = await _billPaymentService.GetPendingTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending transactions");
                return StatusCode(500, new { message = "Failed to fetch pending transactions" });
            }
        }

        [HttpPost("verify-transaction/{transactionId}")]
        public async Task<ActionResult> VerifyTransaction(
            int transactionId,
            [FromBody] AdminVerificationRequest request)
        {
            try
            {
                var success = await _billPaymentService.ProcessAdminVerificationAsync(
                    transactionId,
                    request.Status,
                    request.Comments);

                if (!success)
                {
                    return BadRequest(new { message = "Failed to verify transaction" });
                }

                return Ok(new { message = "Transaction verified successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying transaction");
                return StatusCode(500, new { message = "Failed to verify transaction" });
            }
        }
    }
}

