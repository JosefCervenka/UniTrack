using Application.Services;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class ValidationMiddlewares : IMiddleware
{
    public ValidationMiddlewares(JwtService jwtService)
    {
        
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next.Invoke(context);
    }
}