/// Domain representation of a platform announcement.
class Announcement {
  const Announcement({
    required this.id,
    required this.title,
    required this.body,
    required this.publishedAt,
    this.imageUrl,
    this.isPinned = false,
  });

  final String id;
  final String title;
  final String body;
  final DateTime publishedAt;
  final String? imageUrl;
  final bool isPinned;
}
