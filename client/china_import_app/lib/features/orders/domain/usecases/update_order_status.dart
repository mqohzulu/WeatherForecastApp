import '../../../../core/network/api_result.dart';
import '../entities/order.dart';
import '../entities/order_status.dart';
import '../repositories/orders_repository.dart';

/// Updates the status of an order.
class UpdateOrderStatus {
  const UpdateOrderStatus(this._repository);

  final OrdersRepository _repository;

  FutureResult<Order> call({
    required String orderId,
    required OrderStatus status,
    String? note,
  }) {
    return _repository.updateStatus(
      orderId: orderId,
      status: status,
      note: note,
    );
  }
}
