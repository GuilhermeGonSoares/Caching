using Api.Entities;

namespace Api.Services;

public class LibraryService(ILibraryRepository libraryRepository)
{
    private readonly ILibraryRepository _libraryRepository = libraryRepository;

    public async Task<Library?> GetLibrary(Guid libraryId) =>
        await _libraryRepository.GetLibrary(libraryId);

    public async Task<List<Library>> GetLibraries() => await _libraryRepository.GetLibraries();

    public async Task<Guid> Add(string name)
    {
        var library = new Library(name);
        _libraryRepository.Add(library);
        await _libraryRepository.SaveChangesAsync();
        return library.Id;
    }

    public async Task RemoveLibrary(Guid libraryId)
    {
        var library =
            await _libraryRepository.GetLibrary(libraryId)
            ?? throw new InvalidOperationException("Library not found");
        _libraryRepository.RemoveLibrary(library);
        await _libraryRepository.SaveChangesAsync();
        return;
    }

    public Task SaveChangesAsync() => _libraryRepository.SaveChangesAsync();
}
