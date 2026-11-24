using System.ComponentModel.DataAnnotations;

namespace Domain;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}