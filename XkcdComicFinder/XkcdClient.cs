using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace XkcdComicFinder
{
    public class XkcdClient : IXkcdClient
    {
        private const string PageUri = "info.0.json";
        private readonly HttpClient _httpClient;

        public XkcdClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<Comic?> GetByNumberAsync(int number)
        {
            try
            {
                var path = $"{number}/{PageUri}";
                var stream = await _httpClient.GetStreamAsync(path);
                return JsonSerializer.Deserialize<Comic>(stream);
            }
            catch (AggregateException e) when (e.InnerException is HttpRequestException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<Comic> GetLatestAsync()
        {
            var stream = await _httpClient.GetStreamAsync(PageUri);
            return JsonSerializer.Deserialize<Comic>(stream)!;
        }
    }
}
