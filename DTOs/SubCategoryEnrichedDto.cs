namespace ECS_Logistics.DTOs;

public class SubCategoryEnrichedDto
{
    public int SubCategoryId { get; set; }
    public ProductCategoryDto ProductCategory { get; set; }
    public string SubCategoryName { get; set; }
    public string? SubCategoryDescription { get; set; }
    public string? SubCategoryImage { get; set; }
}