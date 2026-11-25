using Application.Services;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class ValidationMiddleware : IMiddleware
{
    private readonly JwtService _jwtService;
    
    public ValidationMiddleware(JwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Cookies.TryGetValue("jwt", out var jwt))
        {
           var claims = await  _jwtService.ValidateToken(jwt);

           if (claims is not null)
           {
               context.User = claims;
           }
        }
        
        await next.Invoke(context);
    }
}