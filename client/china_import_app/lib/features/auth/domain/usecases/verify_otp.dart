import '../../../../core/network/api_result.dart';
import '../../../../core/utils/validators.dart';
import '../entities/user.dart';
import '../repositories/auth_repository.dart';

/// Verifies an OTP code and, on success, returns the authenticated user.
class VerifyOtp {
  const VerifyOtp(this._repository);

  final AuthRepository _repository;

  FutureResult<User> call({
    required String phoneNumber,
    required String verificationId,
    required String code,
  }) {
    return _repository.verifyOtp(
      phoneNumber: Validators.normaliseSaMobile(phoneNumber),
      verificationId: verificationId,
      code: code,
    );
  }
}
