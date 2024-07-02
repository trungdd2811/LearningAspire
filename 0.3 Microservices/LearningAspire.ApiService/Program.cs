using LearningAspire.Commons;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisDistributedCache(Constants.RedisCache);
// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (IDistributedCache cache) =>
{
	byte[]? cachedForecast = null;
	try
	{
		cachedForecast = await cache.GetAsync("forecast");
	}
	catch (Exception ex)
	{
		//do nothing
	}

	if (cachedForecast is null)
	{
		var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
		var forecast = Enumerable.Range(1, 5).Select(index =>
		new WeatherForecast
		(
			DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			Random.Shared.Next(-20, 55),
			summaries[Random.Shared.Next(summaries.Length)]
		))
		.ToArray();
		try
		{
			await cache.SetAsync("forecast", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(forecast)), new()
			{
				AbsoluteExpiration = DateTime.Now.AddSeconds(10)
			});
		}
		catch (Exception ex)
		{
			//do nothing
		}

		return forecast;
	}

	return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedForecast);
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}