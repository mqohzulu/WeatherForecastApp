import '../../../../core/network/api_result.dart';
import '../entities/user.dart';

/// Abstract authentication contract consumed by the domain/presentation layers.
abstract interface class AuthRepository {
  /// Requests an OTP be sent to [phoneNumber] (E.164 format).
  ///
  /// Returns a server-issued verification id used in [verifyOtp].
  FutureResult<String> requestOtp(String phoneNumber);

  /// Verifies [code] against the [verificationId] for [phoneNumber].
  ///
  /// On success persists tokens and returns the authenticated [User].
  FutureResult<User> verifyOtp({
    required String phoneNumber,
    required String verificationId,
    required String code,
  });

  /// Returns the locally cached/authenticated user, if a valid session exists.
  FutureResult<User?> currentUser();

  /// Clears tokens and any cached identity.
  FutureResult<void> logout();

  /// Whether a token is currently stored on the device.
  Future<bool> hasSession();
}
