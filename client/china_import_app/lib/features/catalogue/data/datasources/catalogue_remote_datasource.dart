import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';
import '../../../../shared/models/paginated_response.dart';
import '../models/category_model.dart';
import '../models/product_model.dart';

/// Remote calls for the catalogue feature.
abstract interface class CatalogueRemoteDataSource {
  Future<List<CategoryModel>> getCategories();

  Future<PaginatedResponse<ProductModel>> getProducts({
    String? categoryId,
    String? search,
    int page,
    int pageSize,
  });

  Future<ProductModel> getProduct(String id);
}

class CatalogueRemoteDataSourceImpl implements CatalogueRemoteDataSource {
  CatalogueRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<List<CategoryModel>> getCategories() async {
    final res = await _dio.get<List<dynamic>>(ApiEndpoints.categories);
    final data = res.data ?? <dynamic>[];
    return data
        .map((e) => CategoryModel.fromJson(e as Map<String, dynamic>))
        .toList(growable: false);
  }

  @override
  Future<PaginatedResponse<ProductModel>> getProducts({
    String? categoryId,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) async {
    final res = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.products,
      queryParameters: <String, dynamic>{
        if (categoryId != null) 'categoryId': categoryId,
        if (search != null && search.isNotEmpty) 'q': search,
        'page': page,
        'pageSize': pageSize,
      },
    );
    return PaginatedResponse<ProductModel>.fromJson(
      res.data!,
      (json) => ProductModel.fromJson(json),
    );
  }

  @override
  Future<ProductModel> getProduct(String id) async {
    final res = await _dio.get<Map<String, dynamic>>(ApiEndpoints.product(id));
    return ProductModel.fromJson(res.data!);
  }
}
