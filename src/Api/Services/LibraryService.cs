using Api.Entities;
using Api.Interfaces.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Services;

public class LibraryService(
    ILibraryRepository libraryRepository,
    IMemoryCache cache,
    IUnitOfWork unitOfWork
)
{
    private readonly ILibraryRepository _libraryRepository = libraryRepository;
    private readonly IMemoryCache _cache = cache;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Library?> GetLibrary(int libraryId)
    {
        // Write-around cache: A ideia é que se o item não estiver no cache, devemos buscar no banco de dados e adicionar ao cache.
        if (!_cache.TryGetValue(libraryId, out Library? library))
        {
            library = await _libraryRepository.GetLibrary(libraryId);
            if (library != null)
            {
                _cache.Set(libraryId, library, TimeSpan.FromMinutes(10));
            }
        }
        return library;
    }

    public async Task<List<Library>> GetLibraries() => await _libraryRepository.GetLibraries();

    public async Task<int> Add(string name)
    {
        //Poderiamos optar por já salvar no também no cache.
        //Isso irá depender se a aplicação é mais leitura ou escrita.
        var library = new Library(name);
        _libraryRepository.Add(library);
        await _unitOfWork.SaveChangesAsync();
        return library.Id;
    }

    public async Task RemoveLibrary(int libraryId)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        _libraryRepository.RemoveLibrary(library);
        //Devemos invalidar o cache caso ele exista
        if (_cache.TryGetValue(libraryId, out _))
        {
            _cache.Remove(libraryId);
        }
        await _unitOfWork.SaveChangesAsync();
        return;
    }

    public async Task AddBook(int libraryId, List<(string title, string author)> books)
    {
        //Write-trough cache: A ideia é que sempre que adicionarmos um livro, devemos atualizar o cache.
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        books.ForEach(b => library.AddBook(b.title, b.author));
        _cache.Set(libraryId, library, TimeSpan.FromMinutes(10));
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveBook(int libraryId, int bookId)
    {
        // Write-around cache (invalidação): A ideia é que sempre que removemos um livro, devemos invalidar o cache.
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        library.RemoveBook(bookId);
        if (_cache.TryGetValue(libraryId, out _))
        {
            _cache.Remove(libraryId);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task BorrowBook(int libraryId, int bookId)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        library.BorrowBook(bookId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReturnBook(int libraryId, int bookId)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        library.ReturnBook(bookId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<Book>> GetBooks(int libraryId, bool borrowed)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        return library.Books.Where(b => b.IsAvailable == !borrowed).ToList();
    }
}
