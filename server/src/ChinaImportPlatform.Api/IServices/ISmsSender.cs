namespace ChinaImportPlatform.Api.IServices;

/// <summary>
/// Sends transactional SMS (currently just OTPs). The scaffold logs the message;
/// a production implementation calls an SMS provider (e.g. Clickatell, Twilio).
/// </summary>
public interface ISmsSender
{
    Task SendOtpAsync(string phoneNumber, string otp, CancellationToken ct = default);
}
