using Application.Dtos;
using Application.Services;
using Application.Validations;
using Domain;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class LoginUserValidatorTests
{
    private readonly AppDbContext _context = new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private readonly HashService _hashService = new();

    private LoginUserValidator Validator => new(_context, _hashService);

    [Fact]
    public async Task ValidCredentialsPassValidation()
    {
        _context.User.Add(new User { Email = "user@example.com", PasswordHash = _hashService.HashPassword("legacy"), UserName = "user", GivenName = "User", Surname = "Example", PhoneNumber = "123" });
        await _context.SaveChangesAsync();
        var result = await Validator.ValidateAsync(new LoginUser { Email = "USER@example.com", Password = "legacy" });
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("missing@example.com", "legacy")]
    [InlineData("user@example.com", "wrong")]
    public async Task InvalidCredentialsAreRejected(string email, string password)
    {
        _context.User.Add(new User { Email = "user@example.com", PasswordHash = _hashService.HashPassword("legacy"), UserName = "user", GivenName = "User", Surname = "Example", PhoneNumber = "123" });
        await _context.SaveChangesAsync();
        var result = await Validator.ValidateAsync(new LoginUser { Email = email, Password = password });
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Invalid email or password.");
    }

    [Theory]
    [InlineData("", "password", "Email is required.")]
    [InlineData("not-an-email", "password", "A valid email address is required.")]
    [InlineData("user@example.com", "", "Password is required.")]
    public async Task InvalidCredentialsHaveHelpfulErrors(string email, string password, string message)
    {
        var result = await Validator.ValidateAsync(new LoginUser { Email = email, Password = password });
        Assert.Contains(result.Errors, error => error.ErrorMessage == message);
    }

    [Fact]
    public async Task OverlongFieldsAreRejected()
    {
        var result = await Validator.ValidateAsync(new LoginUser
        {
            Email = new string('a', 250) + "@example.com",
            Password = new string('a', 129)
        });
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Email must not exceed 256 characters.");
        Assert.Contains(result.Errors, error => error.ErrorMessage == "Password must not exceed 128 characters.");
    }
}