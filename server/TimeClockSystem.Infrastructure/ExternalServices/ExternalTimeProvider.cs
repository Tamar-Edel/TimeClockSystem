using System.Text.Json;
using TimeClockSystem.Core.Interfaces;

namespace TimeClockSystem.Infrastructure.ExternalServices;

public class ExternalTimeProvider : IExternalTimeProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalTimeProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<DateTimeOffset> GetCurrentTimeAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://timeapi.io/api/v1/time/current/zone?timezone=Europe/Zurich");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(body);

            // timeapi.io returns the field as "date_time" (snake_case).
            // The value includes the UTC offset, e.g. "2026-03-11T18:50:31.480199+01:00".
            // DateTimeOffset.Parse correctly preserves the Zurich offset including DST.
            var datetimeString = document.RootElement.GetProperty("date_time").GetString();

            return DateTimeOffset.Parse(datetimeString!);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Unable to retrieve current time from external time provider.");
        }
    }
}
