import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
import '../../domain/repositories/catalogue_repository.dart';
import '../datasources/catalogue_remote_datasource.dart';

class CatalogueRepositoryImpl with RepositoryHelper implements CatalogueRepository {
  CatalogueRepositoryImpl({required CatalogueRemoteDataSource remote})
      : _remote = remote;

  final CatalogueRemoteDataSource _remote;

  @override
  FutureResult<List<Category>> getCategories() {
    return guard(() async {
      final models = await _remote.getCategories();
      return models.map((m) => m.toEntity()).toList(growable: false);
    });
  }

  @override
  FutureResult<PaginatedResponse<Product>> getProducts({
    String? categoryId,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) {
    return guard(() async {
      final response = await _remote.getProducts(
        categoryId: categoryId,
        search: search,
        page: page,
        pageSize: pageSize,
      );
      return response.map((m) => m.toEntity());
    });
  }

  @override
  FutureResult<Product> getProduct(String id) {
    return guard(() async {
      final model = await _remote.getProduct(id);
      return model.toEntity();
    });
  }
}
