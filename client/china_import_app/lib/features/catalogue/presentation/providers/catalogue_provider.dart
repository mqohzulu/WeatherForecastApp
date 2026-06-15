import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../data/datasources/catalogue_remote_datasource.dart';
import '../../data/repositories/catalogue_repository_impl.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
import '../../domain/repositories/catalogue_repository.dart';
import '../../domain/usecases/get_categories.dart';
import '../../domain/usecases/get_product.dart';
import '../../domain/usecases/get_products.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final catalogueRemoteDataSourceProvider =
    Provider<CatalogueRemoteDataSource>((ref) {
  return CatalogueRemoteDataSourceImpl(ref.watch(dioProvider));
});

final catalogueRepositoryProvider = Provider<CatalogueRepository>((ref) {
  return CatalogueRepositoryImpl(
    remote: ref.watch(catalogueRemoteDataSourceProvider),
  );
});

final getCategoriesUseCaseProvider = Provider<GetCategories>((ref) {
  return GetCategories(ref.watch(catalogueRepositoryProvider));
});

final getProductsUseCaseProvider = Provider<GetProducts>((ref) {
  return GetProducts(ref.watch(catalogueRepositoryProvider));
});

final getProductUseCaseProvider = Provider<GetProduct>((ref) {
  return GetProduct(ref.watch(catalogueRepositoryProvider));
});

// ---------------------------------------------------------------------------
// UI state
// ---------------------------------------------------------------------------

/// Free-text search query applied to the product list. Empty = no filter.
final productSearchQueryProvider = StateProvider<String>((ref) => '');

// ---------------------------------------------------------------------------
// Async data
// ---------------------------------------------------------------------------

/// All categories. Throws the failure message so [AsyncValue] surfaces it.
final categoriesProvider = FutureProvider<List<Category>>((ref) async {
  final result = await ref.watch(getCategoriesUseCaseProvider)();
  return result.match(
    (failure) => throw Exception(failure.message),
    (categories) => categories,
  );
});

/// Products filtered by an optional category id (family argument), and by the
/// current [productSearchQueryProvider].
final productsProvider =
    FutureProvider.family<PaginatedResponse<Product>, String?>(
        (ref, categoryId) async {
  final search = ref.watch(productSearchQueryProvider);
  final result = await ref.watch(getProductsUseCaseProvider)(
    categoryId: categoryId,
    search: search.isEmpty ? null : search,
  );
  return result.match(
    (failure) => throw Exception(failure.message),
    (page) => page,
  );
});

/// A single product by id.
final productDetailProvider =
    FutureProvider.family<Product, String>((ref, id) async {
  final result = await ref.watch(getProductUseCaseProvider)(id);
  return result.match(
    (failure) => throw Exception(failure.message),
    (product) => product,
  );
});
