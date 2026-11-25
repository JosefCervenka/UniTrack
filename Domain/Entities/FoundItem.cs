namespace Domain;

public class FoundItem : BaseEntity
{
    public Guid FinderId { get; set; }
    
    public User Finder { get; set; }

    public string Name { get; set; }

    public string Desctiption { get; set; }

    public string LostLocation { get; set; }

    public int StatusId { get; set; }
    
    public Status Status { get; set; }
    
    public byte[] Image { get; set; }
}