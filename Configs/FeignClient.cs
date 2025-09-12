using ECS_Logistics.DTOs;
using Steeltoe.Discovery;

namespace ECS_Logistics.Configs;

public class FeignClient(IHttpClientFactory httpClientFactory, 
    IDiscoveryClient discoveryClient, 
    IHttpContextAccessor httpContextAccessor,
    ServiceToServiceAuthorization serviceAuthTokenProvider)
{
    public async Task<T?> GetAsync<T>(string serviceName, string path)
    {
        var instances = discoveryClient.GetInstances(serviceName);
        if (instances == null || instances.Count == 0)
            throw new Exception($"Service {serviceName} not found in Eureka");

        var baseUri = instances[0].Uri;
        var client = httpClientFactory.CreateClient();
        var headers = httpContextAccessor.HttpContext?.Request.Headers;
        string? authHeader = headers?["Authorization"];
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
        if (headers == null || string.IsNullOrEmpty(authHeader))
        {
            string serviceToken = serviceAuthTokenProvider.GetToken();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceToken}");
        }
        var response = await client.GetAsync($"{baseUri}{path}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}