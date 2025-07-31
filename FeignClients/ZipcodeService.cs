using ECS_Logistics.DTOs;
using Newtonsoft.Json;

namespace ECS_Logistics.FeignClients;

public class ZipcodeService(HttpClient httpClient)
{
    public async Task<string?> GetCityByZipAsync(string zipCode)
    {
        var response = await httpClient.GetAsync($"https://api.zippopotam.us/us/{zipCode}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Zip code not found");

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ZipcodeResponseDto>(content);

        var place = data?.Places.FirstOrDefault();
        return place?.PlaceName;
    }
}