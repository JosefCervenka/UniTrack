using Application.Dtos;
using FluentValidation;
using Application.Services;
using Domain;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Validations;

public class LoginUserValidator : AbstractValidator<LoginUser>
{
    public LoginUserValidator(AppDbContext context, HashService hashService)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters.");

        RuleFor(x => x)
            .MustAsync(async (login, cancellationToken) =>
            {
                User? user = await FindUserByEmailAsync(context, login.Email, cancellationToken);
                return user != null && hashService.VerifyPassword(login.Password, user.PasswordHash);
            })
            .When(x => !string.IsNullOrWhiteSpace(x.Email) && x.Email.Length <= 256 &&
                       !string.IsNullOrEmpty(x.Password) && x.Password.Length <= 128)
            .WithMessage("Invalid email or password.");
    }

    private static Task<User?> FindUserByEmailAsync(AppDbContext context, string email,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLower();
        return context.User.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
    }
}