using System.Collections.Concurrent;

namespace SmsRelay.Services
{
    public class MessageRateLimiter
    {
        // Threadsafe dictionary -- could use something like Redis which is faster in the future.
        // May not stand up to very high loads, would suggest Distributed Cache with REdis
        private static readonly ConcurrentDictionary<string, int> _phoneMsgCounts = new ConcurrentDictionary<string, int>(); 
        private static int _totalMsgCount = 0;
        public static readonly int MAX_MSG_PER_NUMBER_PER_SEC = 1;
        public static readonly int MAX_MSG_TOTAL_PER_SEC = 5;

        public bool CanSendMessage(string phoneNum) {
            // TODO: Validate phone number is legit
            _phoneMsgCounts.TryGetValue(phoneNum, out int phoneCount);

            if (phoneCount >= MAX_MSG_PER_NUMBER_PER_SEC) {
                Console.WriteLine($"[{phoneNum}][Limit Exceeded] Phone Number Limit: {phoneCount}");
                return false;
            }

            if (_totalMsgCount >= MAX_MSG_TOTAL_PER_SEC) {
                Console.WriteLine($"[Limit Exceeded] Total Limit: {_totalMsgCount}");
                return false;
            }

            _phoneMsgCounts[phoneNum] = phoneCount + 1;
            _totalMsgCount++; 

            Console.WriteLine($"[Message Sent] {phoneNum}: PhoneCount = {phoneCount + 1}, TotalCount = {_totalMsgCount}");

            // Non-blocking - this should go to the threadpool until it decrements it's own value.
            Task.Delay(1000).ContinueWith(_ => {
                // Realtime decrement the counts - do specify time period above
                _phoneMsgCounts.AddOrUpdate(phoneNum, 0, (key, count) => count - 1);
                _totalMsgCount--;

                // Remove phone number from dictionary if count is 0
                // I would rather use a cache that has a LRU expiry
                _phoneMsgCounts.TryGetValue(phoneNum, out int updatedCount);
                if (updatedCount <= 0) {
                    _phoneMsgCounts.TryRemove(phoneNum, out int _);
                }
            });

            return true;

        }

          // Method to check if the phone number exists in the dictionary -- mostly for testing
        public bool PhoneNumberExists(string phoneNum) {
            return _phoneMsgCounts.ContainsKey(phoneNum);
        }

        // Commented code should not be in production, but I did want some sort of Deadlock reporting.
        // public bool CanSendMessageWithRetry(string phoneNum, int maxRetries = 3) {
        //     int attempts = 0;

        //     while (attempts < maxRetries) {
        //         try {
        //             return CanSendMessage(phoneNum);
        //         } catch (Exception ex) {
        //             attempts++;
        //             Console.WriteLine($"[{phoneNum}][Retry] Attempt {attempts} failed: {ex.Message}");
        //             if (attempts >= maxRetries) {
        //                 Console.WriteLine($"[CRITICAL] All retries failed for {phoneNum}");
        //                 return false;
        //             }
        //         }
        //     }
        //     return false;
        // }
    }

    
}