using ECS_Logistics.Configs;
using ECS_Logistics.DTOs;

namespace ECS_Logistics.FeignClients;

public class InventoryService(FeignClient feignClient)
{
    private const string ServiceName = "ecs-inventory-admin";
    public async Task<AdminDataDto?> GetAdminById(string id)
    {
        return await feignClient.GetAsync<AdminDataDto>(ServiceName, $"/api/admin/getById/{id}");
    }
}