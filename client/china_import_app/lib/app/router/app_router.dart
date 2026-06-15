import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/announcements/presentation/screens/announcements_screen.dart';
import '../../features/auth/presentation/providers/auth_provider.dart';
import '../../features/auth/presentation/screens/login_screen.dart';
import '../../features/auth/presentation/screens/otp_screen.dart';
import '../../features/cart/presentation/screens/cart_screen.dart';
import '../../features/catalogue/presentation/screens/categories_screen.dart';
import '../../features/catalogue/presentation/screens/product_detail_screen.dart';
import '../../features/catalogue/presentation/screens/product_list_screen.dart';
import '../../features/messaging/presentation/screens/chat_screen.dart';
import '../../features/messaging/presentation/screens/conversations_screen.dart';
import '../../features/orders/presentation/screens/order_detail_screen.dart';
import '../../features/orders/presentation/screens/orders_screen.dart';
import '../../features/payments/presentation/screens/payment_request_screen.dart';
import '../../features/seller/presentation/screens/consolidated_demand_screen.dart';
import '../../features/seller/presentation/screens/dashboard_screen.dart';
import '../../features/seller/presentation/screens/order_queue_screen.dart';
import 'routes.dart';

/// Provides the application's [GoRouter], rebuilt when auth status changes.
final routerProvider = Provider<GoRouter>((ref) {
  final notifier = _AuthRouterNotifier(ref);

  return GoRouter(
    initialLocation: Routes.homePath,
    refreshListenable: notifier,
    debugLogDiagnostics: true,
    redirect: (context, goRouterState) {
      final auth = ref.read(authProvider);

      // Wait for session restoration before redirecting.
      if (auth.status == AuthStatus.unknown) return null;

      final loggingIn = goRouterState.matchedLocation == Routes.loginPath ||
          goRouterState.matchedLocation == Routes.otpPath;

      if (!auth.isAuthenticated) {
        return loggingIn ? null : Routes.loginPath;
      }

      // Authenticated users should not sit on the auth screens.
      if (loggingIn) {
        return auth.isSeller ? Routes.sellerDashboardPath : Routes.homePath;
      }
      return null;
    },
    routes: [
      GoRoute(
        path: Routes.loginPath,
        name: Routes.login,
        builder: (context, state) => const LoginScreen(),
      ),
      GoRoute(
        path: Routes.otpPath,
        name: Routes.otp,
        builder: (context, state) => const OtpScreen(),
      ),
      GoRoute(
        path: Routes.homePath,
        name: Routes.home,
        builder: (context, state) => const CategoriesScreen(),
      ),
      GoRoute(
        path: Routes.productListPath,
        name: Routes.productList,
        builder: (context, state) => ProductListScreen(
          categoryId: state.uri.queryParameters['categoryId'],
        ),
      ),
      GoRoute(
        path: Routes.productDetailPath,
        name: Routes.productDetail,
        builder: (context, state) =>
            ProductDetailScreen(productId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: Routes.cartPath,
        name: Routes.cart,
        builder: (context, state) => const CartScreen(),
      ),
      GoRoute(
        path: Routes.ordersPath,
        name: Routes.orders,
        builder: (context, state) => const OrdersScreen(),
      ),
      GoRoute(
        path: Routes.orderDetailPath,
        name: Routes.orderDetail,
        builder: (context, state) =>
            OrderDetailScreen(orderId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: Routes.announcementsPath,
        name: Routes.announcements,
        builder: (context, state) => const AnnouncementsScreen(),
      ),
      GoRoute(
        path: Routes.conversationsPath,
        name: Routes.conversations,
        builder: (context, state) => const ConversationsScreen(),
      ),
      GoRoute(
        path: Routes.chatPath,
        name: Routes.chat,
        builder: (context, state) =>
            ChatScreen(conversationId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: Routes.paymentRequestPath,
        name: Routes.paymentRequest,
        builder: (context, state) => PaymentRequestScreen(
          orderId: state.uri.queryParameters['orderId'],
        ),
      ),
      GoRoute(
        path: Routes.sellerDashboardPath,
        name: Routes.sellerDashboard,
        builder: (context, state) => const SellerDashboardScreen(),
      ),
      GoRoute(
        path: Routes.orderQueuePath,
        name: Routes.orderQueue,
        builder: (context, state) => const OrderQueueScreen(),
      ),
      GoRoute(
        path: Routes.consolidatedDemandPath,
        name: Routes.consolidatedDemand,
        builder: (context, state) => const ConsolidatedDemandScreen(),
      ),
    ],
    errorBuilder: (context, state) => Scaffold(
      appBar: AppBar(title: const Text('Not found')),
      body: Center(child: Text('Route not found: ${state.uri}')),
    ),
  );
});

/// Bridges Riverpod auth state changes into GoRouter's [Listenable] refresh.
class _AuthRouterNotifier extends ChangeNotifier {
  _AuthRouterNotifier(this._ref) {
    _ref.listen<AuthState>(
      authProvider,
      (_, __) => notifyListeners(),
    );
  }

  final Ref _ref;
}
