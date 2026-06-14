/// REST endpoint path constants for the .NET 8 API.
///
/// All paths are relative to `AppConfig.instance.apiUrl`
/// (which already includes the `/api/v1` prefix), so these strings
/// must NOT repeat the version segment.
class ApiEndpoints {
  ApiEndpoints._();

  // ---- Auth ----
  static const String requestOtp = '/auth/request-otp';
  static const String verifyOtp = '/auth/verify-otp';
  static const String refresh = '/auth/refresh';
  static const String logout = '/auth/logout';
  static const String me = '/auth/me';

  // ---- Catalogue ----
  static const String categories = '/categories';
  static String category(String id) => '/categories/$id';

  static const String products = '/products';
  static String product(String id) => '/products/$id';

  // ---- Trips (consolidated shipping runs) ----
  static const String trips = '/trips';
  static String trip(String id) => '/trips/$id';

  // ---- Orders ----
  static const String orders = '/orders';
  static String order(String id) => '/orders/$id';
  static String orderStatus(String id) => '/orders/$id/status';
  static String orderMessages(String id) => '/orders/$id/messages';

  // ---- Seller ----
  static const String consolidatedDemand = '/consolidated-demand';

  // ---- Announcements ----
  static const String announcements = '/announcements';
  static String announcement(String id) => '/announcements/$id';

  // ---- Messaging ----
  static const String conversations = '/conversations';
  static String conversation(String id) => '/conversations/$id';
  static const String messages = '/messages';
  static String conversationMessages(String conversationId) =>
      '/conversations/$conversationId/messages';

  // ---- Payments ----
  static const String paymentRequests = '/payment-requests';
  static String paymentRequest(String id) => '/payment-requests/$id';
  static String paymentProof(String id) => '/payment-requests/$id/proof';

  // ---- Notifications / devices ----
  static const String devices = '/devices';
  static String device(String id) => '/devices/$id';
}
