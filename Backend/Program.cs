using Application.Middlewares;
using Application.Services;
using Application.Validations;
using Backend.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using SmartComponents.LocalEmbeddings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

builder.Services.AddControllers();

builder.Services
    .AddAuthentication("Jwt")
    .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("Jwt", _ => { });
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<LostItemService>();

builder.Services.AddSingleton<HashService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<LocalEmbedder>();
builder.Services.AddTransient<JwtMiddleware>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Not fount connection string 'DefaultConnection'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsql => npgsql.UseVector()));

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("StartAngular"))
{
    var angularDevServer = new AngularDevServer(app.Logger);
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            angularDevServer.Start();
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "Could not start the Angular development server.");
        }
    });
    app.Lifetime.ApplicationStopping.Register(angularDevServer.Stop);
}
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();