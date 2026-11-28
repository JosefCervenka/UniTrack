using Application.Dtos;
using Domain.Enums;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Validations;

public class CreateLostItemValidator : AbstractValidator<CreateLostItem>
{
    private readonly AppDbContext _context;

    public CreateLostItemValidator(AppDbContext context)
    {
        _context = context;

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
            .MaximumLength(300).WithMessage("Lost location must not exceed 300 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.LostLocation), Reason = "MaxLength" });

        RuleFor(x => x.StatusId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("A valid status is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.StatusId), Reason = "Required" })
            .Must(statusId => statusId == (int)StatusEnum.Lost || statusId == (int)StatusEnum.Found)
                .WithMessage("New items can only have the Lost or Found status.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.StatusId), Reason = "InvalidStatus" })
            .MustAsync(async (statusId, cancellationToken) =>
                await _context.Status.AnyAsync(status => status.Id == statusId, cancellationToken))
                .WithMessage("The selected status does not exist.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.StatusId), Reason = "NotFound" });

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