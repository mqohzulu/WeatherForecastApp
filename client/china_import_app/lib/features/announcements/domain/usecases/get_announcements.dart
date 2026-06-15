import '../../../../core/network/api_result.dart';
import '../../../../shared/models/paginated_response.dart';
import '../entities/announcement.dart';
import '../repositories/announcements_repository.dart';

/// Fetches a page of platform announcements.
class GetAnnouncements {
  const GetAnnouncements(this._repository);

  final AnnouncementsRepository _repository;

  FutureResult<PaginatedResponse<Announcement>> call({
    int page = 1,
    int pageSize = 20,
  }) {
    return _repository.getAnnouncements(page: page, pageSize: pageSize);
  }
}
