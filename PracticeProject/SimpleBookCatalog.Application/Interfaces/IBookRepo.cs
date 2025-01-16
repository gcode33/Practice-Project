

using SimpleBookCatalog.Domain.Entities;

namespace SimpleBookCatalog.Application.Interfaces
{
    public interface IBookRepo
    {
         Task AddAsync(Books book);

    }
}
