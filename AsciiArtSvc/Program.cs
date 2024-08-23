using Figgle;

namespace AsciiArtSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            const StringComparison SCIC = StringComparison.OrdinalIgnoreCase;

            app.MapGet("/AllFonts", () => AsciiArt.AllFonts.Value);
            app.MapGet("/{text}", (string text, string? font) => AsciiArt.Write(text, font));
            app.MapGet("/TakeFont", (int take) => AsciiArt.AllFonts.Value.Take(take));
            app.MapGet("/SkipAndTake", (int? skip, int? take) => AsciiArt.AllFonts.Value.Skip(skip ?? 0).Take(take ?? 100));

            app.MapGet("/", (int? skip, int? take, FiggleTextDirection? dir, string? name, string? order) =>
            {
                var query = from f in AsciiArt.AllFontsParam.Value
                            where (name == null || f.Name.Contains(name, SCIC))
                              && (dir == null || f.Font.Direction == dir)
                            select f;
                if (string.Equals("desc", order, SCIC))
                {
                    query = query.OrderByDescending(f => f.Name);
                }
                else
                {
                    query = query.OrderBy(f => f.Name);
                }

                return query.Skip(skip ?? 0).Take(take ?? 200).Select(f => f.Name);
            });
            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }
}
