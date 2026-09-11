using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Data;
using Microsoft.EntityFrameworkCore;


namespace URLShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Delete : ControllerBase
    {
        [HttpPost("{shortCode}")]
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
