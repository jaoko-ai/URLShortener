using URLShortener.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace URLShortener;

class Program
{

    record UrlRequest(string LongUrl);
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var app = builder.Build();

        const string ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


        static string Encode(int num)
        {
            string output = "";
            if (num == 0)
            {
                return "0";
            }
            while (num > 0)
            {
                output = $"{ALPHABET[num % 62]}{output}";
                num /= 62;
            }
            return output;
        }



        static int Decode(string str)
        {
            int result = 0;
            foreach (char character in str)
            {
                var value = ALPHABET.IndexOf(character);
                //TODO: implement better error handling
                // if (value === -1) throw new Error('Invalid Base62 character: ' + char);
                result = result * 62 + value;
            }
            return result;
        }

        app.MapPost("/shorten", async (AppDbContext db, UrlRequest request) =>
       {
           if (string.IsNullOrEmpty(request.LongUrl))
           {
               return Results.BadRequest("URL cannot be empty.");
           }

           var newUrl = new Urls
           {
               OriginalUrl = request.LongUrl,

               ShortCode = "temp" // Placeholder
           };

           db.Urls.Add(newUrl);
           await db.SaveChangesAsync();

           newUrl.ShortCode = Encode(newUrl.Id);

           await db.SaveChangesAsync();

           return Results.Ok(new { ShortUrl = $"http://localhost:5062/{newUrl.ShortCode}" });
       });
        app.MapGet("/:{shortCode}", async (AppDbContext db, string shortCode, HttpContext context) =>
        {
            Console.WriteLine($"Parameter reached: {shortCode}");
            // Validate

            if (string.IsNullOrWhiteSpace(shortCode))
            {
                return Results.BadRequest("bad Request.");
            }

            Urls record;
            try
            {

                record = await db.Urls
                .Where(u => u.ShortCode == shortCode)
                .SingleAsync();
            }
            catch
            {
                return Results.Ok(new { ShortCode = "Not found" });
            }
            record.ClickCount++;

            string clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            string userAgent = context.Request.Headers.UserAgent.ToString();
            string referrer = context.Request.Headers.Referer.ToString();
            string language = context.Request.Headers.AcceptLanguage.ToString();
            try
            {

                var input = await db.Clicks.Where(u => u.Id == record.Id).SingleAsync();
                Clicks analytics;
                if (input == null)
                {
                    analytics = new Clicks
                    {
                        UrlId = record.Id,
                        referrer = referrer,
                        UserAgent = userAgent,
                        IpAddress = clientIp,
                        Urls = record
                    };
                    await db.Clicks.AddAsync(analytics);
                }
            }
            catch
            {
                var analytics = new Clicks
                {
                    UrlId = record.Id,
                    referrer = referrer,
                    UserAgent = userAgent,
                    IpAddress = clientIp,
                    Urls = record
                };
                await db.Clicks.AddAsync(analytics);
            }
            await db.SaveChangesAsync();
            return Results.Redirect(record.OriginalUrl);
        });

        app.MapGet("/stats/:{shortCode}", async (AppDbContext db, string shortCode) =>
        {
            var input = Decode(shortCode);
            Clicks? stats = null;
            try
            {
                stats = await db.Clicks.Where(u => u.UrlId == input).SingleAsync();
            }
            catch
            {
                return Results.Ok("No stats yet fot this route");
            }

            return Results.Ok(stats);
        });
        app.MapPost("/custom", async (AppDbContext db, UrlRequest request) =>
        {
            if (string.IsNullOrEmpty(request.LongUrl))
            {
                return Results.BadRequest("URL cannot be empty.");
            }

            var newUrl = new Urls
            {
                OriginalUrl = request.LongUrl,

                ShortCode = "temp" // Placeholder
            };

            db.Urls.Add(newUrl);
            await db.SaveChangesAsync();

            newUrl.ShortCode = Encode(newUrl.Id);

            await db.SaveChangesAsync();

            return Results.Ok(new { ShortUrl = $"https://localhost:5062{newUrl.ShortCode}" });
        });
        app.MapDelete("/:{shortCode}", async (AppDbContext db, string shortCode) =>
        {
            var input = await db.Urls.Where(c => c.ShortCode == shortCode).SingleAsync();
            if (input == null)
            {
                return Results.NotFound();
            }

            db.Urls.Remove(input);
            await db.SaveChangesAsync();
            return Results.Ok(new { Message = "Data was successfully deleted" });
        });
        app.Run();

    }
}