using System.Security.Claims;
using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Validations;

public class UpdateLostItemValidator : AbstractValidator<UpdateLostItem>
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateLostItemValidator(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Item id is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Id), Reason = "Required" })
            .MustAsync(async (id, cancellationToken) =>
                await _context.LostItem.AnyAsync(x => x.Id == id, cancellationToken))
                .WithMessage("Item with the given id does not exist.")
                .WithErrorCode("404")
                .WithState(x => new { HttpStatus = 404, Field = nameof(x.Id), Reason = "NotFound" })
            .MustAsync(OwnedByCurrentUserAsync)
                .WithMessage("You are not allowed to update this item.")
                .WithErrorCode("403")
                .WithState(x => new { HttpStatus = 403, Field = nameof(x.Id), Reason = "Forbidden" });

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