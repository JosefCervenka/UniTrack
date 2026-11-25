using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;

namespace Application.Validations;

public class UpdateLostItemValidator : AbstractValidator<UpdateLostItem>
{
    private readonly AppDbContext _context;
    
    public UpdateLostItemValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x =>x.Id)
            .NotEmpty().WithMessage("Item id is required.")
            .Must(id => _context.LostItem.Any(li => li.Id == id))
            .WithMessage("Item with the given id does not exist.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(150).WithMessage("Item name must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Item description is required.")
            .MaximumLength(1000).WithMessage("Item description must not exceed 1000 characters.");

        RuleFor(x => x.LostLocation)
            .NotEmpty().WithMessage("Lost location is required.")
            .MaximumLength(300).WithMessage("Lost location must not exceed 300 characters.");

        RuleFor(x => x.Image)
            .Must(img => img is null || img.Length > 0)
            .WithMessage("Image cannot be empty.")
            .Must(img => img is null || img.Length <= 5 * 1024 * 1024)
            .WithMessage("Image size must not exceed 5 MB.");
    }
}