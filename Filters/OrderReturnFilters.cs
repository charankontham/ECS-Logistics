namespace ECS_Logistics.Filters;

public class OrderReturnFilters
{
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? ReturnReasonCategoryId { get; set; }
}