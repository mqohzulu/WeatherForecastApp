import '../../../../core/network/api_result.dart';
import '../../../../core/utils/validators.dart';
import '../repositories/auth_repository.dart';

/// Requests an OTP for a (possibly local-format) SA mobile number.
///
/// Normalises the number to E.164 before calling the repository, and returns
/// the server verification id.
class RequestOtp {
  const RequestOtp(this._repository);

  final AuthRepository _repository;

  FutureResult<String> call(String phoneNumber) {
    final normalised = Validators.normaliseSaMobile(phoneNumber);
    return _repository.requestOtp(normalised);
  }
}
