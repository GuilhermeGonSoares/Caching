namespace Api.Entities;

public interface ILibraryRepository
{
    Task<Library?> GetLibrary(Guid libraryId);
    Task<List<Library>> GetLibraries();
    void Add(Library library);
    void RemoveLibrary(Library library);
}
