import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../domain/entities/demand_line.dart';
import '../../domain/repositories/seller_repository.dart';
import '../datasources/seller_remote_datasource.dart';

class SellerRepositoryImpl with RepositoryHelper implements SellerRepository {
  SellerRepositoryImpl({required SellerRemoteDataSource remote})
      : _remote = remote;

  final SellerRemoteDataSource _remote;

  @override
  FutureResult<List<DemandLine>> getConsolidatedDemand() {
    return guard(() async {
      final models = await _remote.getConsolidatedDemand();
      return models.map((model) => model.toEntity()).toList(growable: false);
    });
  }
}
