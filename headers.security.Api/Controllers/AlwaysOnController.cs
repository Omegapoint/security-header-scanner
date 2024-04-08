using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace headers.security.Api.Controllers;

/// <summary>
/// When Azure tries to check if site is up it does so using the User-Agent "alwayson"
/// </summary>
[ApiController]
public class AlwaysOnController : Controller
{
    private const string AlwaysOnUserAgent = "alwayson";

    [HttpGet("/")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult AlwaysOn()
    {
        if (Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgent))
        {
            if (userAgent.Any(s => s.Equals(AlwaysOnUserAgent, StringComparison.OrdinalIgnoreCase)))
            {
                return NoContent();
            }
        }

        // If the requests are not from the Always On functionality,
        // return a 404 Not Found
        return NotFound();
    }
}