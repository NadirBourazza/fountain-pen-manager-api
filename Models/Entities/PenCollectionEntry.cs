using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Models.Entities;

public class PenCollectionEntry
{
    [Key]
    public Guid EntryId { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(50, ErrorMessage = "SKU cannot be longer than 50 characters.")]
    public Guid PenId { get; set; }
    
    [JsonIgnore]
    public virtual PenCatalogueEntry Pen { get; set; }
    
    [Required]
    [StringLength(50, ErrorMessage = "Color cannot be longer than 50 characters.")]
    public string Color { get; set; }
    
    [Required]
    [StringLength(20, ErrorMessage = "Nib size cannot be longer than 20 characters.")]
    public string NibSize { get; set; }
    
    [Required]
    [StringLength(20, ErrorMessage = "Nib material cannot be longer than 20 characters.")]
    public string NibMaterial { get; set; }
    
    [StringLength(20, ErrorMessage = "Nickname cannot be longer than 20 characters.")]
    public string? Nickname { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Purchase price must be a positive value.")]
    public int? PurchasePricePence { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Delivery fee must be a positive value.")]
    public int? DeliveryFeePence { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Import fee must be a positive value.")]
    public int? ImportFeePence { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Current value must be a positive value.")]
    public int? CurrentValuePence { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? PurchaseDate { get; set; }
}