namespace ECS_Logistics.Utils;

public enum StatusCodesEnum
{
    InvalidEmail = 1,
    InvalidPassword = 2,
    DeliveryAgentNotFound = 3,
    EmailAlreadyExists = 4,
    AuthenticationFailed = 5,
    DeliveryHubNameAlreadyExists = 6,
    DeliveryHubNotFound = 7,
    AddressNotFound = 8,
    OrderReturnNotFound = 9,
    OrderTrackingNotFound = 10,
    InternalServerError = 11,
    EnrichedDtoMappingsFailed = 12,
    FailedToUpdateOrderTracking = 13,
    ValidationFailed = 14,
    NoErrorFound = 20,
}

public static class ServiceUrls
{
    public const string BaseAddress = "http://localhost:8080";
    public const string AddressService = "http://localhost:8080/ecs-customer/api/address";
    public const string AdminService = "http://localhost:8080/ecs-inventory-admin/api/admin";
}