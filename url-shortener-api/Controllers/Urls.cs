using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Entities;
using URLShortener.Data;
using Microsoft.EntityFrameworkCore;
using URLShortener.utilities;

namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Url(AppDbContext db) : ControllerBase
    {
        public record UrlRequest(string LongUrl, Guid? UserId);
        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten(UrlRequest request)
        {
            if (string.IsNullOrEmpty(request.LongUrl))
            {
                return BadRequest("Bad URL");
            }

            if (!Uri.TryCreate(request.LongUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest("Bad URL");
            }
            //    if (!await db.Users.AnyAsync(c => c.Id == request.UserId))
            //    {
            //        return Problem();
            //    }
            var newUrl = new Urls
            {
                OriginalUrl = request.LongUrl,

                ShortCode = "temp" // Placeholder
            };

            db.Urls.Add(newUrl);
            await db.SaveChangesAsync();

            newUrl.ShortCode = Utilities.Encode(newUrl.Id);

            await db.SaveChangesAsync();
            Console.WriteLine($"original: {request.LongUrl}, shortCode given: {newUrl.ShortCode}");
            return Ok(new { ShortUrl = $"http://localhost:5062/{newUrl.ShortCode}" });
        }

        [HttpPost("custom")]
        public IActionResult Delete()
        {
            return Ok(new { Message = "Server changes under way" });
        }


        [HttpGet("stats/{shortCode}")]

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


        [HttpDelete("{shortCode}")]
        public async Task<IActionResult> delete(string shortCode, AppDbContext db)
        {
            var input = await db.Urls.Where(c => c.ShortCode == shortCode).SingleOrDefaultAsync();
            if (input == null)
            {
                return NotFound();
            }

            db.Urls.Remove(input);
            await db.SaveChangesAsync();
            return Ok(new { Message = "Data was successfully deleted" });
        }
    }
}
