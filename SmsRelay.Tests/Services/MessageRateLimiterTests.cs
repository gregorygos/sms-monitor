using Xunit;
using SmsRelay.Services;
using System.Threading.Tasks;

namespace SmsRelay.Tests.Services
{
    public class SmsRelayTests
    {
        private readonly MessageRateLimiter _service;
        private readonly int _maxAllowablePerNum;
        private readonly int _maxTotal;
        // I would  never do this in production!
        private const string numberOfAGuyWhoWantsThisJob = "4032467481";

        public SmsRelayTests()
        {
            _service = new MessageRateLimiter();
            _maxAllowablePerNum = MessageRateLimiter.MAX_MSG_PER_NUMBER_PER_SEC;
            _maxTotal = MessageRateLimiter.MAX_MSG_TOTAL_PER_SEC;
        }
        
        [Fact]
        public void CanSendMessageUnderLimit()
        {
            var result = _service.CanSendMessage(numberOfAGuyWhoWantsThisJob);
            Assert.True(result);
        }

        [Fact]
        public void CanNotSendMessageOverLimit()
        {
            for (int i = 0; i < _maxAllowablePerNum; i++)
            {
                _service.CanSendMessage(numberOfAGuyWhoWantsThisJob);
            }

            var result = _service.CanSendMessage(numberOfAGuyWhoWantsThisJob);
            Assert.False(result);
        }

        [Fact]
        public void CanNotSendMessageOverAccountLimit()
        {
            for (int i = 0; i < _maxTotal; i++)
            {
                // Unique phone numbers should fill the maxTotal.
                _service.CanSendMessage($"40{i}2467481");
            }

            var result = _service.CanSendMessage("4039876543");
            Assert.False(result);
        }

        [Fact]
        public async Task CanSendMessageAfterDelay()
        {
            _service.CanSendMessage(numberOfAGuyWhoWantsThisJob);

            // await to avoid blocking thread
            await Task.Delay(1005);

            var result = _service.CanSendMessage(numberOfAGuyWhoWantsThisJob);
            Assert.True(result);
        }

        [Fact]
        public async Task PhoneNumberRemovedAfterCountDecrements()
        {
            _service.CanSendMessage("4032467489");

            await Task.Delay(1000);

            // Check if the phone number has been removed from the dictionary
            var removed = !_service.PhoneNumberExists("40324674819");
            Assert.True(removed);
        }
        
    }
}
