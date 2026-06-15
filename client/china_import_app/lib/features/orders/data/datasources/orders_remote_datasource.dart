import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../domain/entities/order_status.dart';
import '../models/order_model.dart';

/// Remote calls for the orders feature.
abstract interface class OrdersRemoteDataSource {
  Future<PaginatedResponse<OrderModel>> getOrders({
    int page = 1,
    int pageSize = 20,
  });

  Future<OrderModel> getOrder(String id);

  Future<OrderModel> createOrder(Map<String, dynamic> body);

  Future<OrderModel> updateStatus(
    String id,
    OrderStatus status, {
    String? note,
  });
}

class OrdersRemoteDataSourceImpl implements OrdersRemoteDataSource {
  OrdersRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<PaginatedResponse<OrderModel>> getOrders({
    int page = 1,
    int pageSize = 20,
  }) async {
    final res = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.orders,
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return PaginatedResponse<OrderModel>.fromJson(
      res.data!,
      OrderModel.fromJson,
    );
  }

  @override
  Future<OrderModel> getOrder(String id) async {
    final res = await _dio.get<Map<String, dynamic>>(ApiEndpoints.order(id));
    return OrderModel.fromJson(res.data!);
  }

  @override
  Future<OrderModel> createOrder(Map<String, dynamic> body) async {
    final res = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.orders,
      data: body,
    );
    return OrderModel.fromJson(res.data!);
  }

  @override
  Future<OrderModel> updateStatus(
    String id,
    OrderStatus status, {
    String? note,
  }) async {
    final res = await _dio.patch<Map<String, dynamic>>(
      ApiEndpoints.orderStatus(id),
      data: {'status': status.toJson(), 'note': note},
    );
    return OrderModel.fromJson(res.data!);
  }
}
