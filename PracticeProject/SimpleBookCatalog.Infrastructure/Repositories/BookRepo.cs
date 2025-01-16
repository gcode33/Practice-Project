﻿

using Microsoft.EntityFrameworkCore;
using SimpleBookCatalog.Domain.Entities;
using SimpleBookCatalog.Application.Interfaces;
using SimpleBookCatalog.Infrastructure.Context;

namespace SimpleBookCatalog.Infrastructure.Repositories
{
    public class BookRepo : IBookRepo
    {
        private readonly SimpleBookCatalogDbContext Context;
        public BookRepo(IDbContextFactory<SimpleBookCatalogDbContext> factory) 
        {
            Context = factory.CreateDbContext();
        }

        public async Task AddAsync(Books book)
        {
            Context.Books.Add(book);
            await Context.SaveChangesAsync();
        }
    }
}
