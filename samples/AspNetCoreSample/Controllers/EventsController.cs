using AspNetCoreSample.Events;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Controllers;

[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    [HttpGet("pageViewCount")]
    public IActionResult Count()
    {
        return Ok(new { PageViewEventHandler.Count });
    }
}
