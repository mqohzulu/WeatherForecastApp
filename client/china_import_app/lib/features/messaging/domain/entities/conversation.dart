/// Domain representation of a messaging conversation/thread.
class Conversation {
  const Conversation({
    required this.id,
    required this.title,
    this.lastMessagePreview,
    this.lastMessageAt,
    this.unreadCount = 0,
  });

  final String id;
  final String title;
  final String? lastMessagePreview;
  final DateTime? lastMessageAt;
  final int unreadCount;
}
