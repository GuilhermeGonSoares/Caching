using Api.Entities;
using Api.UnitOfWork;

namespace Api.Services;

public class LibraryService(ILibraryRepository libraryRepository, IUnitOfWork unitOfWork)
{
    private readonly ILibraryRepository _libraryRepository = libraryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Library?> GetLibrary(int libraryId) =>
        await _libraryRepository.GetLibrary(libraryId);

    public async Task<List<Library>> GetLibraries() => await _libraryRepository.GetLibraries();

    public async Task<int> Add(string name)
    {
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
        await _unitOfWork.SaveChangesAsync();
        return;
    }

    public async Task AddBook(int libraryId, List<(string title, string author)> books)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        books.ForEach(b => library.AddBook(b.title, b.author));
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveBook(int libraryId, int bookId)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        library.RemoveBook(bookId);
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
