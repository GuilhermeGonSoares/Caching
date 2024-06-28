using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api")]
public class HealthCheckController : ControllerBase
{
    [HttpGet()]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
}
