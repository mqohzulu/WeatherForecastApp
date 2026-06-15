import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';
import '../models/payment_request_model.dart';

/// Remote calls for the payments feature.
abstract interface class PaymentsRemoteDataSource {
  Future<List<PaymentRequestModel>> getPaymentRequests({String? orderId});

  Future<void> uploadProof(String paymentRequestId, String filePath);
}

class PaymentsRemoteDataSourceImpl implements PaymentsRemoteDataSource {
  PaymentsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<List<PaymentRequestModel>> getPaymentRequests({String? orderId}) async {
    final res = await _dio.get<List<dynamic>>(
      ApiEndpoints.paymentRequests,
      queryParameters: orderId == null ? null : {'orderId': orderId},
    );
    return (res.data ?? <dynamic>[])
        .whereType<Map<String, dynamic>>()
        .map(PaymentRequestModel.fromJson)
        .toList(growable: false);
  }

  @override
  Future<void> uploadProof(String paymentRequestId, String filePath) async {
    final form = FormData.fromMap({
      'file': await MultipartFile.fromFile(filePath),
    });
    await _dio.post<void>(
      ApiEndpoints.paymentProof(paymentRequestId),
      data: form,
    );
  }
}
