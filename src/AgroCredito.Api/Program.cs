using AgroCredito.Infrastructure;
using AgroCredito.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default não configurada (use user-secrets).");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgroCreditoDbContext>();
    db.Database.Migrate();
}

// Rota provisória só para conferir que o banco e o seed funcionam (será substituída na Etapa 4)
app.MapGet("/", async (AgroCreditoDbContext db) => new
{
    api = "AgroCredito",
    oportunidades = await db.Oportunidades.CountAsync()
});

app.Run();
