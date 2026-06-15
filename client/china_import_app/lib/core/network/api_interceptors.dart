import 'package:dio/dio.dart';

import '../constants/api_endpoints.dart';
import '../storage/token_storage.dart';
import '../utils/logger.dart';

/// Attaches the Bearer token to outgoing requests and transparently refreshes
/// it once on a 401 response.
class AuthInterceptor extends QueuedInterceptor {
  AuthInterceptor({
    required TokenStorage tokenStorage,
    required Dio refreshDio,
    required this.onSessionExpired,
  })  : _tokens = tokenStorage,
        _refreshDio = refreshDio;

  final TokenStorage _tokens;

  /// A *separate* Dio instance (without this interceptor) used to call the
  /// refresh endpoint, avoiding infinite interceptor recursion.
  final Dio _refreshDio;

  /// Invoked when refresh fails — the app should route back to login.
  final Future<void> Function() onSessionExpired;

  static const _log = AppLogger('AuthInterceptor');

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    // Don't attach the token to the auth endpoints themselves.
    if (!_isAuthPath(options.path)) {
      final token = await _tokens.getAccessToken();
      if (token != null) {
        options.headers['Authorization'] = 'Bearer $token';
      }
    }
    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final response = err.response;
    final isAuthRequest = _isAuthPath(err.requestOptions.path);

    if (response?.statusCode != 401 || isAuthRequest) {
      return handler.next(err);
    }

    _log.w('Received 401 — attempting token refresh');
    final refreshed = await _tryRefresh();
    if (!refreshed) {
      await onSessionExpired();
      return handler.next(err);
    }

    try {
      final retried = await _retry(err.requestOptions);
      return handler.resolve(retried);
    } on DioException catch (e) {
      return handler.next(e);
    }
  }

  Future<bool> _tryRefresh() async {
    final refreshToken = await _tokens.getRefreshToken();
    if (refreshToken == null) return false;

    try {
      final res = await _refreshDio.post<Map<String, dynamic>>(
        ApiEndpoints.refresh,
        data: {'refreshToken': refreshToken},
      );
      final data = res.data;
      if (data == null) return false;

      final newAccess = data['accessToken'] as String?;
      final newRefresh = data['refreshToken'] as String? ?? refreshToken;
      if (newAccess == null) return false;

      await _tokens.saveTokens(
        AuthTokens(accessToken: newAccess, refreshToken: newRefresh),
      );
      _log.i('Token refresh succeeded');
      return true;
    } catch (e) {
      _log.e('Token refresh failed', e);
      await _tokens.clear();
      return false;
    }
  }

  Future<Response<dynamic>> _retry(RequestOptions requestOptions) {
    final token = requestOptions.headers['Authorization'];
    return _refreshDio.fetch<dynamic>(
      requestOptions.copyWith(
        headers: {
          ...requestOptions.headers,
          if (token != null) 'Authorization': token,
        },
      ),
    );
  }

  bool _isAuthPath(String path) =>
      path.contains(ApiEndpoints.requestOtp) ||
      path.contains(ApiEndpoints.verifyOtp) ||
      path.contains(ApiEndpoints.refresh);
}

/// Verbose request/response logging (enabled per [AppConfig.enableLogging]).
class LoggingInterceptor extends Interceptor {
  const LoggingInterceptor({this.enabled = true});

  final bool enabled;
  static const _log = AppLogger('HTTP');

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    if (enabled) {
      _log.d('--> ${options.method} ${options.uri}');
      if (options.data != null) _log.d('    body: ${options.data}');
    }
    handler.next(options);
  }

  @override
  void onResponse(Response<dynamic> response, ResponseInterceptorHandler handler) {
    if (enabled) {
      _log.d(
        '<-- ${response.statusCode} ${response.requestOptions.method} '
        '${response.requestOptions.uri}',
      );
    }
    handler.next(response);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    if (enabled) {
      _log.w(
        'xxx ${err.response?.statusCode} ${err.requestOptions.method} '
        '${err.requestOptions.uri} :: ${err.message}',
      );
    }
    handler.next(err);
  }
}
