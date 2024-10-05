using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

public class User
{
    [Key]
    public Guid UserId { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    public string Email { get; set; }
    
    [Required]
    public string Salt { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
}