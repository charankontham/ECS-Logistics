using System.Transactions;
using AutoMapper;
using ECS_Logistics.Data;
using ECS_Logistics.DbContexts;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Repositories;
using ECS_Logistics.Utils;
using Serilog.Core;

namespace ECS_Logistics.Services;

public class DeliveryHubService(
    IDeliveryHubRepository repository, 
    CustomerService customerService,
    IMapper mapper, 
    MySqlDbContext context, 
    ILogger<DeliveryHubService> logger) : IDeliveryHubService
{
    public async Task<IEnumerable<DeliveryHubEnrichedDto>> GetAllHubsAsync(DeliveryHubFilters? filters)
    {
        var hubs = await repository.GetAllAsync(filters);
        if (filters is { Address: not null } && filters.Address.Trim() != "")
        {
            hubs = GetMatchingAddressHubs(hubs, filters.Address, logger);
        }
        return mapper.Map<IEnumerable<DeliveryHubEnrichedDto>>(hubs);
    }

    public async Task<object> GetHubByIdAsync(int id)
    {
        var hub = await repository.GetByIdAsync(id);
        if (hub == null) return StatusCodesEnum.DeliveryHubNotFound;
        return mapper.Map<DeliveryHubEnrichedDto>(hub);
    }

    public async Task<object> CreateHubAsync(DeliveryHubDto hubDto)
    {
        if (context.DeliveryHubs.Any(x => x.DeliveryHubName == hubDto.DeliveryHubName))
        {
            return StatusCodesEnum.DeliveryHubNameAlreadyExists;
        }

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                var hub = mapper.Map<DeliveryHub>(hubDto);
                hub.DateAdded = DateTime.Now;
                hub.DateModified = DateTime.Now;
                var createdHub = await repository.CreateAsync(hub);
                var enrichedHubDto = mapper.Map<DeliveryHubEnrichedDto>(createdHub);
                scope.Complete();
                return enrichedHubDto;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }

    public async Task<object> UpdateHubAsync(DeliveryHubDto hubDto)
    {
        if (hubDto.DeliveryHubId == null)
        {
            return StatusCodesEnum.DeliveryHubNotFound;
        }
        if (hubDto.DeliveryHubAddressId != null && !IsAddressExists(hubDto.DeliveryHubAddressId))
        {
            return StatusCodesEnum.AddressNotFound;
        }
        var hub = mapper.Map<DeliveryHub>(hubDto);
        hub.DateModified = DateTime.Now;
        try
        {
            var updatedHub = await repository.UpdateAsync(hub);
            return mapper.Map<DeliveryHubEnrichedDto>(updatedHub);
        }
        catch (Exception ex)
        {
            logger.LogWarning($"DeliveryHubId not found! , details : {ex.Message}");
            return StatusCodesEnum.DeliveryHubNotFound;
        }
    }

    public async Task<bool> DeleteHubAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    private static IEnumerable<DeliveryHub> GetMatchingAddressHubs(
        IEnumerable<DeliveryHub> hubs, 
        string searchValue,
        ILogger<DeliveryHubService> logger)
    {
        var hubsArray = hubs as DeliveryHub[] ?? hubs.ToArray();
        var matchingHubs = hubsArray.Where(hub =>
        {
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync($"{ServiceUrls.AddressService}/{hub.DeliveryHubAddressId}").GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var address = response.Content.ReadFromJsonAsync<AddressDto>().GetAwaiter().GetResult();
                switch (address)
                {
                    case { State: not null } when address.State.Equals(searchValue):
                    case { City: not null } when address.City.Equals(searchValue):
                    case { Zip: not null } when address.Zip.Equals(searchValue):
                        return true;
                }
            }
            else
            {
                logger.LogError(
                    "Failed to fetch Delivery Hub address with id  {ArgDeliveryHubAddressId}, " +
                    "Responded with status code : {ResponseStatusCode}", 
                    hub.DeliveryHubAddressId, response.StatusCode);
            }
            return false;
        });
        return matchingHubs;
    }

    private bool IsAddressExists(int? id)
    {
        if (id == null) 
            return false;
        try
        {
            var response = customerService.GetAddressById(id ?? 0);
            return response.Result != null;
        }
        catch (Exception ex)
        {
            logger.LogError("IsAddressExists : " + ex.Message);
            return false;
        }
    }
}