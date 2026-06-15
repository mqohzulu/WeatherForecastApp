import '../../../../core/network/api_result.dart';
import '../../../../shared/models/paginated_response.dart';
import '../entities/announcement.dart';

/// Contract for fetching announcements.
abstract interface class AnnouncementsRepository {
  FutureResult<PaginatedResponse<Announcement>> getAnnouncements({
    int page = 1,
    int pageSize = 20,
  });
}
