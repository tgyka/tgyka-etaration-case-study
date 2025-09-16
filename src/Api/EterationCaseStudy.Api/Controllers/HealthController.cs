using EterationCaseStudy.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EterationCaseStudy.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public HealthController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            try
            {
                await _db.Database.CanConnectAsync();
                return Ok("Healthy");
            }
            catch
            {
                return StatusCode(503, "Degraded");
            }
        }
    }
}

