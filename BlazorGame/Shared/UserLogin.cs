using System.ComponentModel.DataAnnotations;

namespace BlazorGame.Shared;

public class UserLogin
{
    [Required(ErrorMessage = "Please enter a email")]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}
