using URLShortener.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        result = result * 62 + value;
    }
    return result;
}

// if (value === -1) throw new Error('Invalid Base62 character: ' + char);

app.Run();
