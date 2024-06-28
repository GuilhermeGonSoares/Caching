using Api.Database;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public class LibraryRepository(ApplicationDbContext context) : ILibraryRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Library?> GetLibrary(int libraryId) =>
        await _context.Libraries.Include(l => l.Books).FirstOrDefaultAsync(l => l.Id == libraryId);

    public async Task<List<Library>> GetLibraries() =>
        await _context.Libraries.AsNoTracking().Include(l => l.Books).ToListAsync();

    public void Add(Library library) => _context.Libraries.Add(library);

    public void RemoveLibrary(Library library) => _context.Libraries.Remove(library);
}
