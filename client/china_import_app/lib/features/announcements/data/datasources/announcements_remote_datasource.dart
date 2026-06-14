import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';
import '../../../../shared/models/paginated_response.dart';
import '../models/announcement_model.dart';

/// Remote calls for the announcements feature.
abstract interface class AnnouncementsRemoteDataSource {
  Future<PaginatedResponse<AnnouncementModel>> getAnnouncements({
    int page = 1,
    int pageSize = 20,
  });
}

class AnnouncementsRemoteDataSourceImpl
    implements AnnouncementsRemoteDataSource {
  AnnouncementsRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<PaginatedResponse<AnnouncementModel>> getAnnouncements({
    int page = 1,
    int pageSize = 20,
  }) async {
    final res = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.announcements,
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return PaginatedResponse.fromJson(
      res.data!,
      (json) => AnnouncementModel.fromJson(json),
    );
  }
}
