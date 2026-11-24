using Application.Dtos;
using Domain;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateUser> _userCreateValidator;
    private readonly IValidator<LoginUser> _loginValidator;
    private readonly HashService _hashService;
    private readonly JwtService _jwtService;


    public UserService(AppDbContext context, IValidator<CreateUser> userCreateValidator, HashService hashService,
        JwtService jwtService, IValidator<LoginUser> loginValidator)
    {
        _context = context;
        _userCreateValidator = userCreateValidator;
        _loginValidator = loginValidator;
        _hashService = hashService;
        _jwtService = jwtService;
    }

    public async Task<string> Login(LoginUser loginUser)
    {
        loginUser.Email = loginUser.Email.ToLower().Trim();
        await _loginValidator.ValidateAndThrowAsync(loginUser);
        return await _jwtService.GenerateToken(loginUser.Email, "USER");
    }

    public async Task CreateUser(CreateUser createUser)
    {
        await _userCreateValidator.ValidateAndThrowAsync(createUser);
        var hash = _hashService.HashPassword(createUser.Password);
        var user = new User
        {
            UserName = createUser.UserName.Trim(),
            GivenName = createUser.GivenName.Trim(),
            Surname = createUser.Surname.Trim(),
            PasswordHash = hash,
            PhoneNumber = createUser.PhoneNumber.Trim(),
            Email = createUser.Email.ToLower().Trim(),
        };

        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}