using Api.Entities;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/libraries")]
public class LibraryController(LibraryService libraryService) : ControllerBase
{
    public record CreateLibraryRequest(string Name);

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
}
