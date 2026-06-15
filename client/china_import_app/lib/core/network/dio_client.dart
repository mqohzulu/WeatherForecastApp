import 'package:dio/dio.dart';

import '../config/app_config.dart';
import '../storage/token_storage.dart';
import 'api_interceptors.dart';

/// Builds and configures the application's [Dio] instance.
class DioClient {
  DioClient({
    required AppConfig config,
    required TokenStorage tokenStorage,
    required Future<void> Function() onSessionExpired,
  }) {
    final baseOptions = BaseOptions(
      baseUrl: config.apiUrl,
      connectTimeout: config.connectTimeout,
      receiveTimeout: config.receiveTimeout,
      contentType: Headers.jsonContentType,
      responseType: ResponseType.json,
      headers: {'Accept': 'application/json'},
      // We map status codes ourselves in NetworkExceptions.
      validateStatus: (status) => status != null && status < 400,
    );

    // A bare Dio used by AuthInterceptor for refresh + retry so the auth
    // interceptor does not recurse into itself.
    _refreshDio = Dio(baseOptions);

    _dio = Dio(baseOptions)
      ..interceptors.addAll([
        AuthInterceptor(
          tokenStorage: tokenStorage,
          refreshDio: _refreshDio,
          onSessionExpired: onSessionExpired,
        ),
        LoggingInterceptor(enabled: config.enableLogging),
      ]);
  }

  late final Dio _dio;
  late final Dio _refreshDio;

  Dio get dio => _dio;
}
