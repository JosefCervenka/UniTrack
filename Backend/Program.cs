using Application.Middlewares;
using Application.Services;
using Application.Validations;
using FluentValidation;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<HashService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddScoped<ValidationMiddleware>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Not fount connection string 'DefaultConnection'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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

app.UseMiddleware<ValidationMiddleware>();
app.UseAuthorization();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();