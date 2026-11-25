using Application.Dtos;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Validations;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    private readonly AppDbContext _context;

    public CreateUserValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.UserName), Reason = "Required" })
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.UserName), Reason = "MinLength" })
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.UserName), Reason = "MaxLength" })
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, hyphens, and underscores.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.UserName), Reason = "InvalidFormat" })
            .MustAsync(BeUniqueUserName).WithMessage("Username is already taken.")
                .WithErrorCode("409")
                .WithState(x => new { HttpStatus = 409, Field = nameof(x.UserName), Reason = "Duplicate" });

        RuleFor(x => x.GivenName)
            .NotEmpty().WithMessage("Given name is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.GivenName), Reason = "Required" })
            .MaximumLength(100).WithMessage("Given name must not exceed 100 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.GivenName), Reason = "MaxLength" });

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Surname), Reason = "Required" })
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Surname), Reason = "MaxLength" });

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Email), Reason = "Required" })
            .EmailAddress().WithMessage("A valid email address is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Email), Reason = "InvalidEmail" })
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Email), Reason = "MaxLength" })
            .MustAsync(BeUniqueEmail).WithMessage("Email is already registered.")
                .WithErrorCode("409")
                .WithState(x => new { HttpStatus = 409, Field = nameof(x.Email), Reason = "Duplicate" });

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.PhoneNumber), Reason = "Required" })
            .Matches(@"^\+?[0-9\s\-]{7,20}$").WithMessage("A valid phone number is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.PhoneNumber), Reason = "InvalidFormat" });

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Password), Reason = "Required" })
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Password), Reason = "MinLength" })
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Password), Reason = "MaxLength" })
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Password), Reason = "MissingUppercase" })
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Password), Reason = "MissingLowercase" })
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.Password), Reason = "MissingDigit" });

        RuleFor(x => x.PasswordAgain)
            .NotEmpty().WithMessage("Password confirmation is required.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.PasswordAgain), Reason = "Required" })
            .Equal(x => x.Password).WithMessage("Passwords do not match.")
                .WithErrorCode("400")
                .WithState(x => new { HttpStatus = 400, Field = nameof(x.PasswordAgain), Reason = "Mismatch" });
    }

    private async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        return !await _context.User
            .AnyAsync(x => x.UserName == userName, cancellationToken);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.User
            .AnyAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);
    }
}