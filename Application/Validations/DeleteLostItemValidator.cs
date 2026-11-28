using System.Security.Claims;
using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Validations;

public class DeleteLostItemValidator : AbstractValidator<SearchLostItem>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteLostItemValidator(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Item id is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Id), Reason = "Required" })
            .MustAsync(async (id, cancellationToken) =>
                await _context.LostItem.AnyAsync(item => item.Id == id, cancellationToken))
                .WithMessage("Item with the given id does not exist.")
                .WithErrorCode("404")
                .WithState(x => new { HttpStatus = 404, Field = nameof(x.Id), Reason = "NotFound" })
            .MustAsync(OwnedByCurrentUserAsync)
                .WithMessage("You are not allowed to delete this item.")
                .WithErrorCode("403")
                .WithState(x => new { HttpStatus = 403, Field = nameof(x.Id), Reason = "Forbidden" });
    }

    private async Task<bool> OwnedByCurrentUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return await _context.LostItem
            .AnyAsync(item => item.Id == id && item.Owner.Email == email, cancellationToken);
    }
}
