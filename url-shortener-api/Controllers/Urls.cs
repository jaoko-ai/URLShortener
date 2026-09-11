using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Entities;
using URLShortener.Data;
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
    }
}
