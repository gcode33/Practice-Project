using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleBookCatalog.Application.Interfaces;
using SimpleBookCatalog.Domain.Entities;
using SimpleBookCatalog.Infrastructure.Context;
using System.Text.Json;

namespace SimpleBookCatalog.Infrastructure.Repositories
{
    public class BookRepo : IBookRepo
    {
        private readonly IDbContextFactory<SimpleBookCatalogDbContext> _contextFactory;
        private readonly ILogger<BookRepo> _logger;

        public BookRepo(IDbContextFactory<SimpleBookCatalogDbContext> contextFactory, ILogger<BookRepo> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task AddAsync(Books book)
        {
            _logger.LogInformation("Starting AddAsync method");
            _logger.LogInformation("Book details: {BookDetails}",
                JsonSerializer.Serialize(new
                {
                    book.Title,
                    book.Author,
                    book.PublicationDate,
                    book.Category
                }));

            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                _logger.LogInformation("Database context created");

                // Verify context state
                _logger.LogInformation("Database path: {DbPath}",
                    context.Database.GetConnectionString());

                // Check if the Books table exists
                try
                {
                    var tableExists = await context.Books.AnyAsync();
                    _logger.LogInformation("Books table exists and is accessible");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking Books table");
                }

                // Add the book
                var entry = await context.Books.AddAsync(book);
                _logger.LogInformation("Book added to context. Entity State: {State}",
                    entry.State);

                // Try to save
                var saveResult = await context.SaveChangesAsync();
                _logger.LogInformation("SaveChanges completed. Records affected: {Count}",
                    saveResult);

                // Verify the save
                var savedBook = await context.Books
                    .FirstOrDefaultAsync(b => b.Title == book.Title && b.Author == book.Author);

                if (savedBook != null)
                {
                    _logger.LogInformation("Book verified in database. ID: {Id}", savedBook.Id);
                }
                else
                {
                    _logger.LogWarning("Book not found in database after save!");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while saving book");
                _logger.LogError("Inner exception: {InnerException}",
                    ex.InnerException?.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while saving book");
                throw;
            }
        }
    }
}