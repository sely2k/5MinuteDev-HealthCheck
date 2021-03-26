using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleHttpClientRetry
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var urlOk = "https://httpbin.org/status/200,500";
            var urlKo = "https://httpbin.org/status/400,500";
            

            Console.WriteLine("--clientWithoutRetry");
            using (HttpClient clientWithoutRetry = new HttpClient())
            {
                HttpRequestMessage requestOk = new HttpRequestMessage(HttpMethod.Get, urlOk);
                HttpRequestMessage requestKo = new HttpRequestMessage(HttpMethod.Get, urlKo);

                Console.WriteLine("RequestOk");
                var resultOk = await clientWithoutRetry.SendAsync(requestOk);
                await printResutlAsync(resultOk);

                Console.WriteLine("RequestKo");
                var resultKo = await clientWithoutRetry.SendAsync(requestKo);
                await printResutlAsync(resultKo);
            }

            Console.WriteLine("--clientWithRetry");
            using (HttpClient clientWithRetry = new HttpClient(new RetryHandler(new HttpClientHandler(), 5)))
            {
                HttpRequestMessage requestOk = new HttpRequestMessage(HttpMethod.Get, urlOk);
                HttpRequestMessage requestKo = new HttpRequestMessage(HttpMethod.Get, urlKo);

                Console.WriteLine("RequestOk");
                var resultOk = await clientWithRetry.SendAsync(requestOk, HttpCompletionOption.ResponseContentRead);
                await printResutlAsync(resultOk);

                Console.WriteLine("RequestKo");
                var resultKo = await clientWithRetry.SendAsync(requestKo);
                await printResutlAsync(resultKo);
            }
        }

        public static async Task printResutlAsync(HttpResponseMessage httpResponseMessage)
        {
            
            Console.WriteLine($"httpResponseMessageIsSuccessStatusCode:{httpResponseMessage.IsSuccessStatusCode}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine($"{httpResponseMessage.ReasonPhrase} {httpResponseMessage.StatusCode}");
            }

            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
        }
    }
}
