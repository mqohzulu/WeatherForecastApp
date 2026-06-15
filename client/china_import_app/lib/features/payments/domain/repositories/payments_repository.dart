import '../../../../core/network/api_result.dart';
import '../entities/payment_request.dart';

/// Contract for the payments feature.
abstract interface class PaymentsRepository {
  FutureResult<List<PaymentRequest>> getPaymentRequests({String? orderId});

  FutureResult<void> uploadProof(String paymentRequestId, String filePath);
}
