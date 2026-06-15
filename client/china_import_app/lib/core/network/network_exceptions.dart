import 'package:dio/dio.dart';

import '../error/exceptions.dart';

/// Translates a [DioException] into a typed [AppException].
class NetworkExceptions {
  NetworkExceptions._();

  static AppException fromDio(DioException error) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return const TimeoutException();

      case DioExceptionType.connectionError:
        return const NetworkException();

      case DioExceptionType.cancel:
        return const UnknownException('Request was cancelled');

      case DioExceptionType.badCertificate:
        return const NetworkException('Invalid server certificate');

      case DioExceptionType.badResponse:
        return _fromResponse(error.response);

      case DioExceptionType.unknown:
        return const UnknownException();
    }
  }

  static AppException _fromResponse(Response<dynamic>? response) {
    final statusCode = response?.statusCode ?? 0;
    final message = _extractMessage(response?.data) ?? 'Request failed';

    switch (statusCode) {
      case 400:
      case 422:
        return ValidationException(
          message,
          errors: _extractFieldErrors(response?.data),
          statusCode: statusCode,
        );
      case 401:
        return UnauthorizedException(message);
      case 404:
        return NotFoundException(message);
      case >= 500:
        return ServerException(
          'A server error occurred. Please try again later.',
          statusCode: statusCode,
        );
      default:
        return ServerException(message, statusCode: statusCode);
    }
  }

  /// The .NET API returns ProblemDetails-style payloads:
  /// `{ "title": "...", "detail": "...", "errors": { "field": ["..."] } }`
  static String? _extractMessage(dynamic data) {
    if (data is Map<String, dynamic>) {
      return (data['detail'] ?? data['message'] ?? data['title'])?.toString();
    }
    if (data is String && data.isNotEmpty) return data;
    return null;
  }

  static Map<String, List<String>>? _extractFieldErrors(dynamic data) {
    if (data is Map<String, dynamic> && data['errors'] is Map) {
      final raw = data['errors'] as Map;
      return raw.map(
        (key, value) => MapEntry(
          key.toString(),
          (value as List).map((e) => e.toString()).toList(),
        ),
      );
    }
    return null;
  }
}
