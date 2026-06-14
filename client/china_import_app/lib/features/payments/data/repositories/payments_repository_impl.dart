import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../domain/entities/payment_request.dart';
import '../../domain/repositories/payments_repository.dart';
import '../datasources/payments_remote_datasource.dart';

class PaymentsRepositoryImpl
    with RepositoryHelper
    implements PaymentsRepository {
  PaymentsRepositoryImpl({required PaymentsRemoteDataSource remote})
      : _remote = remote;

  final PaymentsRemoteDataSource _remote;

  @override
  FutureResult<List<PaymentRequest>> getPaymentRequests({String? orderId}) {
    return guard(() async {
      final models = await _remote.getPaymentRequests(orderId: orderId);
      return models.map((model) => model.toEntity()).toList(growable: false);
    });
  }

  @override
  FutureResult<void> uploadProof(String paymentRequestId, String filePath) {
    return guard(() => _remote.uploadProof(paymentRequestId, filePath));
  }
}
