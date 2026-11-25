namespace Application.Dtos;

public class CreateLostItem
{
    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string LostLocation { get; set; }

    public byte[]? Image { get; set; }
}