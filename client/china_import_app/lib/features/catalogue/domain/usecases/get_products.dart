import '../../../../core/network/api_result.dart';
import '../../../../shared/models/paginated_response.dart';
import '../entities/product.dart';
import '../repositories/catalogue_repository.dart';

/// Fetches a paginated page of products, optionally filtered.
class GetProducts {
  const GetProducts(this._repository);

  final CatalogueRepository _repository;

  FutureResult<PaginatedResponse<Product>> call({
    String? categoryId,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) {
    return _repository.getProducts(
      categoryId: categoryId,
      search: search,
      page: page,
      pageSize: pageSize,
    );
  }
}
