import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../domain/entities/order.dart';
import '../../domain/entities/order_item.dart';
import '../../domain/entities/order_status.dart';
import '../../domain/repositories/orders_repository.dart';
import '../datasources/orders_remote_datasource.dart';

class OrdersRepositoryImpl with RepositoryHelper implements OrdersRepository {
  OrdersRepositoryImpl({required OrdersRemoteDataSource remote})
      : _remote = remote;

  final OrdersRemoteDataSource _remote;

  @override
  FutureResult<PaginatedResponse<Order>> getOrders({
    int page = 1,
    int pageSize = 20,
  }) {
    return guard(() async {
      final response = await _remote.getOrders(page: page, pageSize: pageSize);
      return response.map((model) => model.toEntity());
    });
  }

  @override
  FutureResult<Order> getOrder(String id) {
    return guard(() async {
      final model = await _remote.getOrder(id);
      return model.toEntity();
    });
  }

  @override
  FutureResult<Order> createOrder({
    required List<OrderItem> items,
    String? notes,
    bool isCustomOrder = false,
  }) {
    return guard(() async {
      final body = <String, dynamic>{
        'items': items
            .map((item) => <String, dynamic>{
                  'productId': item.productId,
                  'variantLabel': item.variantLabel,
                  'quantity': item.quantity,
                })
            .toList(growable: false),
        'notes': notes,
        'isCustomOrder': isCustomOrder,
      };
      final model = await _remote.createOrder(body);
      return model.toEntity();
    });
  }

  @override
  FutureResult<Order> updateStatus({
    required String orderId,
    required OrderStatus status,
    String? note,
  }) {
    return guard(() async {
      final model = await _remote.updateStatus(orderId, status, note: note);
      return model.toEntity();
    });
  }
}
