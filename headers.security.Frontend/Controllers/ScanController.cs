using headers.security.Common;
using headers.security.Common.Domain;
using headers.security.Scanner;
using Microsoft.AspNetCore.Mvc;

namespace headers.security.Frontend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScanController(ILogger<ScanController> logger, Worker worker) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ScanRequestContract req, CancellationToken cancellationToken)
    {
        try
        {
            var scanStart = DateTime.UtcNow;

            var conf = new CrawlerConfiguration
            {
                FollowRedirects = req.FollowRedirects,
                CancellationToken = cancellationToken
            };
            
            var results = await worker.PerformScan(conf, req.Target);

            return Ok(ScanResultsContract.From(scanStart, req, results));
        }
        catch (ScannerException e)
        {
            // TODO: these are errors that the UI knows about, but maybe log something here for monitoring?
            return BadRequest(new ScanError
            {
                Message = e.Message,
                Origin = e.Origin,
            });
        }
        catch (Exception e)
        {
            // TODO: should add app insights for tracking when this happens
            logger.LogTrace(e, "Unrecoverable error");

            return StatusCode(StatusCodes.Status500InternalServerError, new ScanError
            {
                Message = "Unrecoverable error",
                Origin = ErrorOrigin.Other
            });
        }
    }
}