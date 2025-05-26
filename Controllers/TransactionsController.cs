// Controllers/TransactionsController.cs
using Microsoft.AspNetCore.Mvc;
using BillPaymentBackend.Models;
using BillPaymentBackend.Data;
using System.Threading.Tasks;

namespace BillPaymentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PaymentDto dto)
        {
            // For now, simulate payment success
            var transaction = new Transaction
            {
                UserId = dto.UserId,
                BillType = dto.BillType,
                BillNumber = dto.BillNumber,
                Amount = dto.Amount,
                DiscountAmount = 5, // Rs 5 discount
                FinalAmount = dto.Amount - 5,
                PaymentStatus = "Success"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment recorded successfully", transactionId = transaction.TransactionId });
        }
    }

    // DTO
    public class PaymentDto
    {
        public int UserId { get; set; }
        public string BillType { get; set; }
        public string BillNumber { get; set; }
        public decimal Amount { get; set; }
    }
}