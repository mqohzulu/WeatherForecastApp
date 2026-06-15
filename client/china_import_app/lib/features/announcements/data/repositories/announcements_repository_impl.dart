import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../domain/entities/announcement.dart';
import '../../domain/repositories/announcements_repository.dart';
import '../datasources/announcements_remote_datasource.dart';

class AnnouncementsRepositoryImpl
    with RepositoryHelper
    implements AnnouncementsRepository {
  AnnouncementsRepositoryImpl({required AnnouncementsRemoteDataSource remote})
      : _remote = remote;

  final AnnouncementsRemoteDataSource _remote;

  @override
  FutureResult<PaginatedResponse<Announcement>> getAnnouncements({
    int page = 1,
    int pageSize = 20,
  }) {
    return guard(() async {
      final response = await _remote.getAnnouncements(
        page: page,
        pageSize: pageSize,
      );
      return response.map((model) => model.toEntity());
    });
  }
}
