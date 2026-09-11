using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Data;
using URLShortener.utilities;
using Microsoft.EntityFrameworkCore;


namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Stats : ControllerBase
    {
        [HttpGet("{shortCode}")]

        public async Task<IActionResult> statistics([FromRoute] string shortCode, AppDbContext db)
        {
            var input = Utilities.Decode(shortCode);
            Clicks[] stats = await db.Clicks.Where(u => u.UrlId == input).ToArrayAsync();
            if (stats == null)
            {
                return Ok("No stats yet for this route");
            }

            return Ok(stats);
        }
    }
}
