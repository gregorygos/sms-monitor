using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SmsRelay.Services
{
    public class MessageInfo
    {
        public string? PhoneNumber { get; set; }
        public string? AccountId { get; set; }
        public List<DateTime> MessageTimestamps { get; set; } = new List<DateTime>();
    }

    public class MessageRateLimiter
    {
        public const int MAX_MSG_PER_NUMBER_PER_SEC = 5;
        public const int MAX_MSG_TOTAL_PER_SEC = 20;

        private readonly ConcurrentDictionary<string, MessageInfo> _messageDataByNumber = new ConcurrentDictionary<string, MessageInfo>();
        private readonly ConcurrentDictionary<string, MessageInfo> _messageDataByAccount = new ConcurrentDictionary<string, MessageInfo>();

        public bool CanSendMessage(string phoneNumber, string accountId)
        {
            var now = DateTime.UtcNow;
            bool canSendByNumber = CanSendByPhoneNumber(phoneNumber, now);
            bool canSendByAccount = CanSendByAccount(accountId, now);

            return canSendByNumber && canSendByAccount;
        }

        private bool CanSendByPhoneNumber(string phoneNumber, DateTime now)
        {
            var messageInfo = _messageDataByNumber.GetOrAdd(phoneNumber, new MessageInfo { PhoneNumber = phoneNumber });

            lock (messageInfo.MessageTimestamps)
            {
                CleanOldMessages(messageInfo, now);
                if (messageInfo.MessageTimestamps.Count >= MAX_MSG_PER_NUMBER_PER_SEC) {
                    return false;
                }
                messageInfo.MessageTimestamps.Add(now);
            }

            return true;
        }

        private bool CanSendByAccount(string accountId, DateTime now)
        {
            var messageInfo = _messageDataByAccount.GetOrAdd(accountId, new MessageInfo { AccountId = accountId });

            lock (messageInfo.MessageTimestamps)
            {
                CleanOldMessages(messageInfo, now);
                if (messageInfo.MessageTimestamps.Count >= MAX_MSG_TOTAL_PER_SEC)
                {
                    return false;
                }
                messageInfo.MessageTimestamps.Add(now);
            }

            return true;
        }

        private void CleanOldMessages(MessageInfo messageInfo, DateTime now)
        {
            messageInfo.MessageTimestamps.RemoveAll(timestamp => (now - timestamp).TotalSeconds > 1);
        }

        // Testing method
        public bool PhoneNumberExists(string phoneNumber)
        {
            return _messageDataByNumber.ContainsKey(phoneNumber);
        }

        public IEnumerable<MessageInfo> GetAllMessages()
        {
            return _messageDataByNumber.Values.Concat(_messageDataByAccount.Values);
        }

        public double GetMessagesPerSecond()
        {
            var now = DateTime.UtcNow;
            var oneSecondAgo = now.AddSeconds(-1);

            var messageCount = _messageDataByNumber.Values
                .Concat(_messageDataByAccount.Values)
                .SelectMany(info => info.MessageTimestamps)
                .Count(timestamp => timestamp > oneSecondAgo);

            return messageCount;
        }

        public IEnumerable<MessageInfo> GetFilteredMessages(string phoneNumber, DateTime? startDate, DateTime? endDate)
        {
            return _messageDataByNumber.Values
                .Where(info => (string.IsNullOrEmpty(phoneNumber) || info.PhoneNumber == phoneNumber) &&
                               (!startDate.HasValue || info.MessageTimestamps.Any(t => t >= startDate)) &&
                               (!endDate.HasValue || info.MessageTimestamps.Any(t => t <= endDate)));
        }

        public IEnumerable<MessageInfo> GetMessagesByAccount(string accountId, DateTime? startDate, DateTime? endDate)
        {
            return _messageDataByAccount.Values
                .Where(info => (string.IsNullOrEmpty(accountId) || info.AccountId == accountId) &&
                               (!startDate.HasValue || info.MessageTimestamps.Any(t => t >= startDate)) &&
                               (!endDate.HasValue || info.MessageTimestamps.Any(t => t <= endDate)));
        }
    }
}
