using URLShortener.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.Extensions;

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
                // if (value === -1) throw new Error('Invalid Base62 character: ' + char);
                result = result * 62 + value;
            }
            return result;
        }


        app.MapPost("/shorten", (AppDbContext db, UrlRequest request) =>
        {
            if (string.IsNullOrEmpty(request.LongUrl))
            {
                return Results.BadRequest("URL cannot be empty.");
            }

            string shortCode = Encode(1);
            return Results.Ok(new { ShortUrl = $"http://localhost:5062/{shortCode}" });
        });

        app.MapGet("/:shorten", () =>
        {

        });

        app.MapPost("/stats/:shortcode", () => { });
        app.MapPost("/custom", () => { });
        app.MapDelete("/:shortcode", () => { });
        app.Run();

    }
}