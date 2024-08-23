
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Polly.Extensions.Http;
using Polly;
using XkcdComicFinder;

namespace ComicFinderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cfg = builder.Configuration;
            var connStr = cfg.GetConnectionString("Sqlite");
            var baseAddr = cfg.GetValue<string>("BaseAddr");

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IComicRepository, ComicRepository>();
            builder.Services.AddScoped<IXkcdClient, XkcdClient>();
            builder.Services.AddScoped<ComicFinder>();

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ComicDbContext>(option => option.UseSqlite(connStr));

            builder.Services.AddHttpClient<IXkcdClient, XkcdClient>(client => client.BaseAddress = new Uri(baseAddr!))
                .AddPolicyHandler(GetRetryPolicy());


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            using var keepAliveConn = new SqliteConnection(connStr);
            keepAliveConn.Open();

            using (var scope = app.Services.CreateScope())
            {
                var dbCtxt = scope.ServiceProvider.GetRequiredService<ComicDbContext>();
                dbCtxt.Database.EnsureCreated();
            }

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.Run();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => 
            HttpPolicyExtensions.HandleTransientHttpError().
            WaitAndRetryAsync(6, retryCount => //Retries up to six times
            TimeSpan.FromMilliseconds //Delays before each retry
            (100 * Math.Pow(2, retryCount))); //100 ms times a power of 2

        //Custom HTTP response-code retry policy
        static IAsyncPolicy<HttpResponseMessage> GetCustomRetryPolicy() => 
            Policy <HttpResponseMessage> //Indicates the result type
            .HandleResult(r => r.StatusCode ==System.Net.HttpStatusCode.TooManyRequests) //429 status code
            .OrTransientHttpStatusCode()
            .WaitAndRetryAsync(6, retryCount =>
            TimeSpan.FromMilliseconds
            (100 * Math.Pow(2, retryCount)));
    }
}
