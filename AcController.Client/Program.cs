using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AcController.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var culture = CultureInfo.CreateSpecificCulture("ar-SA");
            Thread.CurrentThread.CurrentCulture = culture; //Sets culture on the application
            Thread.CurrentThread.CurrentUICulture = culture;

            var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5022") //Fills in port number
            };
            //Uses Accept-Language header
            client.DefaultRequestHeaders.AcceptLanguage
                .Add(new StringWithQualityHeaderValue(culture.Name)); //Sets culture code

            //Gets only response body
            var responseBody = await client.GetStringAsync("/measure/TIJ1/1");
            var temps = JsonSerializer.Deserialize<TempsDto>( //Uses DTOs to deserialize
              responseBody, new JsonSerializerOptions()
              {
                  PropertyNameCaseInsensitive = true
              }
            );

            //Writes local time
            Console.WriteLine("Timestamp: " + temps!.Timestamp.LocalDateTime);
            Console.WriteLine("Site     : " + temps.Site);
            Console.WriteLine("Unit ID  : " + temps.UnitId);
            foreach (var measure in temps.Measurements)
            {
                //Right-aligns description to 36 chars
                var reading = culture.TextInfo.IsRightToLeft
                  ? $"C {measure.Value} :{measure.Description}"
                  : $"{measure.Description}: {measure.Value} C"; //Temperature is in Celsius.
                Console.WriteLine(reading);
            }
        }
    }
    internal record TempsDto(
              int UnitId,
              string Site,
              DateTimeOffset Timestamp,
              IEnumerable<MeasurementDto> Measurements)
    { }

    public record MeasurementDto(
      string SensorName,
      decimal Value,
      string Description)
    { }
}