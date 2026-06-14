import '../../../../core/network/api_result.dart';
import '../../../../shared/models/paginated_response.dart';
import '../entities/order.dart';
import '../entities/order_item.dart';
import '../entities/order_status.dart';

/// Abstract orders contract consumed by the domain/presentation layers.
abstract interface class OrdersRepository {
  /// Returns a paginated page of the current user's orders.
  FutureResult<PaginatedResponse<Order>> getOrders({
    int page = 1,
    int pageSize = 20,
  });

  /// Returns a single order by [id].
  FutureResult<Order> getOrder(String id);

  /// Creates a new order from the given [items].
  FutureResult<Order> createOrder({
    required List<OrderItem> items,
    String? notes,
    bool isCustomOrder = false,
  });

  /// Updates the status of the order identified by [orderId].
  FutureResult<Order> updateStatus({
    required String orderId,
    required OrderStatus status,
    String? note,
  });
}
