using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHttpClientRetry
{
    public class RetryHandler : System.Net.Http.DelegatingHandler
    {
        // Strongly consider limiting the number of retries - "retry forever" is
        // probably not the most user friendly way you could respond to "the
        // network cable got pulled out."
        public int MaxRetries { get; set; }
        public int TimeToWait { get; set; }

        public RetryHandler(System.Net.Http.HttpMessageHandler innerHandler, int maxRetries = 3, int timeToWait = 3000)
            : base(innerHandler)
        {
            MaxRetries = maxRetries;
            TimeToWait = timeToWait;
        }

        protected override async Task<System.Net.Http.HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                //Debug only
                Console.WriteLine($"Overrite SendAsync request {i} IsSuccessStatusCode:{response.IsSuccessStatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    System.Threading.Thread.Sleep(TimeToWait);
                }
            }

            return response;
        }
    }

}
