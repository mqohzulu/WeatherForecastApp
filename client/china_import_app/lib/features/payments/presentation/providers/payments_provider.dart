import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../data/datasources/payments_remote_datasource.dart';
import '../../data/repositories/payments_repository_impl.dart';
import '../../domain/entities/payment_request.dart';
import '../../domain/repositories/payments_repository.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final paymentsRemoteDataSourceProvider =
    Provider<PaymentsRemoteDataSource>((ref) {
  return PaymentsRemoteDataSourceImpl(ref.watch(dioProvider));
});

final paymentsRepositoryProvider = Provider<PaymentsRepository>((ref) {
  return PaymentsRepositoryImpl(
    remote: ref.watch(paymentsRemoteDataSourceProvider),
  );
});

// ---------------------------------------------------------------------------
// Screen state
// ---------------------------------------------------------------------------

/// Payment requests for an optional [orderId]. When `null`, returns all.
final paymentRequestsProvider =
    FutureProvider.family<List<PaymentRequest>, String?>((ref, orderId) async {
  final result = await ref
      .watch(paymentsRepositoryProvider)
      .getPaymentRequests(orderId: orderId);
  return result.match(
    (failure) => throw Exception(failure.message),
    (requests) => requests,
  );
});
