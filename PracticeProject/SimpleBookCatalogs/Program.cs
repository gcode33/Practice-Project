using Microsoft.EntityFrameworkCore;
using SimpleBookCatalog.Infrastructure.Context;
using SimpleBookCatalogs.Components;
using MudBlazor.Services;
using SimpleBookCatalog.Application.Interfaces;
using SimpleBookCatalog.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add this right after creating the builder
builder.Configuration["ConnectionStrings:SimpleBookCatalogsConnection"] =
    "Server=(localdb)\\mssqllocaldb;Database=SimpleBookCatalogs;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";
// Add these diagnostic lines
var configuration = builder.Configuration;
foreach (var c in configuration.GetSection("ConnectionStrings").GetChildren())
{
    Console.WriteLine($"Found connection string: {c.Key} = {c.Value}");
}
var targetConnection = configuration.GetConnectionString("SimpleBookCatalogsConnection");
Console.WriteLine($"Target connection string: {targetConnection}");
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddDbContextFactory<SimpleBookCatalogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SimpleBookCatalogsConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'SimpleBookCatalogsConnection' not found.");
    }
    Console.WriteLine($"Configuring DbContext with connection string: {connectionString}");
    options.UseSqlServer(connectionString);
});// Add after your builder.Services.AddDbContextFactory line
var connectionString = builder.Configuration.GetConnectionString("SimpleBookCatalogsConnection");
Console.WriteLine($"[Startup] Using connection string: {connectionString}");
// Add this near the top of your Program.cs, after var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
builder.Services.AddScoped<IBookRepo, BookRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
// Add this to your Program.cs before app.Run() this is to check if the connection to the database is good go to the api endpoint in the browser to see
app.MapGet("/api/test-db", async (IDbContextFactory<SimpleBookCatalogDbContext> contextFactory, ILogger<Program> logger) =>
{
    try
    {
        using var context = await contextFactory.CreateDbContextAsync();
        var canConnect = await context.Database.CanConnectAsync();
        if (!canConnect)
        {
            var connectionString = context.Database.GetConnectionString();
            logger.LogError("Failed to connect to database. Connection string: {ConnectionString}", connectionString);
            return Results.Problem($"Cannot connect to database. Connection string used: {connectionString}");
        }
        return Results.Ok(new { Connected = canConnect });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error testing database connection");
        return Results.Problem($"Database connection test failed with error: {ex.Message}");
    }
});
app.Run();