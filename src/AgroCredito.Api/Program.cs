using AgroCredito.Application;
using AgroCredito.Application.Services;
using AgroCredito.Infrastructure;
using AgroCredito.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default não configurada (use user-secrets).");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgroCreditoDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", async (OportunidadeService oportunidades) => await oportunidades.ListAsync(null));

app.Run();
