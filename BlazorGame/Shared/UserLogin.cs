using System.ComponentModel.DataAnnotations;

namespace BlazorGame.Shared;

public class UserLogin
{
    [Required(ErrorMessage = "Please enter a user name")]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}
