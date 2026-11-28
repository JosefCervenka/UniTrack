namespace Application.Dtos;

public class UpdateLostItem
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string LostLocation { get; set; }

    public required int StatusId { get; set; }

    public byte[]? Image { get; set; }
}