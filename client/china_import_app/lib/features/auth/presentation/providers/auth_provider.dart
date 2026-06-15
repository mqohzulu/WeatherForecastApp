import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/di/providers.dart';
import '../../data/datasources/auth_remote_datasource.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../domain/entities/user.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/usecases/request_otp.dart';
import '../../domain/usecases/verify_otp.dart';

// ---------------------------------------------------------------------------
// Dependency wiring
// ---------------------------------------------------------------------------

final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>((ref) {
  return AuthRemoteDataSourceImpl(ref.watch(dioProvider));
});

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  return AuthRepositoryImpl(
    remote: ref.watch(authRemoteDataSourceProvider),
    tokenStorage: ref.watch(tokenStorageProvider),
    secureStorage: ref.watch(secureStorageProvider),
  );
});

final requestOtpUseCaseProvider = Provider<RequestOtp>((ref) {
  return RequestOtp(ref.watch(authRepositoryProvider));
});

final verifyOtpUseCaseProvider = Provider<VerifyOtp>((ref) {
  return VerifyOtp(ref.watch(authRepositoryProvider));
});

// ---------------------------------------------------------------------------
// Auth state
// ---------------------------------------------------------------------------

enum AuthStatus { unknown, unauthenticated, authenticated }

class AuthState {
  const AuthState({
    this.status = AuthStatus.unknown,
    this.user,
    this.isSubmitting = false,
    this.errorMessage,
    this.pendingPhoneNumber,
    this.verificationId,
  });

  final AuthStatus status;
  final User? user;
  final bool isSubmitting;
  final String? errorMessage;

  /// Phone number awaiting OTP verification.
  final String? pendingPhoneNumber;

  /// Server verification id returned by request-otp.
  final String? verificationId;

  bool get isAuthenticated => status == AuthStatus.authenticated;
  bool get isSeller => user?.role.isSeller ?? false;

  AuthState copyWith({
    AuthStatus? status,
    User? user,
    bool? isSubmitting,
    String? errorMessage,
    bool clearError = false,
    String? pendingPhoneNumber,
    String? verificationId,
  }) {
    return AuthState(
      status: status ?? this.status,
      user: user ?? this.user,
      isSubmitting: isSubmitting ?? this.isSubmitting,
      errorMessage: clearError ? null : (errorMessage ?? this.errorMessage),
      pendingPhoneNumber: pendingPhoneNumber ?? this.pendingPhoneNumber,
      verificationId: verificationId ?? this.verificationId,
    );
  }
}

class AuthNotifier extends StateNotifier<AuthState> {
  AuthNotifier(this._ref) : super(const AuthState()) {
    _restore();
  }

  final Ref _ref;

  AuthRepository get _repo => _ref.read(authRepositoryProvider);

  /// Restores an existing session on app start.
  Future<void> _restore() async {
    final result = await _repo.currentUser();
    result.match(
      (_) => state = state.copyWith(status: AuthStatus.unauthenticated),
      (user) => state = user == null
          ? state.copyWith(status: AuthStatus.unauthenticated)
          : state.copyWith(status: AuthStatus.authenticated, user: user),
    );
  }

  /// Step 1 — send the OTP. Returns true on success.
  Future<bool> requestOtp(String phoneNumber) async {
    state = state.copyWith(isSubmitting: true, clearError: true);
    final result = await _ref.read(requestOtpUseCaseProvider)(phoneNumber);
    return result.match(
      (failure) {
        state = state.copyWith(
          isSubmitting: false,
          errorMessage: failure.message,
        );
        return false;
      },
      (verificationId) {
        state = state.copyWith(
          isSubmitting: false,
          pendingPhoneNumber: phoneNumber,
          verificationId: verificationId,
        );
        return true;
      },
    );
  }

  /// Step 2 — verify the OTP code. Returns true on success.
  Future<bool> verifyOtp(String code) async {
    final phone = state.pendingPhoneNumber;
    final verificationId = state.verificationId;
    if (phone == null || verificationId == null) {
      state = state.copyWith(errorMessage: 'Please request a new code.');
      return false;
    }

    state = state.copyWith(isSubmitting: true, clearError: true);
    final result = await _ref.read(verifyOtpUseCaseProvider)(
      phoneNumber: phone,
      verificationId: verificationId,
      code: code,
    );
    return result.match(
      (failure) {
        state = state.copyWith(
          isSubmitting: false,
          errorMessage: failure.message,
        );
        return false;
      },
      (user) {
        state = state.copyWith(
          isSubmitting: false,
          status: AuthStatus.authenticated,
          user: user,
        );
        return true;
      },
    );
  }

  Future<void> logout() async {
    await _repo.logout();
    state = const AuthState(status: AuthStatus.unauthenticated);
  }
}

final authProvider = StateNotifierProvider<AuthNotifier, AuthState>((ref) {
  return AuthNotifier(ref);
});
