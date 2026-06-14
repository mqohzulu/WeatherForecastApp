import '../../../../core/network/api_result.dart';
import '../../../../shared/models/paginated_response.dart';
import '../entities/category.dart';
import '../entities/product.dart';

/// Abstract catalogue contract consumed by the domain/presentation layers.
abstract interface class CatalogueRepository {
  /// Returns the full list of product categories.
  FutureResult<List<Category>> getCategories();

  /// Returns a paginated page of products, optionally filtered by category
  /// and/or free-text [search].
  FutureResult<PaginatedResponse<Product>> getProducts({
    String? categoryId,
    String? search,
    int page,
    int pageSize,
  });

  /// Returns a single product by [id].
  FutureResult<Product> getProduct(String id);
}
