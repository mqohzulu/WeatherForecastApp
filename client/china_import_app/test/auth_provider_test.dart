import 'package:china_import_app/core/network/api_result.dart';
import 'package:china_import_app/core/error/failure.dart';
import 'package:china_import_app/features/auth/domain/entities/user.dart';
import 'package:china_import_app/features/auth/domain/repositories/auth_repository.dart';
import 'package:china_import_app/features/auth/presentation/providers/auth_provider.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

class MockAuthRepository extends Mock implements AuthRepository {}

const _testUser = User(
  id: 'u1',
  phoneNumber: '+27821234567',
  role: UserRole.customer,
);

ProviderContainer _makeContainer(AuthRepository repo) {
  final container = ProviderContainer(
    overrides: [authRepositoryProvider.overrideWithValue(repo)],
  );
  addTearDown(container.dispose);
  return container;
}

void main() {
  group('AuthNotifier', () {
    late MockAuthRepository repo;

    setUp(() {
      repo = MockAuthRepository();
      // Default: no existing session on construction.
      when(() => repo.currentUser())
          .thenAnswer((_) async => success<User?>(null));
    });

    test('starts unauthenticated when no session exists', () async {
      final container = _makeContainer(repo);

      // Allow the async _restore() in the constructor to settle.
      container.read(authProvider);
      await Future<void>.delayed(Duration.zero);

      expect(container.read(authProvider).status, AuthStatus.unauthenticated);
      expect(container.read(authProvider).isAuthenticated, isFalse);
    });

    test('requestOtp stores verification id and returns true on success',
        () async {
      when(() => repo.requestOtp(any()))
          .thenAnswer((_) async => success('verif-123'));

      final container = _makeContainer(repo);
      await Future<void>.delayed(Duration.zero);

      final ok =
          await container.read(authProvider.notifier).requestOtp('0821234567');

      expect(ok, isTrue);
      final state = container.read(authProvider);
      expect(state.verificationId, 'verif-123');
      expect(state.pendingPhoneNumber, '0821234567');
      expect(state.isSubmitting, isFalse);
      expect(state.errorMessage, isNull);
    });

    test('requestOtp surfaces an error message on failure', () async {
      when(() => repo.requestOtp(any()))
          .thenAnswer((_) async => failure<String>(const NetworkFailure()));

      final container = _makeContainer(repo);
      await Future<void>.delayed(Duration.zero);

      final ok =
          await container.read(authProvider.notifier).requestOtp('0821234567');

      expect(ok, isFalse);
      expect(container.read(authProvider).errorMessage, isNotNull);
    });

    test('verifyOtp authenticates the user on success', () async {
      when(() => repo.requestOtp(any()))
          .thenAnswer((_) async => success('verif-123'));
      when(
        () => repo.verifyOtp(
          phoneNumber: any(named: 'phoneNumber'),
          verificationId: any(named: 'verificationId'),
          code: any(named: 'code'),
        ),
      ).thenAnswer((_) async => success(_testUser));

      final container = _makeContainer(repo);
      await Future<void>.delayed(Duration.zero);

      await container.read(authProvider.notifier).requestOtp('0821234567');
      final ok = await container.read(authProvider.notifier).verifyOtp('123456');

      expect(ok, isTrue);
      final state = container.read(authProvider);
      expect(state.status, AuthStatus.authenticated);
      expect(state.user?.id, 'u1');
    });

    test('verifyOtp fails when no OTP was requested first', () async {
      final container = _makeContainer(repo);
      await Future<void>.delayed(Duration.zero);

      final ok = await container.read(authProvider.notifier).verifyOtp('123456');

      expect(ok, isFalse);
      expect(container.read(authProvider).errorMessage, isNotNull);
    });

    test('logout resets state to unauthenticated', () async {
      when(() => repo.logout())
          .thenAnswer((_) async => success<void>(null));

      final container = _makeContainer(repo);
      await Future<void>.delayed(Duration.zero);

      await container.read(authProvider.notifier).logout();

      expect(container.read(authProvider).status, AuthStatus.unauthenticated);
      expect(container.read(authProvider).user, isNull);
    });
  });
}
