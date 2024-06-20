namespace LearningAspire.Web;

public class WeatherApiClient(HttpClient httpClient)
{
	public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
	{
		List<WeatherForecast>? forecasts = null;

		try
		{
			await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
			{
				if (forecasts?.Count >= maxItems)
				{
					break;
				}
				if (forecast is not null)
				{
					forecasts ??= [];
					forecasts.Add(forecast);
				}
			}
		}
		catch (Exception ex)
		{

		}
		return forecasts?.ToArray() ?? [];



		//WeatherForecast[]? forecasts = null;

		//try
		//{
		//	forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast", cancellationToken);
		//}
		//catch (Exception ex)
		//{
		//	//do nothing
		//}

		//return forecasts ?? [];
	}
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
