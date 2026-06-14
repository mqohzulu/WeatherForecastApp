import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../config/app_config.dart';
import '../network/dio_client.dart';
import '../storage/secure_storage_service.dart';
import '../storage/token_storage.dart';

/// Active application configuration (env-selected).
final appConfigProvider = Provider<AppConfig>((ref) {
  return AppConfig.instance;
});

/// Secure storage wrapper.
final secureStorageProvider = Provider<SecureStorageService>((ref) {
  return SecureStorageService();
});

/// JWT token persistence.
final tokenStorageProvider = Provider<TokenStorage>((ref) {
  return TokenStorage(ref.watch(secureStorageProvider));
});

/// Signal raised when the session expires and the user must re-authenticate.
///
/// Listened to by the router/auth layer. `DioClient` flips this to `true` when
/// a token refresh ultimately fails.
final sessionExpiredProvider = StateProvider<bool>((ref) => false);

/// Configured Dio client.
final dioClientProvider = Provider<DioClient>((ref) {
  final client = DioClient(
    config: ref.watch(appConfigProvider),
    tokenStorage: ref.watch(tokenStorageProvider),
    onSessionExpired: () async {
      ref.read(sessionExpiredProvider.notifier).state = true;
    },
  );
  return client;
});

/// The shared [Dio] instance most data sources depend on.
final dioProvider = Provider<Dio>((ref) {
  return ref.watch(dioClientProvider).dio;
});
