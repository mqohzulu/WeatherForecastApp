import '../../../../core/network/api_result.dart';
import '../entities/demand_line.dart';

/// Contract for the seller feature.
abstract interface class SellerRepository {
  FutureResult<List<DemandLine>> getConsolidatedDemand();
}
