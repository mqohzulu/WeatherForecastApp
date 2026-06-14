import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/message.dart';
import '../../domain/repositories/messaging_repository.dart';
import '../datasources/messaging_remote_datasource.dart';

class MessagingRepositoryImpl
    with RepositoryHelper
    implements MessagingRepository {
  MessagingRepositoryImpl({required MessagingRemoteDataSource remote})
      : _remote = remote;

  final MessagingRemoteDataSource _remote;

  @override
  FutureResult<List<Conversation>> getConversations() {
    return guard(() async {
      final raw = await _remote.getConversations();
      return raw.map(_toConversation).toList(growable: false);
    });
  }

  @override
  FutureResult<List<Message>> getMessages(String conversationId) {
    return guard(() async {
      final raw = await _remote.getMessages(conversationId);
      return raw
          .map((json) => _toMessage(json, conversationId))
          .toList(growable: false);
    });
  }

  @override
  FutureResult<void> sendMessage(String conversationId, String body) {
    return guard(() => _remote.sendMessage(conversationId, body));
  }

  Conversation _toConversation(Map<String, dynamic> json) {
    final lastAt = json['lastMessageAt'] as String?;
    return Conversation(
      id: (json['id'] as String?) ?? '',
      title: (json['title'] as String?) ?? 'Conversation',
      lastMessagePreview: json['lastMessagePreview'] as String?,
      lastMessageAt: lastAt == null ? null : DateTime.tryParse(lastAt),
      unreadCount: (json['unreadCount'] as num?)?.toInt() ?? 0,
    );
  }

  Message _toMessage(Map<String, dynamic> json, String conversationId) {
    final sentAt = json['sentAt'] as String?;
    return Message(
      id: (json['id'] as String?) ?? '',
      conversationId: (json['conversationId'] as String?) ?? conversationId,
      body: (json['body'] as String?) ?? '',
      isMine: (json['isMine'] as bool?) ?? false,
      sentAt: (sentAt == null ? null : DateTime.tryParse(sentAt)) ??
          DateTime.now(),
    );
  }
}
