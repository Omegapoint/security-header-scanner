using headers.security.Api.Contracts;
using headers.security.Common;
using headers.security.Common.Domain;
using headers.security.Scanner;
using headers.security.Scanner.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace headers.security.Api.Controllers;

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
                TargetKind = req.Kind,
                CancellationToken = cancellationToken
            };
            
            var results = await worker.PerformScan(conf, req.Target);

            return Ok(ScanResultsContract.From(scanStart, req, results));
        }
        catch (ScannerException e)
        {
            return BadRequest(new ScanError
            {
                Message = e.Message,
                Origin = e.Origin,
            });
        }
        catch (Exception e)
        {
            logger.LogTrace(e, "Unrecoverable error");

            return StatusCode(StatusCodes.Status500InternalServerError, new ScanError
            {
                Message = "Unrecoverable error",
                Origin = ErrorOrigin.Other
            });
        }
    }
}