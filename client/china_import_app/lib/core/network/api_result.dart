import 'package:fpdart/fpdart.dart';

import '../error/failure.dart';

/// Standard result type used across repositories and use cases.
///
/// `Left`  = a [Failure]
/// `Right` = the success value of type `T`
typedef Result<T> = Either<Failure, T>;

/// Async variant returned by most repository methods.
typedef FutureResult<T> = Future<Either<Failure, T>>;

/// Convenience constructors and helpers around [Result].
extension ResultX<T> on Result<T> {
  bool get isSuccess => isRight();
  bool get isFailure => isLeft();

  /// Returns the success value or `null`.
  T? get valueOrNull => getRight().toNullable();

  /// Returns the [Failure] or `null`.
  Failure? get failureOrNull => getLeft().toNullable();
}

/// Helpers to build results without importing fpdart everywhere.
Result<T> success<T>(T value) => Right<Failure, T>(value);
Result<T> failure<T>(Failure f) => Left<Failure, T>(f);
