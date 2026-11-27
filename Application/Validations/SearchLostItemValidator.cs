using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;

namespace Application.Validations;

public class SearchLostItemValidator : AbstractValidator<SearchLostItem>
{
    private readonly AppDbContext _context;
    
    public SearchLostItemValidator(AppDbContext context)
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
    }
}