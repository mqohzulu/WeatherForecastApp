using System.Text.Json.Serialization;
using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Repos;
using ChinaImportPlatform.Api.Services;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Logging (Serilog)
// ---------------------------------------------------------------------------
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console());

// ---------------------------------------------------------------------------
// JSON-backed data store options
// The repositories read seed JSON from this directory. Defaults to the
// Data/SeedData folder shipped with the app so the API runs with zero setup.
// ---------------------------------------------------------------------------
var jsonStoreOptions = new JsonStoreOptions();
builder.Configuration.GetSection(JsonStoreOptions.SectionName).Bind(jsonStoreOptions);
if (string.IsNullOrWhiteSpace(jsonStoreOptions.DataPath))
{
    jsonStoreOptions.DataPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "SeedData");
}
builder.Services.AddSingleton(jsonStoreOptions);

// ---------------------------------------------------------------------------
// Repositories (JSON-backed). Registered as singletons because each holds an
// in-memory cache of its file. To move to PostgreSQL, implement these
// interfaces against AppDbContext and swap the registrations below.
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ITripRepository, TripRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddSingleton<IPaymentRequestRepository, PaymentRequestRepository>();
builder.Services.AddSingleton<IConversationRepository, ConversationRepository>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

// ---------------------------------------------------------------------------
// Application services
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<INotificationService, LoggingNotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IConsolidatedDemandService, ConsolidatedDemandService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IMessagingService, MessagingService>();

// ---------------------------------------------------------------------------
// When you connect PostgreSQL, uncomment the lines below (and add EF repos):
//
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
// ---------------------------------------------------------------------------

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "China Import & Distribution Platform API",
        Version = "v1",
        Description = "REST API for the private ordering and distribution platform."
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "China Import Platform API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseSerilogRequestLogging();
app.UseCors();
app.MapControllers();

app.Run();
