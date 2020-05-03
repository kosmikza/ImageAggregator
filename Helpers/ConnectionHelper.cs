using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ImmageAggregatorAPI
{
    public class ConnectionHelper
    {
        private readonly HttpClient _httpClient;
        private readonly CancellationToken _cancellationToken = default;

        public ConnectionHelper(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public async Task<T> GetAsync<T>(string request)
        {
            var response = await _httpClient.GetAsync(request, _cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    throw new HttpRequestException($"{response.StatusCode}:{response.Content}");
                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, cancellationToken: default);
            }

            throw new HttpRequestException($"{response.StatusCode}:{response.Content.ReadAsStringAsync().Result}");
        }
        public async Task<byte[]> GetAsyncByte(string request)
        {
            var response = await _httpClient.GetAsync(request, _cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    throw new HttpRequestException($"{response.StatusCode}:{response.Content}");
                return await response.Content.ReadAsByteArrayAsync();
            }

            throw new HttpRequestException($"{response.StatusCode}:{response.Content.ReadAsStringAsync().Result}");
        }
    }
}
