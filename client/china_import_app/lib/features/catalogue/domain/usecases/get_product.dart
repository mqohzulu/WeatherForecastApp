import '../../../../core/network/api_result.dart';
import '../entities/product.dart';
import '../repositories/catalogue_repository.dart';

/// Fetches a single product by id.
class GetProduct {
  const GetProduct(this._repository);

  final CatalogueRepository _repository;

  FutureResult<Product> call(String id) => _repository.getProduct(id);
}
