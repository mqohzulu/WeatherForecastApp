import 'package:dio/dio.dart';

import '../error/exceptions.dart';
import '../error/failure.dart';
import 'api_result.dart';
import 'network_exceptions.dart';

/// Mixin that wraps remote calls in a [Result], converting Dio/typed
/// exceptions into [Failure]s. Mix into repository implementations.
mixin RepositoryHelper {
  /// Executes [request] and maps any error into a [Failure].
  Future<Result<T>> guard<T>(Future<T> Function() request) async {
    try {
      final value = await request();
      return success(value);
    } on DioException catch (e) {
      return failure(_mapException(NetworkExceptions.fromDio(e)));
    } on AppException catch (e) {
      return failure(_mapException(e));
    } catch (e) {
      return failure(UnknownFailure(e.toString()));
    }
  }

  Failure _mapException(AppException e) {
    return switch (e) {
      NetworkException() => NetworkFailure(e.message),
      TimeoutException() => TimeoutFailure(e.message),
      UnauthorizedException() => AuthFailure(e.message),
      NotFoundException() => NotFoundFailure(e.message),
      ValidationException() =>
        ValidationFailure(e.message, fieldErrors: e.errors),
      ServerException() => ServerFailure(e.message, code: e.statusCode),
      CacheException() => CacheFailure(e.message),
      UnknownException() => UnknownFailure(e.message),
    };
  }
}
