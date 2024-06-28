using Api.Entities;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/libraries")]
public class LibraryController(LibraryService libraryService) : ControllerBase
{
    public record CreateLibraryRequest(string Name);

    public record CreateBookRequest(string Title, string Author);

    private readonly LibraryService _libraryService = libraryService;

    [HttpGet]
    public async Task<ActionResult<List<Library>>> GetLibraries() =>
        await _libraryService.GetLibraries();

    [HttpGet("{libraryId}")]
    public async Task<ActionResult<Library>> GetLibrary(Guid libraryId)
    {
        var library = await _libraryService.GetLibrary(libraryId);

        if (library is null)
        {
            return NotFound();
        }

        return library;
    }

    [HttpPost]
    public async Task<ActionResult<Library>> AddLibrary(CreateLibraryRequest request)
    {
        var libraryId = await _libraryService.Add(request.Name);
        var library = await _libraryService.GetLibrary(libraryId);

        return CreatedAtAction(nameof(GetLibrary), new { libraryId }, library);
    }

    [HttpDelete("{libraryId}")]
    public async Task<ActionResult> RemoveLibrary(Guid libraryId)
    {
        try
        {
            await _libraryService.RemoveLibrary(libraryId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{libraryId}/books")]
    public async Task<ActionResult> AddBook(Guid libraryId, List<CreateBookRequest> books)
    {
        try
        {
            await _libraryService.AddBook(
                libraryId,
                books.Select(b => (b.Title, b.Author)).ToList()
            );
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{libraryId}/books/{bookId}")]
    public async Task<ActionResult> RemoveBook(Guid libraryId, Guid bookId)
    {
        try
        {
            await _libraryService.RemoveBook(libraryId, bookId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{libraryId}/books/{bookId}/borrow")]
    public async Task<ActionResult> BorrowBook(Guid libraryId, Guid bookId)
    {
        try
        {
            await _libraryService.BorrowBook(libraryId, bookId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{libraryId}/books/{bookId}/return")]
    public async Task<ActionResult> ReturnBook(Guid libraryId, Guid bookId)
    {
        try
        {
            await _libraryService.ReturnBook(libraryId, bookId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{libraryId}/books")]
    public async Task<ActionResult<List<Book>>> GetBooks(Guid libraryId, bool borrowed)
    {
        try
        {
            return await _libraryService.GetBooks(libraryId, borrowed);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
