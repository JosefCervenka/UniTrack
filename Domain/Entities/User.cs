namespace Domain;

public class User : BaseEntity
{
    public string UserName { get; set; }

    public string GivenName { get; set; }

    public string Surname { get; set; }
    
    public string Email { get; set; }

    public string Contact { get; set; }
    
    public byte[] PasswordHash { get; set; }
    
    public byte[] PasswordSalt { get; set; }
}