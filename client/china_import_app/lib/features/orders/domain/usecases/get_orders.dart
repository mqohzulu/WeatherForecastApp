import '../../../../core/network/api_result.dart';
import '../../../../shared/models/paginated_response.dart';
import '../entities/order.dart';
import '../repositories/orders_repository.dart';

/// Fetches a paginated page of the current user's orders.
class GetOrders {
  const GetOrders(this._repository);

  final OrdersRepository _repository;

  FutureResult<PaginatedResponse<Order>> call({
    int page = 1,
    int pageSize = 20,
  }) {
    return _repository.getOrders(page: page, pageSize: pageSize);
  }
}
