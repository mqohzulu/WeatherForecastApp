import '../../../../core/constants/app_constants.dart';
import '../../../../core/network/api_result.dart';
import '../../../../core/network/repository_helper.dart';
import '../../../../core/storage/secure_storage_service.dart';
import '../../../../core/storage/token_storage.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_datasource.dart';

class AuthRepositoryImpl with RepositoryHelper implements AuthRepository {
  AuthRepositoryImpl({
    required AuthRemoteDataSource remote,
    required TokenStorage tokenStorage,
    required SecureStorageService secureStorage,
  })  : _remote = remote,
        _tokens = tokenStorage,
        _storage = secureStorage;

  final AuthRemoteDataSource _remote;
  final TokenStorage _tokens;
  final SecureStorageService _storage;

  @override
  FutureResult<String> requestOtp(String phoneNumber) {
    return guard(() async {
      final res = await _remote.requestOtp(phoneNumber);
      return res.verificationId;
    });
  }

  @override
  FutureResult<User> verifyOtp({
    required String phoneNumber,
    required String verificationId,
    required String code,
  }) {
    return guard(() async {
      final res = await _remote.verifyOtp(
        phoneNumber: phoneNumber,
        verificationId: verificationId,
        code: code,
      );
      await _tokens.saveTokens(
        AuthTokens(
          accessToken: res.accessToken,
          refreshToken: res.refreshToken,
        ),
      );
      final user = res.user.toEntity();
      await _cacheIdentity(user);
      return user;
    });
  }

  @override
  FutureResult<User?> currentUser() {
    return guard(() async {
      if (!await _tokens.hasTokens()) return null;
      final model = await _remote.me();
      final user = model.toEntity();
      await _cacheIdentity(user);
      return user;
    });
  }

  @override
  FutureResult<void> logout() {
    return guard(() async {
      try {
        await _remote.logout();
      } finally {
        await _tokens.clear();
      }
    });
  }

  @override
  Future<bool> hasSession() => _tokens.hasTokens();

  Future<void> _cacheIdentity(User user) async {
    await _storage.write(AppConstants.keyUserId, user.id);
    await _storage.write(AppConstants.keyUserRole, user.role.name);
  }
}
