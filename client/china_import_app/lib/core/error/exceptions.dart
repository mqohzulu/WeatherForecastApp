/// Low-level exceptions thrown by data sources.
///
/// These are caught in repository implementations and mapped to
/// [Failure] objects from `failure.dart`.
sealed class AppException implements Exception {
  const AppException(this.message, {this.statusCode});

  final String message;
  final int? statusCode;

  @override
  String toString() => '$runtimeType($statusCode): $message';
}

class ServerException extends AppException {
  const ServerException(super.message, {super.statusCode});
}

class NetworkException extends AppException {
  const NetworkException([super.message = 'No internet connection']);
}

class TimeoutException extends AppException {
  const TimeoutException([super.message = 'The request timed out']);
}

class UnauthorizedException extends AppException {
  const UnauthorizedException([super.message = 'Session expired'])
      : super(statusCode: 401);
}

class NotFoundException extends AppException {
  const NotFoundException([super.message = 'Resource not found'])
      : super(statusCode: 404);
}

class ValidationException extends AppException {
  const ValidationException(super.message, {this.errors, super.statusCode = 422});

  /// Field-level validation errors keyed by field name.
  final Map<String, List<String>>? errors;
}

class CacheException extends AppException {
  const CacheException([super.message = 'Cache error']);
}

class UnknownException extends AppException {
  const UnknownException([super.message = 'An unexpected error occurred']);
}
