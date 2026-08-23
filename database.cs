using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace URLShortener.Data;

public class AppDbContext : DbContext
{
    public DbSet<Urls> Urls { get; set; }
    public DbSet<Clicks> Clicks { get; set; }

    // The constructor accepts options from Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}

public class Urls
{
    public int Id { get; set; }
    public required string OriginalUrl { get; set; }
    public required string ShortCode { get; set; }
    public required string CreatedAt { get; set; }
    public int ClickCount { get; set; }

    // To be implemented when implementing USer accounts
    // public required string UserId { get; set; }
}


public class Clicks
{
    public required int Id { get; set; }
    [ForeignKey("Urls")]
    public required int UrlId { get; set; }
    public required int timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public required string referrer { get; set; }
}