using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EtherscanTest.Helpers
{
    public class RetryHandler : DelegatingHandler
    {
        private static int MAX_RETRIES = int.Parse(ConfigurationManager.AppSettings["MAX_RETRIES"]);
        private static int DELAY_PERIOD = int.Parse(ConfigurationManager.AppSettings["DELAY_PERIOD"]);

        public RetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < MAX_RETRIES; i++)
            {
                await Task.Delay(DELAY_PERIOD);
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }
}
