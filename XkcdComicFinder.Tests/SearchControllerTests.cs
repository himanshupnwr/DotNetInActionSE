using ComicFinderService;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace XkcdComicFinder.Tests
{
    public class SearchControllerTests
    {
        private const string BaseAddress = "https://xkcd.com";
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpMessageHandler _fakeMsgHandler;

        public SearchControllerTests()
        {
            _fakeMsgHandler = A.Fake<HttpMessageHandler>();
            //WebApplicationFactory creates an in-memory host that we can use for testing.
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    //The environment can be set by environment variable, add an appsettings.Integration.json file
                    //builder.UseEnvironment("Integration");
                    builder.ConfigureServices(services =>
                    {
                        //Finds HttpClient added by Program.cs
                        ServiceDescriptor sd = services.First(s => s.ServiceType == typeof(HttpClient));
                        //Removed from dependency injection
                        services.Remove(sd);
                        //Removed from dependency injection
                        services.AddHttpClient<IXkcdClient, XkcdClient>(h => h.BaseAddress = new Uri(BaseAddress))
                        //Uses fake message handler
                        .ConfigurePrimaryHttpMessageHandler(() => _fakeMsgHandler);
                    });
                });
        }

        //The WebApplicationFactory provides a client that we can call as though we’re trying to make an HTTP call to our service.

        [Fact]
        public async Task FoundB()
        {
            XkcdComicFinderTests.SetResponseComics(_fakeMsgHandler,
              new Comic() { Number = 12, Title = "b" },
              new Comic() { Number = 1, Title = "a" },
              new Comic() { Number = 4, Title = "c" });

            HttpClient client = _factory.CreateClient();
            var response = await client.GetAsync("/search?searchText=b");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            var comics = JsonSerializer.Deserialize<Comic[]>(content);
            Assert.NotNull(comics);
            Assert.Single(comics);
            Assert.Single(comics, c => c.Number == 12);
        }
    }
}
