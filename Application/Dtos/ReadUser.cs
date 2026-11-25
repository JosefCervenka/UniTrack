namespace Application.Dtos;

public class ReadUser
{
    public Guid Id { get; set; }
    
    public string Surname { get; set; }

    public string GivenName { get; set; }
    
    public string Email { get; set; }

    public string UserName { get; set; }
}