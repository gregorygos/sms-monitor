using Microsoft.AspNetCore.Mvc;
using SmsRelay.Services;

namespace SmsRelay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly MessageRateLimiter _rateLimiter;

        public MessageController(MessageRateLimiter rateLimiter) {
            _rateLimiter = rateLimiter;
        }

        [HttpPost("can-send")]
        public IActionResult CanSendMessage([FromBody] MessageRequest request) {
        if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.AccountId)) {
            return BadRequest(new { error = "Invalid request payload" });
        }

        var canSend = _rateLimiter.CanSendMessage(request.PhoneNumber, request.AccountId);
        return Ok(new { canSend });
        } 

        [HttpGet("all-messages")]
        public IEnumerable<MessageInfo> GetAllMessages() {
            return _rateLimiter.GetAllMessages();
        }

        [HttpGet("messages-per-second")]
        public double GetMessagesPerSecond() {
            return _rateLimiter.GetMessagesPerSecond();
        }

        [HttpGet("filtered-messages")]
        public IEnumerable<MessageInfo> GetFilteredMessages([FromQuery] string phoneNumber, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) {
            return _rateLimiter.GetFilteredMessages(phoneNumber, startDate, endDate);
        }

        [HttpGet("messages-by-account")]
        public IEnumerable<MessageInfo> GetMessagesByAccount([FromQuery] string accountId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) {
            return _rateLimiter.GetMessagesByAccount(accountId, startDate, endDate);
        }
    }

    public class MessageRequest {
        public string PhoneNumber { get; set; }
        public string AccountId { get; set; }
    }
}
