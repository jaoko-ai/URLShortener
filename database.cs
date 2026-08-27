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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Urls>()
            .Property(c => c.Id)
            .UseIdentityAlwaysColumn();


        modelBuilder.Entity<Clicks>()
            .Property(c => c.Id)
            .UseIdentityAlwaysColumn();
    }

}

public class Urls
{
    public int Id { get; set; }
    public required string OriginalUrl { get; set; }
    public required string ShortCode { get; set; }
    public string CreatedAt { get; set; } = DateTime.Now.ToString("h:mm:ss tt");
    public int ClickCount { get; set; }

    public ICollection<Clicks> Clicks { get; set; } = new List<Clicks>();


    // To be implemented when implementing USer accounts
    // public required string UserId { get; set; }
}


public class Clicks
{
    public int Id { get; set; }
    public required int UrlId { get; set; }
    [ForeignKey(nameof(UrlId))]
    public Urls? urls { get; set; }
    public string Timestamp { get; set; } = DateTime.Now.ToString("h:mm:ss tt");
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public required string referrer { get; set; }
}