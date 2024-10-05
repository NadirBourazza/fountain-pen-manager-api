using System.ComponentModel.DataAnnotations;
using Models.Enums;
using Newtonsoft.Json;

namespace Models.Entities;

public class PenCatalogueEntry
{
    [Key]
    [Required]
    [StringLength(50, ErrorMessage = "SKU cannot be longer than 50 characters.")]
    public Guid PenId { get; init; } 
    
    [Required]
    [StringLength(100, ErrorMessage = "Manufacturer cannot be longer than 100 characters.")]
    public string Manufacturer { get; init; }
    
    [Required]
    [StringLength(100, ErrorMessage = "Model cannot be longer than 100 characters.")]
    public string Model { get; init; }
}