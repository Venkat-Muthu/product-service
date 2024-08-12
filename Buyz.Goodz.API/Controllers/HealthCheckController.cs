using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Buyz.Goodz.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class HealthCheckController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealthAsync()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();

        if (healthReport.Status != HealthStatus.Healthy)
        {
            return StatusCode(500, "Health check failed");
        }

        return Ok("Health check OK");
    }
}