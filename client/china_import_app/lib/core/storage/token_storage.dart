import '../constants/app_constants.dart';
import 'secure_storage_service.dart';

/// Auth token bundle persisted securely on the device.
class AuthTokens {
  const AuthTokens({required this.accessToken, required this.refreshToken});

  final String accessToken;
  final String refreshToken;
}

/// Persists and retrieves JWT access/refresh tokens via [SecureStorageService].
class TokenStorage {
  TokenStorage(this._storage);

  final SecureStorageService _storage;

  Future<void> saveTokens(AuthTokens tokens) async {
    await _storage.write(AppConstants.keyAccessToken, tokens.accessToken);
    await _storage.write(AppConstants.keyRefreshToken, tokens.refreshToken);
  }

  Future<String?> getAccessToken() =>
      _storage.read(AppConstants.keyAccessToken);

  Future<String?> getRefreshToken() =>
      _storage.read(AppConstants.keyRefreshToken);

  Future<AuthTokens?> getTokens() async {
    final access = await getAccessToken();
    final refresh = await getRefreshToken();
    if (access == null || refresh == null) return null;
    return AuthTokens(accessToken: access, refreshToken: refresh);
  }

  Future<bool> hasTokens() async => (await getAccessToken()) != null;

  Future<void> clear() async {
    await _storage.delete(AppConstants.keyAccessToken);
    await _storage.delete(AppConstants.keyRefreshToken);
    await _storage.delete(AppConstants.keyUserId);
    await _storage.delete(AppConstants.keyUserRole);
  }
}
