import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../data/datasources/messaging_remote_datasource.dart';
import '../../data/repositories/messaging_repository_impl.dart';
import '../../domain/entities/conversation.dart';
import '../../domain/entities/message.dart';
import '../../domain/repositories/messaging_repository.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final messagingRemoteDataSourceProvider =
    Provider<MessagingRemoteDataSource>((ref) {
  return MessagingRemoteDataSourceImpl(ref.watch(dioProvider));
});

final messagingRepositoryProvider = Provider<MessagingRepository>((ref) {
  return MessagingRepositoryImpl(
    remote: ref.watch(messagingRemoteDataSourceProvider),
  );
});

// ---------------------------------------------------------------------------
// Screen state
// ---------------------------------------------------------------------------

final conversationsProvider =
    FutureProvider<List<Conversation>>((ref) async {
  final result = await ref.watch(messagingRepositoryProvider).getConversations();
  return result.match(
    (failure) => throw Exception(failure.message),
    (conversations) => conversations,
  );
});

final messagesProvider =
    FutureProvider.family<List<Message>, String>((ref, conversationId) async {
  final result =
      await ref.watch(messagingRepositoryProvider).getMessages(conversationId);
  return result.match(
    (failure) => throw Exception(failure.message),
    (messages) => messages,
  );
});
