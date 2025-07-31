using Newtonsoft.Json;

namespace ECS_Logistics.DTOs;

public class ZipcodeResponseDto
{
    [JsonProperty("post code")]
    public string PostCode { get; set; }

    public string Country { get; set; }

    public List<Place> Places { get; set; }
}

public class Place
{
    [JsonProperty("place name")]
    public string PlaceName { get; set; }

    public string State { get; set; }
}