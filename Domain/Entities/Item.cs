namespace Domain;

public class Item : BaseEntity
{
    public Guid OwnerId { get; set; }
    
    public User Owner { get; set; }

    public string Name { get; set; }

    public string Desctiption { get; set; }

    public string LostLocation { get; set; }

    public string Status { get; set; }
}