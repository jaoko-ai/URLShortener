using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using URLShortener.utilities;

namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Query(AppDbContext db) : ControllerBase
    {
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> Get([FromRoute] string shortCode)
        {
            Console.WriteLine($"Parameter reached: {shortCode}");
            // Validate

            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return BadRequest("bad Request.");
            }

            Urls record;
            try
            {

                record = await db.Urls
                .Where(u => u.ShortCode == shortCode).SingleAsync();
            }
            catch
            {
                return Ok(new { ShortCode = $"{shortCode}: Not found" });
            }
            record.ClickCount++;


            // Get click analytics
            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            string userAgent = HttpContext.Request.Headers.UserAgent.ToString();
            string referrer = HttpContext.Request.Headers.Referer.ToString();
            string language = HttpContext.Request.Headers.AcceptLanguage.ToString();
            Clicks analytics = new Clicks
            {
                UrlId = record.Id,
                referrer = referrer,
                UserAgent = userAgent,
                IpAddress = clientIp,
                Urls = record
            };
            await db.Clicks.AddAsync(analytics);
            await db.SaveChangesAsync();
            return Ok(new { OriginalUrl = record.OriginalUrl });
        }
    }
}
