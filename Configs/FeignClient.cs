using ECS_Logistics.DTOs;
using Steeltoe.Discovery;

namespace ECS_Logistics.Configs;

public class FeignClient(IHttpClientFactory httpClientFactory, 
    IDiscoveryClient discoveryClient, 
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<T?> GetAsync<T>(string serviceName, string path)
    {
        var instances = discoveryClient.GetInstances(serviceName);
        if (instances == null || instances.Count == 0)
            throw new Exception($"Service {serviceName} not found in Eureka");

        var baseUri = instances[0].Uri;
        var client = httpClientFactory.CreateClient();
        var headers = httpContextAccessor.HttpContext?.Request.Headers;
        if (headers != null)
        {
            var restrictedHeaders = new[] { "Content-Type", "Content-Length" };
            foreach (var header in headers)
            {
                if (!restrictedHeaders.Contains(header.Key) && !client.DefaultRequestHeaders.Contains(header.Key))
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
                }
            }
        }
        var response = await client.GetAsync($"{baseUri}{path}");
        // Console.WriteLine($"Feign client response : {await response.Content.ReadAsStringAsync()}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}