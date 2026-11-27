using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;

namespace Application.Validations;

public class SearchLostItemPictureValidator : AbstractValidator<SearchLostItem>
{
    private readonly AppDbContext _context;
    
    public SearchLostItemPictureValidator(AppDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Item id is required.")
            .WithErrorCode("400")
            .WithState(x => new { HttpStatus = 400, Field = nameof(x.Id), Reason = "Required" })
            .Must(id => _context.LostItem.Any(x => x.Id == id && x.Image != null && x.Image.Length > 0))
            .WithMessage("Item picture was not found.")
            .WithErrorCode("404")
            .WithState(x => new { HttpStatus = 404, Field = nameof(x.Id), Reason = "PictureNotFound" });
    }
}