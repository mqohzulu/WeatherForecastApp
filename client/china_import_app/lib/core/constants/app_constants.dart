/// Application-wide constant values.
class AppConstants {
  AppConstants._();

  static const String appName = 'China Import';
  static const String appTagline = 'Import & Distribution Platform';

  // Locale / currency
  static const String defaultLocale = 'en_ZA';
  static const String currencyCode = 'ZAR';
  static const String currencySymbol = 'R';

  // Pagination
  static const int defaultPageSize = 20;
  static const int firstPage = 1;

  // OTP
  static const int otpLength = 6;
  static const Duration otpResendCooldown = Duration(seconds: 60);

  // Storage keys (see TokenStorage / SecureStorageService)
  static const String keyAccessToken = 'access_token';
  static const String keyRefreshToken = 'refresh_token';
  static const String keyUserId = 'user_id';
  static const String keyUserRole = 'user_role';

  // Country
  static const String defaultDialCode = '+27';
  static const String defaultCountryCode = 'ZA';
}
