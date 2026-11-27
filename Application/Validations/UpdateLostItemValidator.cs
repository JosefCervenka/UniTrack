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

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Item id is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Id), Reason = "Required" })
            .Must(id => _context.LostItem.Any(x => x.Id == id))
                .WithMessage("Item with the given id does not exist.")
                .WithErrorCode("404")
                .WithState(x => new { HttpStatus = 404, Field = nameof(x.Id), Reason = "NotFound" });

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Name), Reason = "Required" })
            .MaximumLength(150).WithMessage("Item name must not exceed 150 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Name), Reason = "MaxLength" });

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Item description is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Description), Reason = "Required" })
            .MaximumLength(1000).WithMessage("Item description must not exceed 1000 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Description), Reason = "MaxLength" });

        RuleFor(x => x.LostLocation)
            .NotEmpty().WithMessage("Lost location is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.LostLocation), Reason = "Required" })
            .MaximumLength(300).WithMessage("Lost location must not exceed 300 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.LostLocation), Reason = "MaxLength" });

        RuleFor(x => x.Image)
            .Must(img => img is null || img.Length > 0)
                .WithMessage("Image cannot be empty.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Image), Reason = "EmptyFile" })
            .Must(img => img is null || img.Length <= 5 * 1024 * 1024)
                .WithMessage("Image size must not exceed 5 MB.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Image), Reason = "FileTooLarge" });
    }
}