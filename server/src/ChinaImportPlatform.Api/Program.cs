using System.Text;
using System.Text.Json.Serialization;
using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
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

// ---------------------------------------------------------------------------
// Data access. JSON-file backed by default so the API runs with zero setup.
// Set "DataProvider": "Postgres" (and ConnectionStrings:Postgres) to switch to
// EF Core + Npgsql — no other code changes. See Common/DataAccessRegistration.
// ---------------------------------------------------------------------------
builder.Services.AddDataAccess(builder.Configuration, jsonStoreOptions);

// ---------------------------------------------------------------------------
// Application services
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<INotificationService, LoggingNotificationService>();
builder.Services.AddSingleton<ISmsSender, LoggingSmsSender>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IConsolidatedDemandService, ConsolidatedDemandService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IMessagingService, MessagingService>();

// ---------------------------------------------------------------------------
// Authentication & authorization (JWT bearer)
// ---------------------------------------------------------------------------
var jwtOptions = new JwtOptions();
builder.Configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    // Development fallback so the API runs without configuration. Production MUST
    // supply Jwt:SigningKey from AWS Secrets Manager (>= 32 chars).
    jwtOptions.SigningKey = "dev-only-signing-key-change-me-please-32+chars-minimum";
}
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.SellerOnly, policy => policy.RequireRole("Seller"));

    // Closed user base: every endpoint requires authentication unless [AllowAnonymous].
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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

    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the JWT access token (without the 'Bearer' prefix).",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", scheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
