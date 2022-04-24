namespace BlazorGame.Shared;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public byte[] Password { get; set; }

    public byte[] Salt { get; set; }

    public int Bananas { get; set; }

    public DateTime DateOfBirth { get; set; }

    public bool IsConfirmed { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastUpdateDate { get; set; }

    public List<UserUnit> Units { get; set; }

    public int Battles { get; set; }

    public int Victories { get; set; }

    public int Defeats { get; set; }
}
