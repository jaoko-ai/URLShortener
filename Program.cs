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
        app.MapGet("/:{shortCode}", async (AppDbContext db, string shortCode) =>
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
                throw new ArgumentException("shortCode not found in database");
            }
            record.ClickCount++;
            await db.SaveChangesAsync();
            return Results.Redirect(record.OriginalUrl);
        });

        app.MapPost("/stats/:{shortCode}", async () =>
        {

        });
        app.MapPost("/custom", () => { });
        app.MapDelete("/:{shortCode}", () =>
        {

        });
        app.Run();

    }
}