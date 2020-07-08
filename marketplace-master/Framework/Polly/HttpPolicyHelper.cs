using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace Framework.Polly
{
    public static class HttpPolicyHelper
    {
        private static AsyncRetryPolicy<HttpResponseMessage> Retry(TimeSpan[] sleepDurations = null)
        {
            IEnumerable<TimeSpan> GetSleepDurations()
            {
                if (sleepDurations == null || !sleepDurations.Any())
                {
                    return new[]
                    {
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5)
                    };
                }

                return sleepDurations;
            }

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(GetSleepDurations());
        }

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreaker(int handledEventsAllowedBeforeBreaking = 6, TimeSpan durationOfBreak = default(TimeSpan))
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
                    durationOfBreak: durationOfBreak == default(TimeSpan) ? TimeSpan.FromSeconds(60) : durationOfBreak
                );
        }

        private static AsyncTimeoutPolicy<HttpResponseMessage> Timeout(TimeSpan timeout = default(TimeSpan))
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(timeout: timeout == default(TimeSpan) ? TimeSpan.FromSeconds(30) : timeout);
        }

        public static AsyncPolicyWrap<HttpResponseMessage> ResilienceStrategy(TimeSpan[] sleepDurations = null,
            int handledEventsAllowedBeforeBreaking = 7, TimeSpan durationOfBreak = default(TimeSpan),
            TimeSpan timeout = default(TimeSpan))
        {
            return Policy.WrapAsync(
                Retry(sleepDurations),
                CircuitBreaker(handledEventsAllowedBeforeBreaking, durationOfBreak),
                Timeout(timeout));
        }
    }
}