import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../data/datasources/orders_remote_datasource.dart';
import '../../data/repositories/orders_repository_impl.dart';
import '../../domain/entities/order.dart';
import '../../domain/repositories/orders_repository.dart';
import '../../domain/usecases/get_order.dart';
import '../../domain/usecases/get_orders.dart';
import '../../domain/usecases/update_order_status.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final ordersRemoteDataSourceProvider =
    Provider<OrdersRemoteDataSource>((ref) {
  return OrdersRemoteDataSourceImpl(ref.watch(dioProvider));
});

final ordersRepositoryProvider = Provider<OrdersRepository>((ref) {
  return OrdersRepositoryImpl(
    remote: ref.watch(ordersRemoteDataSourceProvider),
  );
});

final getOrdersUseCaseProvider = Provider<GetOrders>((ref) {
  return GetOrders(ref.watch(ordersRepositoryProvider));
});

final getOrderUseCaseProvider = Provider<GetOrder>((ref) {
  return GetOrder(ref.watch(ordersRepositoryProvider));
});

final updateOrderStatusUseCaseProvider = Provider<UpdateOrderStatus>((ref) {
  return UpdateOrderStatus(ref.watch(ordersRepositoryProvider));
});

// ---------------------------------------------------------------------------
// Data providers
// ---------------------------------------------------------------------------

/// The current user's orders (first page).
final ordersProvider =
    FutureProvider<PaginatedResponse<Order>>((ref) async {
  final result = await ref.watch(getOrdersUseCaseProvider)();
  return result.match(
    (failure) => throw failure,
    (page) => page,
  );
});

/// A single order by id.
final orderDetailProvider =
    FutureProvider.family<Order, String>((ref, orderId) async {
  final result = await ref.watch(getOrderUseCaseProvider)(orderId);
  return result.match(
    (failure) => throw failure,
    (order) => order,
  );
});
