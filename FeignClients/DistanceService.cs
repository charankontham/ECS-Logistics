using ECS_Logistics.Configs;
using ECS_Logistics.DTOs;
using Newtonsoft.Json;

namespace ECS_Logistics.FeignClients;

public class DistanceService(IConfiguration configuration, HttpClient httpClient)
{
    private readonly string _apiKey = configuration["GoogleMaps:ApiKey"] ?? string.Empty;
    public async Task<(double DistanceInKm, string DurationText)> GetDistanceAsync(string origin, string destination)
    {
        var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins=" +
                  $"{Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&units=metric&key={_apiKey}";

        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Google Maps API call failed");

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<DistanceMatrixResponse>(json);

        var element = data?.rows[0].elements[0];

        if (element?.status != "OK")
            throw new Exception("No route found between the addresses");

        return (element.distance.value / 1000.0, element.duration.text);
    }
}