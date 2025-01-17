using Microsoft.EntityFrameworkCore;
using SimpleBookCatalog.Infrastructure.Context;
using SimpleBookCatalogs.Components;
using MudBlazor.Services;
using SimpleBookCatalog.Application.Interfaces;
using SimpleBookCatalog.Infrastructure.Repositories;
using SimpleBookCatalog.Domain.Entities;
using SimpleBookCatalog.Domain.Enum;

var builder = WebApplication.CreateBuilder(args);
// Add this right after creating the builder
builder.Configuration["ConnectionStrings:SimpleBookCatalogsConnection"] =
    "Server=(localdb)\\mssqllocaldb;Database=SimpleBookCatalogs;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";


// Add these diagnostic lines
var configuration = builder.Configuration;



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
});


builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
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


app.MapGet("/api/test-book-add", async (IDbContextFactory<SimpleBookCatalogDbContext> contextFactory) =>
{
    try
    {
        using var context = await contextFactory.CreateDbContextAsync();

        var testBook = new Books
        {
            Title = "Test Book",
            Author = "Test Author",
            PublicationDate = DateTime.Now,
            Category = Category.Science
        };

        context.Books.Add(testBook);
        var result = await context.SaveChangesAsync();

        return Results.Ok(new { Message = "Test book added", AffectedRows = result });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error adding test book: {ex.Message}");
    }
});
app.Run();