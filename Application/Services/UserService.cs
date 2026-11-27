using System.Security.Claims;
using Application.Dtos;
using Domain;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateUser> _userCreateValidator;
    private readonly IValidator<LoginUser> _loginValidator;
    private readonly HashService _hashService;
    private readonly JwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmbeddingService _embeddingService;


    public UserService(AppDbContext context, IValidator<CreateUser> userCreateValidator, HashService hashService,
        JwtService jwtService, IValidator<LoginUser> loginValidator, IHttpContextAccessor httpContextAccessor,
        EmbeddingService embeddingService)
    {
        _context = context;
        _userCreateValidator = userCreateValidator;
        _loginValidator = loginValidator;
        _hashService = hashService;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _embeddingService = embeddingService;
    }

    public async Task<string> Login(LoginUser loginUser)
    {
        loginUser.Email = loginUser.Email?.Trim().ToLower() ?? string.Empty;
        
        await _loginValidator.ValidateAndThrowAsync(loginUser);
        
        return  _jwtService.GenerateToken(loginUser.Email, "USER");
    }

    public async Task<ReadUser?> GetUser(Guid id)
    {
        return _context!.User!.Select(x => new ReadUser
        {
            UserName = x!.UserName,
            Email = x!.Email,
            Surname = x!.Surname,
            GivenName = x!.GivenName,
            Id = x!.Id,
            
        })!.FirstOrDefault(x => x.Id == id);
    }

    public async Task<ReadUser?> GetCurrentUser()
    {
        var claims = _httpContextAccessor?.HttpContext?.User;

        var email = claims?.FindFirst(ClaimTypes.Email).Value ?? string.Empty;
        
        return _context!.User!.Select(x => new ReadUser
        {
            UserName = x!.UserName,
            Email = x!.Email,
            Surname = x!.Surname,
            GivenName = x!.GivenName,
            Id = x!.Id,
            
        })!.FirstOrDefault(x => x.Email == email);
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