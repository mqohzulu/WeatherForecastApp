import '../../../../core/network/api_result.dart';
import '../entities/conversation.dart';
import '../entities/message.dart';

/// Contract for the messaging feature.
abstract interface class MessagingRepository {
  FutureResult<List<Conversation>> getConversations();

  FutureResult<List<Message>> getMessages(String conversationId);

  FutureResult<void> sendMessage(String conversationId, String body);
}
