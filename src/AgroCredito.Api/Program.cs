using AgroCredito.Api.Endpoints;
using AgroCredito.Api.Errors;
using AgroCredito.Application;
using AgroCredito.Infrastructure;
using AgroCredito.Infrastructure.Auth;
using AgroCredito.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default não configurada (use user-secrets).");

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException("Jwt:Key não configurada ou com menos de 32 caracteres (use user-secrets).");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddJwtAuthentication(jwt);
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgroCreditoDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => new { api = "AgroCredito", status = "online" });
app.MapAuthEndpoints();
app.MapProdutorEndpoints();

app.Run();
