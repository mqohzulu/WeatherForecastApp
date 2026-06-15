import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../../../shared/models/paginated_response.dart';
import '../../data/datasources/announcements_remote_datasource.dart';
import '../../data/repositories/announcements_repository_impl.dart';
import '../../domain/entities/announcement.dart';
import '../../domain/repositories/announcements_repository.dart';
import '../../domain/usecases/get_announcements.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final announcementsRemoteDataSourceProvider =
    Provider<AnnouncementsRemoteDataSource>((ref) {
  return AnnouncementsRemoteDataSourceImpl(ref.watch(dioProvider));
});

final announcementsRepositoryProvider =
    Provider<AnnouncementsRepository>((ref) {
  return AnnouncementsRepositoryImpl(
    remote: ref.watch(announcementsRemoteDataSourceProvider),
  );
});

final getAnnouncementsUseCaseProvider = Provider<GetAnnouncements>((ref) {
  return GetAnnouncements(ref.watch(announcementsRepositoryProvider));
});

// ---------------------------------------------------------------------------
// Screen state
// ---------------------------------------------------------------------------

/// Loads the first page of announcements.
final announcementsProvider =
    FutureProvider<PaginatedResponse<Announcement>>((ref) async {
  final result = await ref.watch(getAnnouncementsUseCaseProvider)();
  return result.match(
    (failure) => throw Exception(failure.message),
    (page) => page,
  );
});
