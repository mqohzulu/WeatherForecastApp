import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../data/datasources/seller_remote_datasource.dart';
import '../../data/repositories/seller_repository_impl.dart';
import '../../domain/entities/demand_line.dart';
import '../../domain/repositories/seller_repository.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final sellerRemoteDataSourceProvider =
    Provider<SellerRemoteDataSource>((ref) {
  return SellerRemoteDataSourceImpl(ref.watch(dioProvider));
});

final sellerRepositoryProvider = Provider<SellerRepository>((ref) {
  return SellerRepositoryImpl(
    remote: ref.watch(sellerRemoteDataSourceProvider),
  );
});

// ---------------------------------------------------------------------------
// Screen state
// ---------------------------------------------------------------------------

/// Consolidated demand lines across all open orders.
final consolidatedDemandProvider =
    FutureProvider<List<DemandLine>>((ref) async {
  final result =
      await ref.watch(sellerRepositoryProvider).getConsolidatedDemand();
  return result.match(
    (failure) => throw Exception(failure.message),
    (lines) => lines,
  );
});
