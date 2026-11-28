namespace Application.Dtos;

public class ReadLostItem
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public string? LostLocation { get; set; }

    public required string GivenName { get; set; }

    public required string Surname { get; set; }

    public required int StatusId { get; set; }

    public required string Status { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required Guid OwnerId { get; set; }
}