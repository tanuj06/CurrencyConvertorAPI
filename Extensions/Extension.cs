namespace CurrencyConverterAPI.Extensions
{
    public static class Extension
    {
        /// <summary>
        /// This extension method will be responsible to handle external service failure in giving response
        /// It will try for 5 times to hit the external url and will wait for 2 seconds before sending next request
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="requestUri"></param>
        /// <param name="maxRetries"></param>
        /// <param name="delayinMS"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<HttpResponseMessage> GetWithMultipleRetryAsync(this HttpClient httpClient, string requestUri, int maxRetries = 5, int delayinMS = 2000)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    var response = await httpClient.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                    else
                    {
                        throw new HttpRequestException($"Unexpected status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    attempt++;
                    var waitTime = TimeSpan.FromMilliseconds(delayinMS);
                    Console.WriteLine($"Attempt {attempt} failed with error: {ex.Message}. Retrying in {waitTime.TotalMilliseconds}ms...");
                    await Task.Delay(waitTime);
                }
            }
        }
    }
}
