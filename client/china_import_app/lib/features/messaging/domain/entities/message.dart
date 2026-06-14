/// Domain representation of a single chat message.
class Message {
  const Message({
    required this.id,
    required this.conversationId,
    required this.body,
    required this.isMine,
    required this.sentAt,
  });

  final String id;
  final String conversationId;
  final String body;
  final bool isMine;
  final DateTime sentAt;
}
