using Xunit;
using SmsRelay.Services;
using System.Threading.Tasks;

namespace SmsRelay.Tests.Services
{
    public class SmsRelayTests
    {
        private readonly MessageRateLimiter _service;
        private const string NumberOfAGuyWhoWantsThisJob = "4032467481";
        private const string AccountId = "Account1";

        public SmsRelayTests()
        {
            _service = new MessageRateLimiter();
        }
        
        [Fact]
        public void CanSendMessageUnderLimit() {
            var result = _service.CanSendMessage(NumberOfAGuyWhoWantsThisJob, AccountId);
            Assert.True(result);
        }

        [Fact]
        public void CanNotSendMessageOverLimit()
        {
            for (int i = 0; i < MessageRateLimiter.MAX_MSG_PER_NUMBER_PER_SEC; i++) {
                _service.CanSendMessage(NumberOfAGuyWhoWantsThisJob, AccountId);
            }

            var result = _service.CanSendMessage(NumberOfAGuyWhoWantsThisJob, AccountId);
            Assert.False(result);
        }

        [Fact]
        public void CanNotSendMessageOverAccountLimit() {
            for (int i = 0; i < MessageRateLimiter.MAX_MSG_TOTAL_PER_SEC; i++) {
                _service.CanSendMessage($"40{i}2467481", AccountId);
            }

            var result = _service.CanSendMessage("4039876543", AccountId);
            Assert.False(result);
        }

        [Fact]
        public async Task CanSendMessageAfterDelay() {
            _service.CanSendMessage(NumberOfAGuyWhoWantsThisJob, AccountId);

            // Await to avoid blocking thread
            await Task.Delay(1005);

            var result = _service.CanSendMessage(NumberOfAGuyWhoWantsThisJob, AccountId);
            Assert.True(result);
        }
    }
}
