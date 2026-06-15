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
  static String profile(String userId) => '/auth/profile/$userId';
  static String notificationPreferences(String userId) =>
      '/auth/profile/$userId/notification-preferences';

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
  static const String ordersByUser = '/orders/by-user';
  static String ordersForUser(String userId) => '/orders/by-user/$userId';
  static const String ordersNewCount = '/orders/new-count';
  static const String ordersBulkStatus = '/orders/bulk-status';
  static String order(String id) => '/orders/$id';
  static String orderStatus(String id) => '/orders/$id/status';
  static String orderPricing(String id) => '/orders/$id/pricing';
  static String orderRejectLine(String id) => '/orders/$id/lines/reject';
  static String orderReadyForCollection(String id) =>
      '/orders/$id/ready-for-collection';
  static String orderCancel(String id) => '/orders/$id/cancel';
  static String orderMessages(String id) => '/orders/$id/messages';

  // ---- Seller ----
  static const String consolidatedDemand = '/consolidated-demand';
  static const String consolidatedDemandMarkSourced =
      '/consolidated-demand/mark-sourced';

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
  static String paymentRequestsByOrder(String orderId) =>
      '/payment-requests/by-order/$orderId';
  static String paymentApprove(String id, String paymentId) =>
      '/payment-requests/$id/payments/$paymentId/approve';

  // ---- Notifications / devices ----
  static const String devices = '/devices';
  static String device(String id) => '/devices/$id';
}
