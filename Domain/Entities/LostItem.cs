using Pgvector;

namespace Domain;

public class LostItem : BaseEntity
{
    public Guid OwnerId { get; set; }
    
    public User Owner { get; set; }

    public string Name { get; set; }

    public string Desctiption { get; set; }

    public string? LostLocation { get; set; }

    public int StatusId { get; set; }
    
    public Status Status { get; set; }
    
    public byte[]? Image { get; set; }

    public DateTime CreatedAt  { get; set; }
    
    public Vector? Embedding { get; set; }
}