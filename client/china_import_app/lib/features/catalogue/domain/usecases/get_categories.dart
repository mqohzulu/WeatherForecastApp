import '../../../../core/network/api_result.dart';
import '../entities/category.dart';
import '../repositories/catalogue_repository.dart';

/// Fetches the list of product categories.
class GetCategories {
  const GetCategories(this._repository);

  final CatalogueRepository _repository;

  FutureResult<List<Category>> call() => _repository.getCategories();
}
