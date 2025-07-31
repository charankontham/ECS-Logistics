namespace ECS_Logistics.DTOs;

public class ProductFinalDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public ProductBrandDto Brand { get; set; }
    public SubCategoryEnrichedDto ProductSubCategory { get; set; }
    public string? ProductDescription { get; set; }
    public float? ProductPrice { get; set; }
    public int? ProductQuantity { get; set; }
    public string? ProductImage { get; set; }
    public string? ProductColor { get; set; }
    public float? ProductWeight { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? DateModified { get; set; }
    public string? ProductDimensions { get; set; }
    public string? ProductCondition { get; set; }
}