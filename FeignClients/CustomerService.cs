using ECS_Logistics.Configs;
using ECS_Logistics.DTOs;

namespace ECS_Logistics.FeignClients;

public class CustomerService(FeignClient feignClient)
{
    private const string ServiceName = "ECS-CUSTOMER";
    public async Task<AddressDto?> GetAddressById(int id)
    {
        return await feignClient.GetAsync<AddressDto>(ServiceName, $"api/address/{id}");
    }

    public async Task<CustomerDto?> GetCustomerById(int customerId)
    {
        return await feignClient.GetAsync<CustomerDto>(ServiceName, $"api/customer/{customerId}");
    }
}