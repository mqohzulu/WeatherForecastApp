import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../utils/logger.dart';

/// Push-notification facade (FCM / APNs).
///
/// This is a STUB for the manual phase. A future implementation will:
///  - request notification permissions,
///  - obtain the FCM/APNs device token,
///  - register the token via `POST /api/v1/devices`,
///  - listen for foreground/background messages and route the user
///    to the relevant order/conversation.
abstract interface class PushNotificationService {
  Future<void> initialise();

  /// Returns the device push token, or `null` if unavailable / denied.
  Future<String?> getToken();

  /// Registers [token] with the backend so the seller can target this device.
  Future<void> registerDevice(String token);

  Future<void> dispose();
}

/// No-op implementation used until FCM/APNs is wired in.
class NoopPushNotificationService implements PushNotificationService {
  const NoopPushNotificationService();

  static const _log = AppLogger('Push');

  @override
  Future<void> initialise() async {
    _log.i('Push notifications stubbed (NoopPushNotificationService)');
  }

  @override
  Future<String?> getToken() async => null;

  @override
  Future<void> registerDevice(String token) async {
    _log.d('Would register device token: $token');
  }

  @override
  Future<void> dispose() async {}
}

final pushNotificationServiceProvider =
    Provider<PushNotificationService>((ref) {
  return const NoopPushNotificationService();
});
