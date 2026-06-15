/// User-facing, layer-agnostic error type returned by repositories/use cases.
///
/// Repositories convert [AppException]s into one of these so the presentation
/// layer never depends on networking details.
sealed class Failure {
  const Failure(this.message, {this.code});

  final String message;
  final int? code;

  @override
  String toString() => '$runtimeType: $message';
}

class ServerFailure extends Failure {
  const ServerFailure(super.message, {super.code});
}

class NetworkFailure extends Failure {
  const NetworkFailure([super.message = 'No internet connection. Please try again.']);
}

class TimeoutFailure extends Failure {
  const TimeoutFailure([super.message = 'The request timed out. Please try again.']);
}

class AuthFailure extends Failure {
  const AuthFailure([super.message = 'Your session has expired. Please sign in again.'])
      : super(code: 401);
}

class NotFoundFailure extends Failure {
  const NotFoundFailure([super.message = 'The requested item could not be found.'])
      : super(code: 404);
}

class ValidationFailure extends Failure {
  const ValidationFailure(super.message, {this.fieldErrors, super.code = 422});

  final Map<String, List<String>>? fieldErrors;
}

class CacheFailure extends Failure {
  const CacheFailure([super.message = 'Could not read local data.']);
}

class UnknownFailure extends Failure {
  const UnknownFailure([super.message = 'Something went wrong. Please try again.']);
}
