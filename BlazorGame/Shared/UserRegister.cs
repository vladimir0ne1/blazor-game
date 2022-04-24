using System.ComponentModel.DataAnnotations;

namespace BlazorGame.Shared;

public class UserRegister
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, StringLength(16, ErrorMessage = "Your user name is too long (16 characters max).")]
    public string UserName { get; set; }

    public string Details { get; set; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    public int StartUnitId { get; set; } = 1;

    /// <summary>
    /// How many bananas does user want.
    /// </summary>
    [Range(0, 1000, ErrorMessage = "Please choose a number between 0 and 1000")]
    public int Bananas { get; set; } = 100;

    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Only confirmed users can play!")]
    public bool IsConfirmed { get; set; } = true;
}
