import '../../../../core/network/api_result.dart';
import '../entities/order.dart';
import '../repositories/orders_repository.dart';

/// Fetches a single order by id.
class GetOrder {
  const GetOrder(this._repository);

  final OrdersRepository _repository;

  FutureResult<Order> call(String id) => _repository.getOrder(id);
}
