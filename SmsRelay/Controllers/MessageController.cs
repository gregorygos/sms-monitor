    using Microsoft.AspNetCore.Mvc;
    using SmsRelay.Services;

    namespace SmsRelay.Controllers
    {
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly MessageRateLimiter _rateLimiter;

        public MessageController(MessageRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        [HttpPost("can-send")]
        public IActionResult CanSendMessage([FromBody] string phoneNumber)
        {
            var canSend = _rateLimiter.CanSendMessage(phoneNumber);

            return Ok(
                new { canSend }
            );
        }
    }
    }
