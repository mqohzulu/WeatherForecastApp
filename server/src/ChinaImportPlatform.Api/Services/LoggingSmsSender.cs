using ChinaImportPlatform.Api.IServices;

namespace ChinaImportPlatform.Api.Services;

/// <summary>Development SMS sink. Swap for a real provider behind <see cref="ISmsSender"/>.</summary>
public class LoggingSmsSender : ISmsSender
{
    private readonly ILogger<LoggingSmsSender> _logger;

    public LoggingSmsSender(ILogger<LoggingSmsSender> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default)
    {
        _logger.LogInformation("SMS -> {Phone}: Your verification code is {Otp} (valid 5 minutes).", phoneNumber, otp);
        return Task.CompletedTask;
    }
}
