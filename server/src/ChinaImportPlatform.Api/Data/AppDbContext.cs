using System.Text.Json;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChinaImportPlatform.Api.Data;

/// <summary>
/// EF Core context for the PostgreSQL database. It is only used when the API is started
/// with "DataProvider": "Postgres"; the default JSON-backed provider needs no database.
/// To connect your database:
///
///   1. Set ConnectionStrings:Postgres (ideally from AWS Secrets Manager) and
///      "DataProvider": "Postgres" in configuration.
///   2. Generate and REVIEW the initial migration, then apply it:
///        dotnet ef migrations add InitialCreate --project src/ChinaImportPlatform.Api
///        dotnet ef database update      --project src/ChinaImportPlatform.Api
///
/// Child collections are owned by their aggregate and mapped as JSON columns (Order owns
/// Items + StatusHistory; Product owns Variants + Images; PaymentRequest owns Payments).
/// This keeps disconnected updates trivial (the JSON document is overwritten as a whole).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<PaymentRequest> PaymentRequests => Set<PaymentRequest>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Variant attributes (size/colour/model) persist as a JSON string so the
        // dictionary maps cleanly inside the owned JSON document.
        var attributesConverter = new ValueConverter<Dictionary<string, string>, string>(
            d => JsonSerializer.Serialize(d, (JsonSerializerOptions?)null),
            s => string.IsNullOrEmpty(s)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(s, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        var attributesComparer = new ValueComparer<Dictionary<string, string>>(
            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
            v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());

        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.PhoneNumber).IsUnique();
            b.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
            b.Property(u => u.FullName).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.Property(c => c.Name).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.Property(p => p.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(p => p.CategoryId);
            b.OwnsMany(p => p.Variants, v =>
            {
                v.ToJson();
                v.Property(pv => pv.Attributes)
                    .HasConversion(attributesConverter, attributesComparer);
            });
            b.OwnsMany(p => p.Images, i => i.ToJson());
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasIndex(o => o.OrderNumber).IsUnique();
            b.HasIndex(o => o.UserId);
            b.HasIndex(o => new { o.TripId, o.Status });
            b.OwnsMany(o => o.Items, i => i.ToJson());
            b.OwnsMany(o => o.StatusHistory, h => h.ToJson());
        });

        modelBuilder.Entity<PaymentRequest>(b =>
        {
            b.HasIndex(p => p.OrderId);
            b.OwnsMany(p => p.Payments, pay => pay.ToJson());
        });

        modelBuilder.Entity<Conversation>(b =>
        {
            b.HasIndex(c => c.CustomerId);
        });

        modelBuilder.Entity<Message>(b =>
        {
            b.HasIndex(m => m.ConversationId);
        });

        modelBuilder.Entity<DeviceToken>(b =>
        {
            b.HasIndex(d => new { d.UserId, d.Platform });
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasIndex(t => t.TokenHash);
            b.HasIndex(t => t.UserId);
        });
    }
}
