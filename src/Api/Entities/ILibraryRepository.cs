namespace Api.Entities;

public interface ILibraryRepository
{
    Task<Library?> GetLibrary(int libraryId);
    Task<List<Library>> GetLibraries();
    void Add(Library library);
    void RemoveLibrary(Library library);
}
