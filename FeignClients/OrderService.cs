using ECS_Logistics.Configs;
using ECS_Logistics.DTOs;

namespace ECS_Logistics.FeignClients;

public class OrderService(FeignClient feignClient)
{
    private const string ServiceName = "ECS-ORDER";
    public async Task<OrderItemDto?> GetOrderItemByOrderItemId(int orderItemId)
    {
        return await feignClient.GetAsync<OrderItemDto>(ServiceName, $"api/order/getOrderItemByOrderItemId/{orderItemId}");
    }
}