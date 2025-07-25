using ECS_Logistics.Configs;
using ECS_Logistics.DTOs;

namespace ECS_Logistics.FeignClients;

public class ProductService(FeignClient feignClient)
{
    private const string ServiceName = "ECS-PRODUCT";
    public async Task<ProductFinalDto?> GetProductById(int productId)
    {
        return await feignClient.GetAsync<ProductFinalDto>(ServiceName, $"api/product/{productId}");
    }
}