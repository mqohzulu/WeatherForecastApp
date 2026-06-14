using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Data;

/// <summary>
/// EF Core context for the future PostgreSQL database. It is NOT used by the running
/// application yet — the API serves data from JSON-backed repositories so it can run
/// without a database. When you are ready to connect your DB:
///
///   1. Set the "ConnectionStrings:Postgres" value in configuration / Secrets Manager.
///   2. Uncomment the AddDbContext + EF repository registrations in Program.cs.
///   3. Create EF Core implementations of the IRepos interfaces backed by this context.
///   4. Run: dotnet ef migrations add InitialCreate &amp;&amp; dotnet ef database update
///
/// The owned-type configuration mirrors the JSON aggregates (Order owns its items and
/// status history; Product owns its variants and images; PaymentRequest owns its payments).
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
            b.OwnsMany(p => p.Variants, v =>
            {
                v.ToJson();
            });
            b.OwnsMany(p => p.Images, i =>
            {
                i.ToJson();
            });
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasIndex(o => o.OrderNumber).IsUnique();
            b.HasIndex(o => new { o.TripId, o.Status });
            b.OwnsMany(o => o.Items, i => i.ToJson());
            b.OwnsMany(o => o.StatusHistory, h => h.ToJson());
        });

        modelBuilder.Entity<PaymentRequest>(b =>
        {
            b.HasIndex(p => p.OrderId);
            b.OwnsMany(p => p.Payments, pay => pay.ToJson());
        });

        modelBuilder.Entity<Message>(b =>
        {
            b.HasIndex(m => m.ConversationId);
        });

        modelBuilder.Entity<DeviceToken>(b =>
        {
            b.HasIndex(d => new { d.UserId, d.Platform });
        });
    }
}
