using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Repos;
using ChinaImportPlatform.Api.Repos.Ef;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Common;

/// <summary>
/// Registers the data-access layer. The provider is chosen by the "DataProvider"
/// configuration value:
///   "Json"     (default) → JSON-file backed repositories; no database required.
///   "Postgres"           → EF Core + Npgsql repositories backed by <see cref="AppDbContext"/>.
///
/// Both paths register the exact same repository interfaces, so nothing else in the
/// application changes when you connect your database.
/// </summary>
public static class DataAccessRegistration
{
    public const string Postgres = "Postgres";

    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration, JsonStoreOptions jsonStoreOptions)
    {
        var provider = configuration.GetValue<string>("DataProvider") ?? "Json";

        return provider.Equals(Postgres, StringComparison.OrdinalIgnoreCase)
            ? services.AddEfRepositories(configuration)
            : services.AddJsonRepositories(jsonStoreOptions);
    }

    /// <summary>JSON-file repositories. Singletons because each caches its file in memory.</summary>
    public static IServiceCollection AddJsonRepositories(this IServiceCollection services, JsonStoreOptions options)
    {
        services.AddSingleton(options);

        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddSingleton<ITripRepository, TripRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IAnnouncementRepository, AnnouncementRepository>();
        services.AddSingleton<IPaymentRequestRepository, PaymentRequestRepository>();
        services.AddSingleton<IConversationRepository, ConversationRepository>();
        services.AddSingleton<IMessageRepository, MessageRepository>();
        services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IDeviceTokenRepository, DeviceTokenRepository>();

        return services;
    }

    /// <summary>EF Core + PostgreSQL repositories. Scoped, matching the DbContext lifetime.</summary>
    public static IServiceCollection AddEfRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ITripRepository, EfTripRepository>();
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IAnnouncementRepository, EfAnnouncementRepository>();
        services.AddScoped<IPaymentRequestRepository, EfPaymentRequestRepository>();
        services.AddScoped<IConversationRepository, EfConversationRepository>();
        services.AddScoped<IMessageRepository, EfMessageRepository>();
        services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();
        services.AddScoped<IDeviceTokenRepository, EfDeviceTokenRepository>();

        return services;
    }
}
