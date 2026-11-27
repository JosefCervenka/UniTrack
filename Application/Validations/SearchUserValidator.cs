using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;

namespace Application.Validations;

public class SearchUserValidator : AbstractValidator<SearchUser>
{
    private readonly AppDbContext _context;

    public SearchUserValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User id is required.")
            .WithErrorCode("400")
            .WithState(x => new { HttpStatus = 400, Field = nameof(x.Id), Reason = "Required" })
            .Must(id => _context.User.Any(x => x.Id == id))
            .WithMessage("user with the given id does not exist.")
            .WithErrorCode("404")
            .WithState(x => new { HttpStatus = 404, Field = nameof(x.Id), Reason = "NotFound" });
    }
}