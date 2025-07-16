using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECS_Logistics.Models;

[Table("product")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Column("product_category_id")]
    public int ProductCategoryId { get; set; }

    [Column("sub_category_id")]
    public int SubCategoryId { get; set; }

    [Column("product_brand_id")]
    public int ProductBrandId { get; set; }

    [Column("product_name")]
    public required string ProductName { get; set; }

    [Column("product_description")]
    public string? ProductDescription { get; set; }

    [Column("product_price")]
    public required float ProductPrice { get; set; }

    [Column("product_quantity")]
    public int ProductQuantity { get; set; }

    [Column("product_image")]
    public string? ProductImage { get; set; }

    [Column("product_color")]
    public string? ProductColor { get; set; }

    [Column("product_weight")]
    public float ProductWeight { get; set; }

    [Column("date_added")]
    public DateTime DateAdded { get; set; }

    [Column("date_modified")]
    public DateTime DateModified { get; set; }

    [Column("product_dimensions")]
    public string? ProductDimensions { get; set; }

    [Column("product_condition")]
    public string? ProductCondition { get; set; }
}