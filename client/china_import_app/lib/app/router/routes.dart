/// Centralised route name + path constants.
///
/// Using named routes keeps navigation call-sites refactor-safe.
class Routes {
  Routes._();

  // Auth
  static const String login = 'login';
  static const String loginPath = '/login';

  static const String otp = 'otp';
  static const String otpPath = '/otp';

  // Catalogue / home
  static const String home = 'home';
  static const String homePath = '/';

  static const String productList = 'product-list';
  static const String productListPath = '/products';

  static const String productDetail = 'product-detail';
  static const String productDetailPath = '/products/:id';
  static String productDetailLocation(String id) => '/products/$id';

  // Cart
  static const String cart = 'cart';
  static const String cartPath = '/cart';

  // Orders
  static const String orders = 'orders';
  static const String ordersPath = '/orders';

  static const String orderDetail = 'order-detail';
  static const String orderDetailPath = '/orders/:id';
  static String orderDetailLocation(String id) => '/orders/$id';

  // Announcements
  static const String announcements = 'announcements';
  static const String announcementsPath = '/announcements';

  // Messaging
  static const String conversations = 'conversations';
  static const String conversationsPath = '/messages';

  static const String chat = 'chat';
  static const String chatPath = '/messages/:id';
  static String chatLocation(String id) => '/messages/$id';

  // Payments
  static const String paymentRequest = 'payment-request';
  static const String paymentRequestPath = '/payments/request';

  // Seller
  static const String sellerDashboard = 'seller-dashboard';
  static const String sellerDashboardPath = '/seller';

  static const String orderQueue = 'order-queue';
  static const String orderQueuePath = '/seller/queue';

  static const String consolidatedDemand = 'consolidated-demand';
  static const String consolidatedDemandPath = '/seller/demand';
}
