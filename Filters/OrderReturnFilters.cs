namespace ECS_Logistics.Filters;

public class OrderReturnFilters
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? ReturnReasonCategoryId { get; set; }
}