import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';
import '../models/demand_line_model.dart';

/// Remote calls for the seller feature.
abstract interface class SellerRemoteDataSource {
  Future<List<DemandLineModel>> getConsolidatedDemand();
}

class SellerRemoteDataSourceImpl implements SellerRemoteDataSource {
  SellerRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<List<DemandLineModel>> getConsolidatedDemand() async {
    final res = await _dio.get<List<dynamic>>(
      ApiEndpoints.consolidatedDemand,
    );
    return (res.data ?? <dynamic>[])
        .whereType<Map<String, dynamic>>()
        .map(DemandLineModel.fromJson)
        .toList(growable: false);
  }
}
