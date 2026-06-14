import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';

/// Remote calls for the messaging feature.
///
/// Returns raw maps; the repository maps them to domain entities. This keeps
/// the data source light while remaining real (no mock data).
abstract interface class MessagingRemoteDataSource {
  Future<List<Map<String, dynamic>>> getConversations();

  Future<List<Map<String, dynamic>>> getMessages(String conversationId);

  Future<void> sendMessage(String conversationId, String body);
}

class MessagingRemoteDataSourceImpl implements MessagingRemoteDataSource {
  MessagingRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<List<Map<String, dynamic>>> getConversations() async {
    final res = await _dio.get<List<dynamic>>(ApiEndpoints.conversations);
    return _asMapList(res.data);
  }

  @override
  Future<List<Map<String, dynamic>>> getMessages(String conversationId) async {
    final res = await _dio.get<List<dynamic>>(
      ApiEndpoints.conversationMessages(conversationId),
    );
    return _asMapList(res.data);
  }

  @override
  Future<void> sendMessage(String conversationId, String body) async {
    await _dio.post<void>(
      ApiEndpoints.conversationMessages(conversationId),
      data: {'body': body},
    );
  }

  List<Map<String, dynamic>> _asMapList(List<dynamic>? raw) {
    return (raw ?? <dynamic>[])
        .whereType<Map<String, dynamic>>()
        .toList(growable: false);
  }
}
